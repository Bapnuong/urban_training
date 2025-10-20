using UnityEngine;

public class ShowCursor : MonoBehaviour
{
    void Start()
    {
        Cursor.lockState = CursorLockMode.None; // mở khóa chuột
        Cursor.visible = true;                  // hiện chuột
    }
}
