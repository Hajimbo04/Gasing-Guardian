using UnityEngine;
using TMPro; // REQUIRED for TextMeshPro

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Drag your TextMeshProUGUI element for lives here.")]
    public TextMeshProUGUI LivesText;

    private PlayerHealth playerHealth;

    void Start()
    {
        // 1. Find the PlayerHealth script automatically.
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerHealth = playerObject.GetComponent<PlayerHealth>();
        }

        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth component not found on the GameObject tagged 'Player'. UI will not update.");
        }

        if (LivesText == null)
        {
            Debug.LogError("LivesText (TextMeshProUGUI) is not assigned in the UIManager Inspector!");
        }

        // 2. If components are found, set the initial text.
        if (LivesText != null && playerHealth != null)
        {
            UpdateLivesDisplay(playerHealth.maxLives);
        }
    }

    // Public method called by PlayerHealth whenever lives change
    public void UpdateLivesDisplay(int newLives)
    {
        if (LivesText != null)
        {
            LivesText.text = "Lives: " + newLives;
        }
    }
}