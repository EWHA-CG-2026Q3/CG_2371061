
using UnityEngine;

public class CymbalAnimation : MonoBehaviour
{
    [Header("Cymbal Objects")]
    public Transform[] cymbals = new Transform[4];

    [Header("Animation Settings")]
    public float swingSpeed = 5f;
    public float swingAngle = 4f;

    private Quaternion[] initialRotations;
    private float[] phaseOffsets;

    void Start()
    {
        initialRotations = new Quaternion[cymbals.Length];
        phaseOffsets = new float[cymbals.Length];

        for (int i = 0; i < cymbals.Length; i++)
        {
            if (cymbals[i] == null)
                continue;

            // 심벌의 처음 회전값 저장
            initialRotations[i] = cymbals[i].localRotation;

            // 심벌마다 서로 다른 흔들림 타이밍 설정
            phaseOffsets[i] = i * 1.5f;
        }
    }

    void Update()
    {
        for (int i = 0; i < cymbals.Length; i++)
        {
            if (cymbals[i] == null)
                continue;

            // 사인 함수로 흔들림 각도 계산
            float angle = Mathf.Sin(
                Time.time * swingSpeed + phaseOffsets[i]
            ) * swingAngle;

            // 심벌을 처음 회전 상태 기준으로 기울이기
            Quaternion swingRotation =
                Quaternion.Euler(angle, 0f, angle * 0.5f);

            cymbals[i].localRotation =
                initialRotations[i] * swingRotation;
        }
    }
}
