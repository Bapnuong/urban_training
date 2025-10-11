using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MinimapEnemyTracker : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public RectTransform minimapRect;
    public GameObject enemyIconPrefab;
    public float mapScale = 2f;
    public bool rotateWithPlayer = true;

    [Header("Enemy List")]
    public Transform[] enemies;

    private Dictionary<Transform, RectTransform> iconDict = new Dictionary<Transform, RectTransform>();

    void Update()
    {
        // 1️⃣ Duyệt tất cả enemies trong mảng
        foreach (Transform enemy in enemies)
        {
            // Nếu enemy đã bị destroy → skip, dọn ở bước 2
            if (enemy == null) continue;

            // Nếu chưa có icon → tạo icon
            if (!iconDict.ContainsKey(enemy))
            {
                GameObject icon = Instantiate(enemyIconPrefab, minimapRect);
                iconDict.Add(enemy, icon.GetComponent<RectTransform>());
            }

            // Lấy icon và cập nhật vị trí
            RectTransform iconRect = iconDict[enemy];
            Vector3 offset = enemy.position - player.position;
            Vector2 offset2D = new Vector2(offset.x, offset.z);

            // Quay theo hướng player (nếu bật)
            if (rotateWithPlayer)
            {
                float playerY = player.eulerAngles.y;
                offset2D = Quaternion.Euler(0, 0, playerY) * offset2D;
            }

            // Chuyển sang toạ độ minimap
            Vector2 minimapPos = offset2D * mapScale;
            minimapPos = Vector2.ClampMagnitude(minimapPos, minimapRect.rect.width / 2f);
            iconRect.anchoredPosition = minimapPos;
        }

        // 2️⃣ Dọn icon của enemy đã bị destroy (null)
        CleanUpDestroyedEnemies();
    }

    void CleanUpDestroyedEnemies()
    {
        // ✅ Duyệt qua dictionary thay vì FindObjectsByType (nhanh & an toàn hơn)
        List<Transform> destroyedEnemies = new List<Transform>();

        foreach (var kvp in iconDict)
        {
            if (kvp.Key == null)
            {
                Destroy(kvp.Value.gameObject); // xoá icon
                destroyedEnemies.Add(kvp.Key);
            }
        }

        // Xoá key rác khỏi dictionary
        foreach (var e in destroyedEnemies)
        {
            iconDict.Remove(e);
        }
    }
}
