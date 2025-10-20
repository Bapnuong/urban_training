using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CapturePoint : MonoBehaviour
{
    [Header("Capture Settings")]
    public float captureTime = 100f; // Thời gian cần để chiếm 100%
    public float captureProgress = 0f;
    public bool isCaptured = false;

    [Header("Ownership")]
    public string currentOwner = "None";
    public string capturingTeam = "";

    [Header("Visual Feedback")]
    public Renderer zoneRenderer;
    public Color neutralColor = Color.gray;
    public Color playerColor = Color.blue;

    [Header("UI hiển thị % chiếm")]
    public Text captureUIText;
    public static bool playerInside = false;
    private int playerCount = 0;

    void Start()
    {
        if (zoneRenderer == null)
            zoneRenderer = GetComponent<Renderer>();

        zoneRenderer.material.color = neutralColor;

        if (captureUIText != null)
            captureUIText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isCaptured) return;

        // Nếu có Player trong vùng → chiếm
        if (playerCount > 0)
        {
            CaptureProgress();
        }
        else
        {
            // Không ai trong vùng → giảm dần tiến độ
            if (captureProgress > 0f)
            {
                captureProgress = Mathf.Max(0, captureProgress - Time.deltaTime);
                zoneRenderer.material.color = Color.Lerp(neutralColor, playerColor, captureProgress);
            }
        }

        // Cập nhật UI
        if (playerInside && captureUIText != null && !isCaptured)
            captureUIText.text = $"Chiếm cứ điểm: {(captureProgress * 100f):0}%";
    }

    private void CaptureProgress()
    {
        if (capturingTeam != "PlayerTeam")
        {
            capturingTeam = "PlayerTeam";
            captureProgress = 0f;
        }

        captureProgress += Time.deltaTime / captureTime;
        zoneRenderer.material.color = Color.Lerp(neutralColor, playerColor, captureProgress);

        if (captureProgress >= 1f)
        {
            isCaptured = true;
            currentOwner = capturingTeam;
            zoneRenderer.material.color = playerColor;

            if (captureUIText != null)
                captureUIText.text = $"Cứ điểm đã bị chiếm bởi {currentOwner}!";

            SceneManager.LoadScene("finish");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCount++;
            playerInside = true;
            if (captureUIText != null)
                captureUIText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCount = Mathf.Max(0, playerCount - 1);
            playerInside = false;
            if (captureUIText != null)
                captureUIText.gameObject.SetActive(false);
        }
    }
}
