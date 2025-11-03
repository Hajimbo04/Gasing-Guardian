using UnityEngine;

// This line automatically adds an AudioSource when you add this script
[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    // This 'Instance' part is the "Singleton" pattern.
    // It lets us keep track of the one and only AudioManager.
    public static AudioManager Instance { get; private set; }

    private AudioSource musicSource;

    [Header("Music Clip")]
    [Tooltip("The background music you want to play.")]
    public AudioClip backgroundMusic;

    void Awake()
    {
        // --- This is the core logic ---
        
        if (Instance == null)
        {
            // If this is the FIRST time, set this object as the Instance
            Instance = this;
            
            // Tell Unity: "Don't destroy this GameObject when a new scene loads."
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If an Instance ALREADY exists (from a previous scene),
            // then destroy this new, duplicate object.
            Destroy(gameObject);
            return; // Stop running the rest of Awake()
        }
        
        // --- Setup the AudioSource ---
        musicSource = GetComponent<AudioSource>();
        musicSource.clip = backgroundMusic;
        musicSource.loop = true; // Make sure the music loops
        
        // Optional: If you want 2D music (not 3D positional)
        musicSource.spatialBlend = 0f; 

        // Play the music
        if (backgroundMusic != null)
        {
            musicSource.Play();
        }
    }
}