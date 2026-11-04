
using UnityEngine;

public class DrumStickMatrix : MonoBehaviour
{
    public Transform leftStick;
    public Transform rightStick;

    public float swingSpeed = 2f;
    public float swingAngle = 8f;

    private Vector3 leftInitialPosition;
    private Vector3 rightInitialPosition;

    private Quaternion leftInitialRotation;
    private Quaternion rightInitialRotation;

    private Vector3 pivot;

    void Start()
    {
        // 처음 드럼 스틱의 위치와 회전 저장
        leftInitialPosition = leftStick.localPosition;
        rightInitialPosition = rightStick.localPosition;

        leftInitialRotation = leftStick.localRotation;
        rightInitialRotation = rightStick.localRotation;

        // 두 스틱의 중간점을 회전 중심으로 설정
        pivot = (leftInitialPosition + rightInitialPosition) / 2f;
    }

    void Update()
    {
        // 시간에 따라 회전 각도 계산
        float angle = Mathf.Sin(Time.time * swingSpeed) * swingAngle;

        // 왼쪽 스틱 행렬
        Matrix4x4 leftMatrix =
            Matrix4x4.TRS(
                pivot,
                Quaternion.Euler(0, 0, angle),
                Vector3.one
            ) *
            Matrix4x4.Translate(-pivot);

        // 오른쪽 스틱 행렬
        Matrix4x4 rightMatrix =
            Matrix4x4.TRS(
                pivot,
                Quaternion.Euler(0, 0, -angle),
                Vector3.one
            ) *
            Matrix4x4.Translate(-pivot);

        // 행렬을 이용해 위치와 회전 적용
        leftStick.localPosition =
            leftMatrix.MultiplyPoint3x4(leftInitialPosition);

        rightStick.localPosition =
            rightMatrix.MultiplyPoint3x4(rightInitialPosition);

        leftStick.localRotation =
            Matrix4x4.Rotate(Quaternion.Euler(0, 0, angle)).rotation
            * leftInitialRotation;

        rightStick.localRotation =
            Matrix4x4.Rotate(Quaternion.Euler(0, 0, -angle)).rotation
            * rightInitialRotation;
    }
}
