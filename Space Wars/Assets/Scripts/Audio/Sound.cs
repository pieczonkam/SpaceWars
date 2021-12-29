using UnityEngine;

[System.Serializable]
public class Sound
{
    [HideInInspector] public AudioSource audioSource;

    public string soundName;
    public AudioClip clip;
    [Range(0.0f, 1.0f)] public float volume;
    [Range(0.0f, 3.0f)] public float pitch;
    public bool loop;
}
