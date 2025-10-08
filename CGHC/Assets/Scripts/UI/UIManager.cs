using UnityEngine;
using TMPro; // REQUIRED for TextMeshPro

/// <summary>
/// Manages the display of player lives.
/// This script is designed to be called by the PlayerHealth component's UnityEvent.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Drag your TextMeshProUGUI element for lives here.")]
    public TextMeshProUGUI LivesText;

    // We no longer need the private PlayerHealth reference!

    void Start()
    {
        Debug.Log("UIManager Start called");

        if (LivesText == null)
        {
            // We keep this check since the text reference is essential
            Debug.LogError("LivesText (TextMeshProUGUI) is not assigned in the UIManager Inspector! UI will not update.");
        }

        // Note: The initial display update is now handled by PlayerHealth.cs in its Start() method
        // via the OnLivesChanged event, ensuring the value is correct even after persistence loading.
    }

    // Public method called by PlayerHealth whenever lives change
    // This function will be linked via the Unity Inspector.
    public void UpdateLivesDisplay(int newLives)
    {
        if (LivesText != null)
        {
            LivesText.text = "Lives: " + newLives;
            Debug.Log($"UI updated: Lives: {newLives}");
        }
    }
}
