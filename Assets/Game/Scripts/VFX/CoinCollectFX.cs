using StarterAssets;
using UnityEngine;

public class CoinCollectFX : MonoBehaviour
{
    ThirdPersonController controller;
    //[SerializeField] CoinCollectFX;

    private void Start()
    {
        controller = GetComponent<ThirdPersonController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Coin"))
        {                  
            controller.coinCollected.Invoke();
        }
    }
   
}
