using UnityEngine;

public class VFXItem : MonoBehaviour
{
    private void OnParticleSystemStopped()
    {
        // Reset follower nếu có
        var follower = GetComponent<VFXFollower>();
        if (follower != null)
        {
            follower.SetTarget(null, Vector3.zero);
        }
        
        gameObject.SetActive(false);
    }
}
