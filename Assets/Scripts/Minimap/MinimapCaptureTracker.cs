using UnityEngine;
using UnityEngine.UI;

public class MinimapCaptureTracker : MonoBehaviour
{
    public Transform player;
    public RectTransform minimapRect;
    public float mapScale = 2f;
    public GameObject captureIconPrefab;
    public Transform[] capturePoints;
    public bool rotateWithPlayer = true;

    void Update()
    {
        foreach (Transform point in capturePoints)
        {
            if (point == null) continue;

            // Kiểm tra hoặc tạo icon
            if (point.GetComponent<CaptureIconRef>() == null)
            {
                GameObject icon = Instantiate(captureIconPrefab, minimapRect);
                point.gameObject.AddComponent<CaptureIconRef>().icon = icon.GetComponent<RectTransform>();
            }

            RectTransform iconRect = point.GetComponent<CaptureIconRef>().icon;

            // --- Tính vị trí tương đối so với player ---
            Vector3 offset = point.position - player.position;
            Vector2 offset2D = new Vector2(offset.x, offset.z);

            // --- Quay offset ngược hướng player nếu minimap xoay theo player ---
            if (rotateWithPlayer)
            {
                float playerY = player.eulerAngles.y;
                offset2D = Quaternion.Euler(0, 0, playerY) * offset2D;
            }

            // --- Chuyển sang toạ độ minimap ---
            Vector2 minimapPos = offset2D * mapScale;
            minimapPos = Vector2.ClampMagnitude(minimapPos, minimapRect.rect.width / 2f);

            iconRect.anchoredPosition = minimapPos;
        }
    }
}

public class CaptureIconRef : MonoBehaviour
{
    public RectTransform icon;
}
