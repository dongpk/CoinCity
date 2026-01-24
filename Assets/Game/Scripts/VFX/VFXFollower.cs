using UnityEngine;

public class VFXFollower : MonoBehaviour
{
    private Transform target;
    private Vector3 offset;
    private ParticleSystem ps;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    public void SetTarget(Transform newTarget, Vector3 newOffset)
    {
        target = newTarget;
        offset = newOffset;
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            // Target bị destroy hoặc null → dừng particle
            if (ps != null && ps.isPlaying)
            {
                ps.Stop();
            }
            return;
        }

        transform.position = target.position + offset;
    }

    private void OnDisable()
    {
        target = null;
    }
}
