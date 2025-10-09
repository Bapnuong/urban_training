using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    public Transform player;
    public float height = 50f;

    void LateUpdate()
    {
        if (player == null) return;

        // Di chuyển theo vị trí người chơi
        Vector3 newPos = player.position;
        newPos.y += height;
        transform.position = newPos;

        // Xoay theo hướng người chơi
        transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
    }
}
