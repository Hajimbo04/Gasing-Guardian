using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Prefab to Spawn")]
    public GameObject playerPrefab; 

    void Awake()
    {
        if (HealthSystem.Instance == null)
        {
            Debug.Log("Player not found. Spawning player...");
            Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.Log("Player already exists. Spawner will not run.");
        }
    }
}