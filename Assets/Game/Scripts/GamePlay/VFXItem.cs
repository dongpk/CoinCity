using UnityEngine;

public class VFXItem : MonoBehaviour
{


    private void OnParticleSystemStopped()
    {
        this.gameObject.SetActive(false);
    }
}
