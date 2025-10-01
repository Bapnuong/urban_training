using UnityEngine;
using UnityEngine.AI;

public class EnemyFollow3D : MonoBehaviour
{
    public float detectionRange = 15f; // Tầm phát hiện player
    public float moveSpeed = 3.5f;     // Tốc độ di chuyển bot

    private NavMeshAgent agent;
    private Transform player;

    void Start()
    {
        // Lấy NavMeshAgent từ Enemy
        agent = GetComponent<NavMeshAgent>();

        // Set tốc độ cho agent
        agent.speed = moveSpeed;

        // Tìm Player bằng tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        // Tính khoảng cách đến player
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance < detectionRange)
        {
            // Bot đuổi theo player
            agent.SetDestination(player.position);
        }
        else
        {
            // Nếu player ra khỏi tầm thì bot đứng yên
            agent.ResetPath();
        }
    }
}
