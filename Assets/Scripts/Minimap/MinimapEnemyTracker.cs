using UnityEngine;
using UnityEngine.UI;

public class MinimapEnemyTracker : MonoBehaviour
{
    public Transform player;
    public RectTransform minimapRect;
    public float mapScale = 2f;
    public GameObject enemyIconPrefab;
    public Transform[] enemies;
    public bool rotateWithPlayer = true;

    void Update()
    {
        foreach (Transform enemy in enemies)
        {
            if (enemy == null) continue;

            // Kiểm tra hoặc tạo icon
            if (enemy.GetComponent<EnemyIconRef>() == null)
            {
                GameObject icon = Instantiate(enemyIconPrefab, minimapRect);
                enemy.gameObject.AddComponent<EnemyIconRef>().icon = icon.GetComponent<RectTransform>();
            }

            RectTransform iconRect = enemy.GetComponent<EnemyIconRef>().icon;

            // --- Tính vị trí tương đối ---
            Vector3 offset = enemy.position - player.position;
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

public class EnemyIconRef : MonoBehaviour
{
    public RectTransform icon;
}
