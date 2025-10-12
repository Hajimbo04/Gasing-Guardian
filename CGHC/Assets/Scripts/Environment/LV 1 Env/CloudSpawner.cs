using System.Collections;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    [Header("Cloud Prefabs")]
    [Tooltip("The different cloud prefabs to be spawned. One will be chosen randomly each time.")]
    public GameObject[] cloudPrefabs; // This is now an array to hold multiple prefabs

    [Header("Spawning Area")]
    [Tooltip("The x-coordinate where clouds will spawn.")]
    public float spawnXPosition = 15f;
    [Tooltip("The minimum y-coordinate for spawning.")]
    public float minSpawnY = -2f;
    [Tooltip("The maximum y-coordinate for spawning.")]
    public float maxSpawnY = 5f;

    [Header("Spawning Timing")]
    [Tooltip("The minimum delay between cloud spawns in seconds.")]
    public float minSpawnDelay = 1f;
    [Tooltip("The maximum delay between cloud spawns in seconds.")]
    public float maxSpawnDelay = 4f;

    [Header("Cloud Properties")]
    [Tooltip("The minimum speed for a spawned cloud.")]
    public float minCloudSpeed = 0.5f;
    [Tooltip("The maximum speed for a spawned cloud.")]
    public float maxCloudSpeed = 2f;
    [Tooltip("The x-coordinate where clouds will be destroyed.")]
    public float destroyXPosition = -15f;


    void Start()
    {
        // Start the coroutine that handles spawning clouds.
        StartCoroutine(SpawnCloudsRoutine());
    }

    private IEnumerator SpawnCloudsRoutine()
    {
        // This loop will run indefinitely.
        while (true)
        {
            // Wait for a random amount of time before spawning the next cloud.
            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);

            // Make sure the prefab array isn't empty to avoid errors.
            if (cloudPrefabs.Length == 0)
            {
                Debug.LogWarning("No cloud prefabs assigned to the CloudSpawner.");
                continue; // Skip this spawn cycle
            }

            // Determine a random spawn position.
            float randomY = Random.Range(minSpawnY, maxSpawnY);
            Vector3 spawnPosition = new Vector3(spawnXPosition, randomY, transform.position.z);

            // --- This is the main change ---
            // Pick a random cloud prefab from the array.
            int randomIndex = Random.Range(0, cloudPrefabs.Length);
            GameObject chosenCloudPrefab = cloudPrefabs[randomIndex];
            // --------------------------------

            // Instantiate the chosen cloud prefab at the calculated position.
            GameObject newCloud = Instantiate(chosenCloudPrefab, spawnPosition, Quaternion.identity);

            // Access the CloudController script on the newly spawned cloud.
            CloudController cloudController = newCloud.GetComponent<CloudController>();

            if (cloudController != null)
            {
                // Assign a random speed and the destruction point to the new cloud.
                cloudController.moveSpeed = Random.Range(minCloudSpeed, maxCloudSpeed);
                cloudController.destroyXPosition = destroyXPosition;
            }
            else
            {
                Debug.LogError("The cloud prefab '" + chosenCloudPrefab.name + "' does not have a CloudController script attached!");
            }
        }
    }
}

