using UnityEngine;

public class Sound : MonoBehaviour
{
    [Tooltip("Clip sẽ phát khi nhấn chuột trái")]
    public AudioClip clickClip;

    [Tooltip("Volume từ 0 đến 1")]
    [Range(0f, 1f)]
    public float volume = 1f;

    AudioSource audioSource;

    void Awake()
    {
        // thêm AudioSource nếu chưa có
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 0 = 2D sound (không vị trí)
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 0 = left click
        {
            if (clickClip != null)
            {
                audioSource.PlayOneShot(clickClip, volume);
            }
        }
    }
}
