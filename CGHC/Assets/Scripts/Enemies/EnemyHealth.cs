using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        // CHECK 4: Confirm damage was received by this script
        Debug.Log(gameObject.name + " received " + damageAmount + " damage. Current Health (BEFORE): " + currentHealth);

        currentHealth -= damageAmount;

        // CHECK 5: Show new health state
        Debug.Log(gameObject.name + " Current Health (AFTER): " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // CHECK 6: Confirm death sequence started
        Debug.Log("!!! DEFEATED !!! Starting destroy sequence for " + gameObject.name);

        Destroy(gameObject);
    }
}