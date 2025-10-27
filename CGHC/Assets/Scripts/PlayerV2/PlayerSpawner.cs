using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Prefab to Spawn")]
    public GameObject playerPrefab; // Drag your Player prefab from your Project folder here

    // Awake is called before Start, and before OnSceneLoaded
    void Awake()
    {
        // Check if the Player (which IS the HealthSystem.Instance) already exists
        if (HealthSystem.Instance == null)
        {
            // Player doesn't exist, so we must spawn one.
            Debug.Log("Player not found. Spawning player...");
            
            // We can spawn it at (0,0) because your HealthSystem's
            // OnSceneLoaded function will move it to the respawn point immediately.
            Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.Log("Player already exists. Spawner will not run.");
        }
    }
}