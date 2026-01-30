using Unity.Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    private CinemachineImpulseSource impulseSource;
    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("CameraShake instances found! Destroying duplicate.", gameObject);
            Destroy(gameObject);
            return;
        }

        // Set instance
        Instance = this;
        Debug.Log("CameraShake initialized", gameObject);
    }

    public void ShakeCamera()
    {
        impulseSource.GenerateImpulse();
    }

}
