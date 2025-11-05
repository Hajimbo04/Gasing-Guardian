using UnityEngine;
using System.Collections.Generic; 

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0f, 2f)]
    public float volume = 1.0f;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [Tooltip("The AudioSource component for playing music.")]
    public AudioSource musicSource;
    [Tooltip("The AudioSource component for playing sound effects.")]
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    [Tooltip("The background music you want to play.")]
    public AudioClip backgroundMusic;

    [Range(0f, 1f)]
    public float musicVolume = 0.7f;

    [Tooltip("The list of all sound effects for the game.")]
    public Sound[] sfxList;

    private Dictionary<string, Sound> sfxMap;

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
            return;
        }

        sfxMap = new Dictionary<string, Sound>();
        foreach (Sound s in sfxList)
        {
            sfxMap[s.name] = s;
        }

        // --- Setup and Play Music ---
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }
    }

    /// <summary>
    /// Plays a sound effect one time.
    /// </summary>
    /// <param name="soundName">The name of the sound from the Sfx List.</param>
    public void PlaySFX(string soundName)
    {
        if (sfxMap.TryGetValue(soundName, out Sound soundToPlay))
        {
            sfxSource.PlayOneShot(soundToPlay.clip, soundToPlay.volume);
        }
        else
        {
            Debug.LogWarning("AudioManager: SFX not found: " + soundName);
        }
    }
}