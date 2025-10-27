using UnityEngine;
using UnityEngine.UI; // Required for 'Button'
using System.Collections.Generic; // Required for 'List'

public class LevelSelectMenu : MonoBehaviour
{
    // In the Inspector, drag all 8 of your level buttons into this list
    public List<Button> levelButtons;

    void OnEnable()
    {
        // When this panel is turned on, refresh the buttons
        RefreshButtons();
    }

    public void RefreshButtons()
    {
        // Get the highest level unlocked from our saved data
        int highestLevel = GameProgress.GetHighestLevelUnlocked();

        // Loop through all the buttons in our list
        // (This assumes your buttons are in order: Button 1, Button 2, etc.)
        for (int i = 0; i < levelButtons.Count; i++)
        {
            int buttonLevelNumber = i + 1; // Button 0 is Level 1, Button 1 is Level 2

            if (buttonLevelNumber <= highestLevel)
            {
                // This level is unlocked! Make the button clickable.
                levelButtons[i].interactable = true;
            }
            else
            {
                // This level is still locked. Gray it out.
                levelButtons[i].interactable = false;
            }
        }
    }

    // A single function you'll hook up to ALL your level buttons
    public void OnLevelButtonClicked(int levelNumber)
    {
        // Load the level that was clicked
        GameProgress.LoadLevel(levelNumber);
    }
}