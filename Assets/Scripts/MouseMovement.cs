using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public Transform playerBody; // tham chiếu tới thân người
    public float mouseSensitivity = 100f;

    float xRotation = 0f;

    public float topClamp = -90f;
    public float bottomClamp = 90f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Xoay camera (ngẩng lên / cúi xuống)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, topClamp, bottomClamp);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Xoay thân người (quay trái / phải)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
