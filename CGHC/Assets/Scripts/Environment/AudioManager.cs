using UnityEngine;
using System.Collections.Generic; // We need this for the Dictionary

// This struct is a helper. It lets us create a nice-looking list
// of named sound effects in the Inspector.
[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0f, 2f)] // This makes a nice slider
    public float volume = 1.0f; // Default to full volume
}

public class AudioManager : MonoBehaviour
{
    // This 'Instance' part is the "Singleton" pattern.
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

    // A Dictionary is a high-speed way to look up clips by their name.
    private Dictionary<string, Sound> sfxMap;

    void Awake()
    {
        // --- Singleton Logic ---
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

        // --- Build the SFX Dictionary ---
        // We turn the list into a fast-lookup Dictionary
        // so we can call sounds by their name string.
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
        // Find the clip from the dictionary
        if (sfxMap.TryGetValue(soundName, out Sound soundToPlay))
        {
            // Play it as a "one-shot" sound. This allows multiple
            // sounds to play at once without cutting each other off.
            sfxSource.PlayOneShot(soundToPlay.clip, soundToPlay.volume);
        }
        else
        {
            // A small warning if you try to play a sound that doesn't exist
            Debug.LogWarning("AudioManager: SFX not found: " + soundName);
        }
    }
}