// Sound.cs
using UnityEngine;

[System.Serializable]
public class Sound
{
    public enum SoundType { Music, SFX }
    public SoundType type;

    public string name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(0.1f, 3f)]
    public float pitch = 1f;

    [HideInInspector]
    public AudioSource source;
}