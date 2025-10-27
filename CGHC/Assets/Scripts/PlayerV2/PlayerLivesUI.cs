using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerLivesUI : MonoBehaviour
{
    [Header("References")]
    private HealthSystem playerHealth; 

    public List<GameObject> lifeIcons;

    void Start()
    {
        playerHealth = HealthSystem.Instance; 

        if (playerHealth == null)
        {
            Debug.LogError("Player Health Instance not found in PlayerLivesUI!");
            return;
        }

        playerHealth.OnLivesChanged += UpdateLivesUI;

        UpdateLivesUI(playerHealth.CurrentLives);
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnLivesChanged -= UpdateLivesUI;
        }
    }

    private void UpdateLivesUI(int currentLives)
    {
        for (int i = 0; i < lifeIcons.Count; i++)
        {
            if (i < currentLives)
            {
                lifeIcons[i].SetActive(true);
            }
            else
            {
                lifeIcons[i].SetActive(false);
            }
        }
    }
}