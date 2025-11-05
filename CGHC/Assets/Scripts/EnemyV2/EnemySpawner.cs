using UnityEngine;
using System.Collections; 
using System.Collections.Generic; 

[System.Serializable]
public class SpawnInstruction
{
    public GameObject enemyPrefab;  
    public Transform spawnPoint;    
    public float spawnDelay;        
}

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
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

        // Draw a blue cube for  spawn point in list
        Gizmos.color = Color.blue;
        if (spawnList != null) 
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
                return; 
            }

            hasSpawned = true;

            StartCoroutine(SpawnWave());

            if (spawnOnce)
            {
                GetComponent<BoxCollider2D>().enabled = false;
            }
        }
    }

    private IEnumerator SpawnWave()
    {
        foreach (var instruction in spawnList)
        {
            // 1. Check if this instruction has a delay
            if (instruction.spawnDelay > 0)
            {
                yield return new WaitForSeconds(instruction.spawnDelay);
            }

            // 2. Check that the prefab and spawn point are valid
            if (instruction.enemyPrefab != null && instruction.spawnPoint != null)
            {
                // 3. Spawn enemy
                Instantiate(instruction.enemyPrefab, instruction.spawnPoint.position, instruction.spawnPoint.rotation);
            }
            else
            {
                Debug.LogWarning("A spawn instruction is missing a prefab or spawn point!");
            }
        }
    }
}