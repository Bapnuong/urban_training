using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private Animator animator;
    private AudioSource footstepSource;

    public float speed = 12f;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private Vector3 velocity;
    private bool isGrounded;
    private bool isMoving;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // Tạo AudioSource riêng cho bước chân
        footstepSource = gameObject.AddComponent<AudioSource>();
        footstepSource.loop = true; // Lặp âm thanh bước chân

        // Nếu SoundManager có sẵn, lấy clip bước chân (ví dụ soundList[1])
        if (SoundManager.Instance != null && SoundManager.Instance.soundList.Count > 1)
        {
            footstepSource.clip = SoundManager.Instance.soundList[1];
        }
    }

    void Update()
    {
        // check ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // movement input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        // jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump");

            // âm thanh nhảy
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySound(2);
        }

        // gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // check running
        isMoving = (x != 0 || z != 0) && isGrounded;

        // animator parameters
        animator.SetBool("isRunning", isMoving);
        animator.SetBool("isGrounded", isGrounded);

        // xử lý âm bước chân
        HandleFootstepSound();
    }

    void HandleFootstepSound()
    {
        if (isMoving && isGrounded)
        {
            if (!footstepSource.isPlaying && footstepSource.clip != null)
                footstepSource.Play();
        }
        else
        {
            if (footstepSource.isPlaying)
                footstepSource.Stop();
        }
    }
}
