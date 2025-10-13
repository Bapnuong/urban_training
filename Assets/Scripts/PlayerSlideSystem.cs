using UnityEngine;
using System.Collections;

public class PlayerSlideSystem : MonoBehaviour
{
    [Header("References")]
    private CharacterController controller;
    private Animator animator;
    public Transform orientation; // hướng của player
    public Camera playerCamera;

    [Header("Slide Settings")]
    public float slideSpeed = 16f;          // Tốc độ slide
    public float slideTime = 0.8f;          // Thời gian slide tối đa
    public float slideCooldown = 1f;        // Cooldown giữa các lần slide
    public float minSpeedToSlide = 6f;      // Tốc độ tối thiểu để slide

    [Header("Height Settings")]
    public float standHeight = 2f;
    public float slideHeight = 1f;
    public float heightTransitionSpeed = 10f;

    [Header("Camera Effects")]
    public float standCameraY = 0.6f;
    public float slideCameraY = 0.2f;
    public float slideFOV = 85f;
    public float normalFOV = 75f;
    public float fovSpeed = 10f;

    [Header("Controls")]
    public KeyCode slideKey = KeyCode.C;

    // Private variables
    private PlayerMovement playerMovement;
    private bool isSliding = false;
    private float slideTimer = 0f;
    private float slideCooldownTimer = 0f;
    private Vector3 slideDirection;
    private float targetCameraY;
    private float targetFOV;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();

        if (orientation == null)
            orientation = transform;

        if (playerCamera == null)
            playerCamera = Camera.main;

        targetCameraY = standCameraY;
        targetFOV = normalFOV;
    }

    void Update()
    {
        HandleSlideInput();
        HandleSlide();
        UpdateCamera();
        UpdateCooldown();
    }

    void HandleSlideInput()
    {
        // Nhấn C để slide (khi đang chạy)
        if (Input.GetKeyDown(slideKey) && !isSliding && slideCooldownTimer <= 0)
        {
            TryStartSlide();
        }

        // Nhảy để thoát slide
        if (isSliding && Input.GetButtonDown("Jump"))
        {
            StopSlide();
        }
    }

    void TryStartSlide()
    {
        // Kiểm tra có đang chạy không
        float currentSpeed = new Vector3(controller.velocity.x, 0, controller.velocity.z).magnitude;

        if (currentSpeed < minSpeedToSlide)
        {
            Debug.Log("Chạy nhanh hơn để slide!");
            return;
        }

        // Kiểm tra đang ở trên mặt đất
        if (!IsGrounded())
        {
            Debug.Log("Phải ở trên mặt đất để slide!");
            return;
        }

        StartSlide();
    }

    void StartSlide()
    {
        isSliding = true;
        slideTimer = 0f;

        // Lưu hướng slide
        slideDirection = orientation.forward;

        // Giảm chiều cao
        controller.height = slideHeight;

        // Camera
        targetCameraY = slideCameraY;
        targetFOV = slideFOV;

        // Animation
        if (animator != null)
            animator.SetBool("IsSliding", true);

        // Disable PlayerMovement script tạm thời
        if (playerMovement != null)
            playerMovement.enabled = false;

        Debug.Log("🏃 SLIDE!");

        // Sound
        if (SoundManager.Instance != null)
        {
            // SoundManager.Instance.PlaySlideSound();
        }
    }

    void HandleSlide()
    {
        if (!isSliding)
            return;

        slideTimer += Time.deltaTime;

        // Input điều khiển nhẹ khi slide
        float h = Input.GetAxis("Horizontal");
        Vector3 sideControl = orientation.right * h * 0.3f;

        // Tính tốc độ slide giảm dần
        float speedMultiplier = 1f - (slideTimer / slideTime);
        speedMultiplier = Mathf.Clamp(speedMultiplier, 0.5f, 1f);

        float currentSlideSpeed = slideSpeed * speedMultiplier;

        // Di chuyển
        Vector3 slideMove = (slideDirection + sideControl).normalized * currentSlideSpeed;
        controller.Move(slideMove * Time.deltaTime);

        // Gravity
        controller.Move(Vector3.up * playerMovement.gravity * Time.deltaTime);

        // Kết thúc slide
        if (slideTimer >= slideTime || !IsGrounded())
        {
            StopSlide();
        }
    }

    void StopSlide()
    {
        isSliding = false;
        slideCooldownTimer = slideCooldown;

        // Đứng lên
        controller.height = standHeight;

        // Camera
        targetCameraY = standCameraY;
        targetFOV = normalFOV;

        // Animation
        if (animator != null)
            animator.SetBool("IsSliding", false);

        // Enable lại PlayerMovement
        if (playerMovement != null)
            playerMovement.enabled = true;

        Debug.Log("🛑 Slide end");
    }

    void UpdateCamera()
    {
        if (playerCamera == null)
            return;

        // Smooth camera Y position
        Vector3 localPos = playerCamera.transform.localPosition;
        localPos.y = Mathf.Lerp(localPos.y, targetCameraY, Time.deltaTime * heightTransitionSpeed);
        playerCamera.transform.localPosition = localPos;

        // Smooth FOV
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * fovSpeed);

        // Camera tilt khi slide (optional)
        if (isSliding)
        {
            float tiltAngle = -5f;
            playerCamera.transform.localRotation = Quaternion.Lerp(
                playerCamera.transform.localRotation,
                Quaternion.Euler(tiltAngle, 0, 0),
                Time.deltaTime * 5f
            );
        }
        else
        {
            playerCamera.transform.localRotation = Quaternion.Lerp(
                playerCamera.transform.localRotation,
                Quaternion.identity,
                Time.deltaTime * 10f
            );
        }
    }

    void UpdateCooldown()
    {
        if (slideCooldownTimer > 0)
        {
            slideCooldownTimer -= Time.deltaTime;
        }
    }

    bool IsGrounded()
    {
        // Dùng groundCheck từ PlayerMovement
        if (playerMovement != null && playerMovement.groundCheck != null)
        {
            return Physics.CheckSphere(
                playerMovement.groundCheck.position,
                playerMovement.groundDistance,
                playerMovement.groundMask
            );
        }
        return false;
    }

    // Public getters
    public bool IsSliding() => isSliding;
    public float GetSlideCooldown() => slideCooldownTimer;

    void OnGUI()
    {
        // Debug UI
        if (isSliding)
        {
            GUI.Label(new Rect(10, 10, 200, 30), "SLIDING!", new GUIStyle()
            {
                fontSize = 20,
                normal = new GUIStyleState() { textColor = Color.yellow }
            });
        }

        if (slideCooldownTimer > 0)
        {
            GUI.Label(new Rect(10, 40, 200, 30),
                $"Cooldown: {slideCooldownTimer:F1}s",
                new GUIStyle()
                {
                    fontSize = 16,
                    normal = new GUIStyleState() { textColor = Color.white }
                }
            );
        }
    }
}