using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Danh sách âm thanh")]
    public List<AudioClip> soundList = new List<AudioClip>();  // Thêm âm vào đây trong Inspector
    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    // Gọi âm bằng chỉ số
    public void PlaySound(int index)
    {
        if (index >= 0 && index < soundList.Count)
        {
            audioSource.PlayOneShot(soundList[index]);
        }
        else
        {
            Debug.LogWarning("Chỉ số âm không hợp lệ: " + index);
        }
    }

    // Các hàm tiện nhanh
    public void PlaySound0() => PlaySound(0);
    public void PlaySound1() => PlaySound(1);
    public void PlaySound2() => PlaySound(2);
    public void PlaySound3() => PlaySound(3);
}
