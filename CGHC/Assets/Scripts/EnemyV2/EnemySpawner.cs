using UnityEngine;
using System.Collections; // Required for Coroutines
using System.Collections.Generic; // Required for Lists

// This is a small helper class. By marking it [System.Serializable],
// we can see and edit it in the Unity Inspector.
[System.Serializable]
public class SpawnInstruction
{
    public GameObject enemyPrefab;  // The enemy (Zombie, Skeleton, etc.)
    public Transform spawnPoint;    // The location to spawn at
    public float spawnDelay;        // Delay (in seconds) *before* this enemy spawns
}

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    // We replace the single prefab/spawn point with a LIST of instructions
    public List<SpawnInstruction> spawnList;
    public bool spawnOnce = true;

    private bool hasSpawned = false;

    private void OnDrawGizmos()
    {
        // Draw a red wireframe for the trigger area
        Gizmos.color = Color.red;
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            Gizmos.DrawWireCube(transform.position + (Vector3)collider.offset, collider.size);
        }

        // Draw a blue cube for EVERY spawn point in our list
        Gizmos.color = Color.blue;
        if (spawnList != null) // Check if the list exists
        {
            foreach (var instruction in spawnList)
            {
                if (instruction.spawnPoint != null)
                {
                    Gizmos.DrawWireCube(instruction.spawnPoint.position, Vector3.one);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (spawnOnce && hasSpawned)
            {
                return; // We've already spawned, so do nothing
            }

            // Mark as spawned immediately to prevent re-triggering
            hasSpawned = true;

            // Start the Coroutine to handle the wave
            StartCoroutine(SpawnWave());

            // Disable the trigger if it's one-time-use
            if (spawnOnce)
            {
                GetComponent<BoxCollider2D>().enabled = false;
            }
        }
    }

    // This is the new Coroutine that spawns the wave
    private IEnumerator SpawnWave()
    {
        // Loop through every instruction in our list
        foreach (var instruction in spawnList)
        {
            // 1. Check if this instruction has a delay
            if (instruction.spawnDelay > 0)
            {
                // If yes, wait for that many seconds
                yield return new WaitForSeconds(instruction.spawnDelay);
            }

            // 2. Check that the prefab and spawn point are valid
            if (instruction.enemyPrefab != null && instruction.spawnPoint != null)
            {
                // 3. Spawn the enemy!
                Instantiate(instruction.enemyPrefab, instruction.spawnPoint.position, instruction.spawnPoint.rotation);
            }
            else
            {
                Debug.LogWarning("A spawn instruction is missing a prefab or spawn point!");
            }
        }
        
        // The Coroutine ends when the loop is finished
    }
}