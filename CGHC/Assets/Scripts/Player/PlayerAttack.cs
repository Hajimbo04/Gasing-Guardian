using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject attackRangeIndicatorPrefab;
    public PlayerMovement playerMovement;

    public float indicatorDistance = 1.5f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SpawnAttackRangeIndicator();
        }
    }

    private void SpawnAttackRangeIndicator()
    {
        // Vector2 offset = new Vector3(indicatorDistance, 0f, 0f); // Use Vector3 for offset to avoid confusion
        Vector3 offset = new Vector3(indicatorDistance, 0f, 0f);

        GameObject indicatorGO = Instantiate(attackRangeIndicatorPrefab);

        AttackIndicator indicator = indicatorGO.GetComponent<AttackIndicator>();

        // If the component exists, set it up.
        if (indicator != null)
        {
            // This call passes the necessary information (position and direction)
            // The AttackIndicator script handles the rest (damage, duration, cleanup).
            indicator.Setup(transform, offset, playerMovement);
        }
        else
        {
            // This error is what you saw if the AttackIndicator script file/class name was wrong.
            Debug.LogError("AttackRangeIndicator script not found on the prefab!");
            Destroy(indicatorGO);
        }
    }
}