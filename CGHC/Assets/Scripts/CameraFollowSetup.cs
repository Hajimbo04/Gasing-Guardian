using UnityEngine;
using Unity.Cinemachine; 

[RequireComponent(typeof(CinemachineCamera))]
public class CameraFollowSetup : MonoBehaviour
{
    private CinemachineCamera vcam;

    void Start()
    {
        vcam = GetComponent<CinemachineCamera>();
        if (HealthSystem.Instance != null)
        {
            vcam.Follow = HealthSystem.Instance.transform;
        }
        else
        {
            Debug.LogError("CAMERA SETUP: Cannot find persistent Player (HealthSystem.Instance)!", this);
        }

        GameObject cameraBounds = GameObject.Find("CameraBounds2D");
        if (cameraBounds != null)
        {
            CinemachineConfiner2D confiner = vcam.GetComponent<CinemachineConfiner2D>();
            if (confiner != null)
            {
                confiner.BoundingShape2D = cameraBounds.GetComponent<PolygonCollider2D>();
            }
        }
    }
}