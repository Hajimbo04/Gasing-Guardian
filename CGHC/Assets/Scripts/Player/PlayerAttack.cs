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
        Vector2 offset = new Vector3(indicatorDistance, 0f, 0f);

        GameObject indicatorGO = Instantiate(attackRangeIndicatorPrefab);

        AttackIndicator indicator = indicatorGO.GetComponent<AttackIndicator>();

        // If the component exists, set it up.
        if (indicator != null)
        {
            indicator.Setup(transform, offset, playerMovement);
        }
        else
        {
            Debug.LogError("AttackRangeIndicator script not found on the prefab!");
            Destroy(indicatorGO);
        }
    }
}