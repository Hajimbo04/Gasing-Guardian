using UnityEngine;

public class LevelExit : MonoBehaviour
{
    public int nextLevelToUnlock = 2; 
    public int nextLevelToLoad = 2;   

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