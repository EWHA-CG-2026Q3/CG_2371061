
using UnityEngine;

public class SpotlightAnimation : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float panSpeed = 30f;
    public float tiltSpeed = 20f;

    [Header("Rotation Range")]
    public float panRange = 90f;
    public float tiltRange = 35f;

    [Header("Animation Offset")]
    [Range(0f, 360f)]
    public float phaseOffset = 0f;

    private Quaternion initialYokeRotation;
    private Quaternion initialHeadRotation;

    private Transform yoke;
    private Transform head;

    void Start()
    {
        yoke = transform.Find("Yoke");
        head = yoke != null ? yoke.Find("Head") : null;

        if (yoke == null || head == null)
        {
            Debug.LogError("Yoke 또는 Head를 찾을 수 없습니다.");
            enabled = false;
            return;
        }

        initialYokeRotation = yoke.localRotation;
        initialHeadRotation = head.localRotation;
    }

    void Update()
    {
        float time = Time.time;

        float phase = phaseOffset * Mathf.Deg2Rad;

        float pan = Mathf.Sin(
            time * panSpeed * Mathf.Deg2Rad + phase
        ) * panRange;

        float tilt = Mathf.Sin(
            time * tiltSpeed * Mathf.Deg2Rad + phase
        ) * tiltRange;

        yoke.localRotation =
            initialYokeRotation * Quaternion.Euler(0, pan, 0);

        head.localRotation =
            initialHeadRotation * Quaternion.Euler(tilt, 0, 0);
    }
}
