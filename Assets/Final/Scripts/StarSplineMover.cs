
using UnityEngine;
using UnityEngine.Splines;

public class StarSplineMover : MonoBehaviour
{
    [Header("Spline Path")]
    public SplineContainer splinePath;

    [Header("Movement")]
    [Range(0f, 1f)]
    public float startPosition = 0f;

    public float moveSpeed = 0.12f;

    [Header("Rotation")]
    public bool followPathDirection = true;

    private float progress;

    void Start()
    {
        progress = startPosition;
    }

    void Update()
    {
        if (splinePath == null)
            return;

        progress += moveSpeed * Time.deltaTime;

        // 경로 끝에 도달하면 처음으로 돌아감
        progress = Mathf.Repeat(progress, 1f);

        // Spline 위치 계산
        transform.position =
            splinePath.EvaluatePosition(progress);

        // 이동 방향을 따라 회전
        if (followPathDirection)
        {
            Vector3 direction =
                splinePath.EvaluateTangent(progress);

            if (direction.sqrMagnitude > 0.001f)
            {
                transform.rotation =
                    Quaternion.LookRotation(direction);
            }
        }
    }
}
