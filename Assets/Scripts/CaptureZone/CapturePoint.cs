using UnityEngine;
using UnityEngine.UI;

public class CapturePoint : MonoBehaviour
{
    [Header("Capture Settings")]
    public float captureTime = 100f; // Thời gian cần để chiếm 100%
    public float captureProgress = 0f;
    public bool isCaptured = false;

    [Header("Teams")]
    public string currentOwner = "None";
    public string capturingTeam = "";

    [Header("Visual Feedback")]
    public Renderer zoneRenderer;
    public Color neutralColor = Color.gray;
    public Color teamAColor = Color.blue;
    public Color teamBColor = Color.red;
    public Color contestedColor = Color.yellow;

    [Header("UI hiển thị % chiếm")]
    public Text captureUIText; // <— gán Text ở đây
    private bool playerInside = false;

    // Biến đếm số lượng người chơi trong vùng của từng đội
    public int teamA_Count = 0;
    public int teamB_Count = 0;

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
        if (isCaptured) return; // Nếu đã chiếm xong thì không làm gì nữa

        // Nếu cả hai đội cùng trong vùng → dừng chiếm
        if (teamA_Count > 0 && teamB_Count > 0)
        {
            zoneRenderer.material.color = contestedColor;
            return;
        }

        // Nếu chỉ có 1 đội trong vùng → bắt đầu chiếm
        if (teamA_Count > 0 && teamB_Count == 0)
        {
            CaptureProgress("PlayerTeam");
        }
        else if (teamB_Count > 0 && teamA_Count == 0)
        {
            CaptureProgress("EnemyTeam");
        }
        else
        {
            // Không ai trong vùng → reset về trung lập (nếu muốn)
            if (capturingTeam != "")
            {
                captureProgress = Mathf.Max(0, captureProgress - Time.deltaTime); // giảm dần
            }
        }
        if (playerInside && captureUIText != null && !isCaptured)
            captureUIText.text = $"Chiếm cứ điểm: {(captureProgress * 100f):0}%";
    }

    private void CaptureProgress(string team)
    {
        if (capturingTeam != team)
        {
            capturingTeam = team;
            captureProgress = 0f;
        }

        captureProgress += Time.deltaTime / captureTime;

        if (capturingTeam == "PlayerTeam")
            zoneRenderer.material.color = Color.Lerp(neutralColor, teamAColor, captureProgress);
        else
            zoneRenderer.material.color = Color.Lerp(neutralColor, teamBColor, captureProgress);

        if (captureProgress >= 1f)
        {
            isCaptured = true;
            currentOwner = capturingTeam;
            zoneRenderer.material.color = currentOwner == "PlayerTeam" ? teamAColor : teamBColor;


            if (captureUIText != null)
                captureUIText.text = "Cứ điểm đã bị chiếm!";
            Debug.Log($"Cứ điểm đã bị chiếm bởi {currentOwner}!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            teamA_Count++;
            playerInside = true;
            if (captureUIText != null)
                captureUIText.gameObject.SetActive(true);
        }
        else if (other.CompareTag("Enemy"))
            teamB_Count++;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            teamA_Count = Mathf.Max(0, teamA_Count - 1);
            playerInside = false;
            if (captureUIText != null)
                captureUIText.gameObject.SetActive(false);
        }
        else if (other.CompareTag("Enemy"))
            teamB_Count = Mathf.Max(0, teamB_Count - 1);
    }
}