using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [Header("Cinemachine Target")]
    public CinemachineCamera virtualCamera; 

    private CinemachineBasicMultiChannelPerlin perlinNoise;
    private Coroutine shakeCoroutine; 

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        if (virtualCamera != null)
        {
            perlinNoise = virtualCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
            if (perlinNoise == null)
            {
                Debug.LogError("No Perlin Noise component found on the Virtual Camera!");
            }
        }
        else
        {
            Debug.LogError("Virtual Camera not assigned in CameraShake script!");
        }
    }

    public void StartShake(float duration, float magnitude)
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }

        if (perlinNoise != null)
        {
            shakeCoroutine = StartCoroutine(ShakeRoutine(duration, magnitude));
        }
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        Debug.Log("Cinemachine ShakeRoutine STARTED!");
        perlinNoise.AmplitudeGain = magnitude;
        
        yield return new WaitForSeconds(duration);
        
        perlinNoise.AmplitudeGain = 0f;
        shakeCoroutine = null;
    }
}