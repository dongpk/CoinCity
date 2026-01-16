using UnityEngine;

public class CoinVFXItem : MonoBehaviour
{


    private void OnParticleSystemStopped()
    {
        this.gameObject.SetActive(false);
    }
}
