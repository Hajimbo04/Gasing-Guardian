using UnityEngine;
using System.Collections.Generic; // Needed for the Gasing List

public enum GasingType 
{ 
    Angin, 
    Api,   
    Tanah 
}

public class Player_Gasing_Shoot : MonoBehaviour
{
    [Header("Gasing Prefabs")]
    public GameObject gasingAnginPrefab;
    public GameObject gasingApiPrefab;
    public GameObject gasingTanahPrefab;

    [Header("Gasing Settings")]
    public Transform firePoint;     
    public float fireRate = 2f; 

    [Header("Gasing Type: API (Spread)")]
    public int apiProjectileCount = 3; 
    public float apiSpreadAmount = 0.3f; 

    // Private variables
    private float nextFireTime = 0f;
    private bool isFacingRight = true;
    private GasingType currentGasingType = GasingType.Angin;

    private SpriteRenderer playerSpriteRenderer;

    void Awake()
    {
        playerSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (playerSpriteRenderer == null)
        {
            Debug.LogError("Player_Gasing_Shoot cannot find the player's SpriteRenderer!", this);
        }
    }

    void Update()
    {
        if (transform.localScale.x > 0) isFacingRight = true;
        else if (transform.localScale.x < 0) isFacingRight = false;

        if (Input.GetKeyDown(KeyCode.T))
        {
            CycleGasingType();
        }

        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;
            Shoot();
        }
    }

    void CycleGasingType()
    {
        int currentTypeInt = (int)currentGasingType;
        currentTypeInt++;
        
        if (currentTypeInt >= System.Enum.GetNames(typeof(GasingType)).Length)
        {
            currentTypeInt = 0; 
        }
        
        currentGasingType = (GasingType)currentTypeInt;

        Debug.Log("Switched Gasing to: " + currentGasingType);
    }

    void Shoot()
    {
        switch (currentGasingType)
        {
            case GasingType.Angin:
                ShootAngin();
                break;
            case GasingType.Api:
                ShootApi();
                break;
            case GasingType.Tanah:
                ShootTanah();
                break;
        }
    }

    void SetupProjectileSorting(GameObject gasing)
    {
        SpriteRenderer projectileRenderer = gasing.GetComponent<SpriteRenderer>();

        if (playerSpriteRenderer != null && projectileRenderer != null)
        {
            projectileRenderer.sortingLayerID = playerSpriteRenderer.sortingLayerID;
            projectileRenderer.sortingOrder = playerSpriteRenderer.sortingOrder;
        }
        else if (projectileRenderer == null)
        {
            Debug.LogWarning("Spawned projectile prefab is missing a SpriteRenderer!", gasing);
        }
    }

    void ShootAngin()
    {
        Vector2 shootDirection = isFacingRight ? Vector2.right : Vector2.left;
        GameObject gasing = Instantiate(gasingAnginPrefab, firePoint.position, firePoint.rotation);

        AudioManager.Instance.PlaySFX("Player Shoot (Wind)");

        SetupProjectileSorting(gasing); 
        gasing.GetComponent<GasingProjectile>().Launch(shootDirection);
    }

    void ShootApi()
    {
        AudioManager.Instance.PlaySFX("Player Shoot (Fire)");

        Vector2 baseDirection = isFacingRight ? Vector2.right : Vector2.left;
        for (int i = 0; i < apiProjectileCount; i++)
        {
            Vector2 spreadDirection = baseDirection;

            if (i == 0) // Middle shot (no spread)
            {
                spreadDirection = baseDirection;
            }
            else if (i == 1) // Up shot
            {
                spreadDirection = (baseDirection + new Vector2(0, apiSpreadAmount)).normalized;
            }
            else if (i == 2) // Down shot
            {
                spreadDirection = (baseDirection + new Vector2(0, -apiSpreadAmount)).normalized;
            }
            
            GameObject gasing = Instantiate(gasingApiPrefab, firePoint.position, firePoint.rotation);
            SetupProjectileSorting(gasing); 
            gasing.GetComponent<GasingProjectile>().Launch(spreadDirection);
        }
    }

    void ShootTanah()
    {
        Vector2 shootDirection;
        if (isFacingRight)
        {
            shootDirection = (Vector2.right + Vector2.up * 0.5f).normalized;
        }
        else
        {
            shootDirection = (Vector2.left + Vector2.up * 0.5f).normalized;
        }

        GameObject gasing = Instantiate(gasingTanahPrefab, firePoint.position, firePoint.rotation);

        AudioManager.Instance.PlaySFX("Player Shoot (Ground)");

        SetupProjectileSorting(gasing);
        
        gasing.GetComponent<GasingProjectile>().Launch(shootDirection);
    }
}