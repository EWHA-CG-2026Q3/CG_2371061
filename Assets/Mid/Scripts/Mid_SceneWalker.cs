
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Mid_SceneWalker : MonoBehaviour
{
    [Header("Camera")]
    public Transform cameraTransform;

    [Header("Movement")]
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float mouseSensitivity = 0.15f;

    [Header("Gravity")]
    public float gravity = -20f;
    public float groundForce = -2f;

    private CharacterController controller;

    private float pitch = 0f;
    private float verticalVelocity = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // 카메라 자동 연결
        if (cameraTransform == null)
        {
            Camera mainCamera = GetComponentInChildren<Camera>();

            if (mainCamera != null)
                cameraTransform = mainCamera.transform;
        }

        // 커서 초기화
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 카메라 초기 회전값 반영
        if (cameraTransform != null)
            pitch = cameraTransform.localEulerAngles.x;

        if (pitch > 180f)
            pitch -= 360f;
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        Mouse mouse = Mouse.current;

        if (keyboard == null)
            return;

        // 마우스 잠금 및 해제
        if (mouse != null)
        {
            if (mouse.leftButton.wasPressedThisFrame)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            if (keyboard.escapeKey.wasPressedThisFrame)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        // 마우스 시점 이동
        if (mouse != null &&
            Cursor.lockState == CursorLockMode.Locked)
        {
            Vector2 mouseDelta = mouse.delta.ReadValue();

            float mouseX = mouseDelta.x * mouseSensitivity;
            float mouseY = mouseDelta.y * mouseSensitivity;

            transform.Rotate(0f, mouseX, 0f);

            pitch -= mouseY;
            pitch = Mathf.Clamp(pitch, -80f, 80f);

            if (cameraTransform != null)
            {
                cameraTransform.localRotation =
                    Quaternion.Euler(pitch, 0f, 0f);
            }
        }

        // 이동 입력
        float horizontal = 0f;
        float vertical = 0f;

        if (keyboard.wKey.isPressed ||
            keyboard.upArrowKey.isPressed)
            vertical += 1f;

        if (keyboard.sKey.isPressed ||
            keyboard.downArrowKey.isPressed)
            vertical -= 1f;

        if (keyboard.dKey.isPressed ||
            keyboard.rightArrowKey.isPressed)
            horizontal += 1f;

        if (keyboard.aKey.isPressed ||
            keyboard.leftArrowKey.isPressed)
            horizontal -= 1f;

        Vector3 input = new Vector3(
            horizontal, 0f, vertical
        );

        input = Vector3.ClampMagnitude(input, 1f);

        // 바라보는 방향 기준 이동
        Vector3 direction =
            transform.right * input.x +
            transform.forward * input.z;

        float speed = keyboard.leftShiftKey.isPressed ||
                      keyboard.rightShiftKey.isPressed
            ? runSpeed
            : walkSpeed;

        Vector3 movement = direction * speed;

        // 중력 처리
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = groundForce;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        movement.y = verticalVelocity;

        // 최종 이동
        controller.Move(movement * Time.deltaTime);
    }
}
