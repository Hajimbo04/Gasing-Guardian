// using UnityEngine;

// public class AttackIndicator : MonoBehaviour
// {
//     [Header("Attack Settings")]
//     public int attackDamage = 1;
//     public float attackDuration = 0.5f;

//     private PlayerMovement playerMovement;

//     public void Setup(Transform playerTransform, Vector3 offset, PlayerMovement movementScript)
//     {
//         playerMovement = movementScript;

//         Vector3 spawnPosition = playerTransform.position;
//         if (playerMovement._isFacingRight)
//         {
//             spawnPosition += offset;
//         }
//         else
//         {
//             spawnPosition -= offset;
//         }

//         transform.position = spawnPosition;

//         Invoke("DeactivateAttack", attackDuration);
//     }

//     private void OnTriggerEnter2D(Collider2D other)
//     {
//         // CHECK 1: Did the collision event fire at all?
//         Debug.Log("--- HIT REGISTERED on: " + other.gameObject.name + " ---");

//         // 1. Check if the object we hit is tagged "Enemy"
//         if (other.CompareTag("Enemy"))
//         {
//             // CHECK 2: Did the tag check succeed?
//             Debug.Log("Tag check SUCCESS: " + other.gameObject.name + " is tagged Enemy.");

//             // 2. Try to get the EnemyHealth component from the hit object
//             EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();

//             if (enemyHealth != null)
//             {
//                 // CHECK 3: Is the component on the enemy?
//                 Debug.Log("Component check SUCCESS: Applying " + attackDamage + " damage to " + other.gameObject.name);

//                 // 3. Apply damage
//                 enemyHealth.TakeDamage(attackDamage);
//             }
//             else
//             {
//                 Debug.LogError("FATAL ERROR: EnemyHealth component is missing on the object tagged 'Enemy'. Check the Enemy GameObject!");
//             }
//         }
//     }

//     private void DeactivateAttack()
//     {
//         Destroy(gameObject);
//     }
// }