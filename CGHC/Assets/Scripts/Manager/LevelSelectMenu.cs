using UnityEngine;
using UnityEngine.UI; 
using System.Collections.Generic; 

public class LevelSelectMenu : MonoBehaviour
{
    public List<Button> levelButtons;

    void OnEnable()
    {
        RefreshButtons();
    }

    public void RefreshButtons()
    {
        int highestLevel = GameProgress.GetHighestLevelUnlocked();

        for (int i = 0; i < levelButtons.Count; i++)
        {
            int buttonLevelNumber = i + 1; 

            if (buttonLevelNumber <= highestLevel)
            {
                levelButtons[i].interactable = true;
            }
            else
            {
                levelButtons[i].interactable = false;
            }
        }
    }

    public void OnLevelButtonClicked(int levelNumber)
    {
        // Load the level that was clicked
        GameProgress.LoadLevel(levelNumber);
    }
}