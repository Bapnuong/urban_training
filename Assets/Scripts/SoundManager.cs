// SoundManager.cs
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private Sound[] sounds;

    private Dictionary<string, Sound> soundDictionary;

    private void Awake()
    {
        // Singleton Pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);

        soundDictionary = new Dictionary<string, Sound>();

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;

            switch (s.type)
            {
                case Sound.SoundType.Music:
                    s.source.outputAudioMixerGroup = mainMixer.FindMatchingGroups("Music")[0];
                    break;
                case Sound.SoundType.SFX:
                    s.source.outputAudioMixerGroup = mainMixer.FindMatchingGroups("SFX")[0];
                    break;
            }

            soundDictionary[s.name] = s;
        }
    }

    /// <summary>
    /// </summary>
    public void PlaySound(string soundName)
    {
        if (soundDictionary.TryGetValue(soundName, out Sound sound))
        {
            sound.source.PlayOneShot(sound.clip);
        }
        else
        {
            Debug.LogWarning($"Sound with name '{soundName}' not found!");
        }
    }

    /// <summary>
    /// </summary>
    public void PlayMusic(string musicName)
    {
        if (soundDictionary.TryGetValue(musicName, out Sound sound))
        {
            StopAllMusic();

            sound.source.loop = true;
            sound.source.Play();
        }
        else
        {
            Debug.LogWarning($"Music with name '{musicName}' not found!");
        }
    }

    public void StopAllMusic()
    {
        foreach (var sound in soundDictionary.Values)
        {
            if (sound.source.loop && sound.source.isPlaying)
            {
                sound.source.Stop();
            }
        }
    }
    public void SetMusicVolume(float volume)
    {
        mainMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        mainMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }
}