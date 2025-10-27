// This is an example of what your "LevelExit.cs" script might look like
using UnityEngine;

public class LevelExit : MonoBehaviour
{
    public int nextLevelToUnlock = 2; // In the inspector, set this to 2 for Level 1, 3 for Level 2, etc.
    public int nextLevelToLoad = 2;   // The scene number to load

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. Unlock the next level in our save data
            GameProgress.UnlockLevel(nextLevelToUnlock);

            // 2. Load the next level
            GameProgress.LoadLevel(nextLevelToLoad);
        }
    }
}