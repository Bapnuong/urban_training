using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    [Header("References")]
    public Transform playerBody; // tham chiếu tới thân người (object gốc của nhân vật)

    [Header("Mouse Settings")]
    public float mouseSensitivity = 100f;

    [Header("Rotation Clamp")]
    public float topClamp = -90f;
    public float bottomClamp = 90f;

    [Header("Smoothing")]
    public bool smoothLook = true; // bật/tắt xoay mượt
    public float smoothTime = 0.05f; // thời gian mượt

    private float xRotation = 0f;
    private float currentXRotation;
    private float rotationVelocity;

    void Start()
    {
        // Khóa chuột vào giữa màn hình (FPS style)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; // ẩn con trỏ cho chuyên nghiệp hơn
    }

    void Update()
    {
        // Lấy input chuột (nhân độ nhạy & deltaTime)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Xử lý góc xoay theo trục dọc (camera ngẩng/cúi)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, topClamp, bottomClamp);

        // Xoay camera (local rotation)
        if (smoothLook)
        {
            // Làm mượt xoay camera bằng SmoothDamp
            currentXRotation = Mathf.SmoothDamp(currentXRotation, xRotation, ref rotationVelocity, smoothTime);
            transform.localRotation = Quaternion.Euler(currentXRotation, 0f, 0f);
        }
        else
        {
            // Cách xoay cũ của bạn (giữ nguyên)
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        // Xoay thân người (quay trái/phải)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
