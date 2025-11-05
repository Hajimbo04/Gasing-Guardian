using UnityEngine;
using TMPro; 

/// <summary>
/// Manages the display of player lives.
/// This script is designed to be called by the PlayerHealth component's UnityEvent.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Drag your TextMeshProUGUI element for lives here.")]
    public TextMeshProUGUI LivesText;

    void Start()
    {
        Debug.Log("UIManager Start called");

        if (LivesText == null)
        {
            Debug.LogError("LivesText (TextMeshProUGUI) is not assigned in the UIManager Inspector! UI will not update.");
        }

    }

    public void UpdateLivesDisplay(int newLives)
    {
        if (LivesText != null)
        {
            LivesText.text = "Lives: " + newLives;
            Debug.Log($"UI updated: Lives: {newLives}");
        }
    }
}
