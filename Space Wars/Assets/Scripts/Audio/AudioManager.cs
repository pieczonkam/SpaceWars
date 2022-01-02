using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = null;

    private string currentTheme;
    private bool themeStopped = false;

    [SerializeField] private AudioMixerGroup audioMixerGroup;
    [SerializeField] private Sound[] sounds;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound sound in sounds)
        {
            sound.audioSource = gameObject.AddComponent<AudioSource>();
            sound.audioSource.clip = sound.clip;
            sound.audioSource.volume = sound.volume;
            sound.audioSource.pitch = sound.pitch;
            sound.audioSource.loop = sound.loop;
            sound.audioSource.playOnAwake = false;
            sound.audioSource.outputAudioMixerGroup = audioMixerGroup;
        }
    }

    public void Play(string soundName)
    {
        Sound sound = Array.Find(sounds, s => s.soundName == soundName);
        if (sound == null)
        {
            Debug.LogWarning("Sound of given name does not exist");
            return;
        }
        sound.audioSource.Play();
    }

    public void PlayTheme(string themeName)
    {
        themeStopped = false;
        currentTheme = themeName;
        foreach (Sound sound in sounds)
        {
            if (sound.soundName.Contains("Theme"))
                sound.audioSource.Stop();
            if (sound.soundName == themeName)
                sound.audioSource.Play();
        }
    }

    public void StopTheme()
    {
        if (!themeStopped)
        {
            themeStopped = true;
            Sound sound = Array.Find(sounds, s => s.soundName == currentTheme);
            if (sound == null)
            {
                Debug.LogWarning("Theme of given name does not exist");
                return;
            }
            sound.audioSource.Stop();
        }
    }

    public void PauseTheme()
    {
        Sound sound = Array.Find(sounds, s => s.soundName == currentTheme);
        if (sound == null)
        {
            Debug.LogWarning("Theme of given name does not exist");
            return;
        }
        sound.audioSource.Pause();
    }

    public void ResumeTheme()
    {
        Sound sound = Array.Find(sounds, s => s.soundName == currentTheme);
        if (sound == null)
        {
            Debug.LogWarning("Theme of given name does not exist");
            return;
        }
        sound.audioSource.UnPause();
    }
}
