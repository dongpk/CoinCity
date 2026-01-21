using StarterAssets;
using System;
using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    public int CurrentCoins { get; private set; } = 0;

    public event Action<int> OnCoinCollected;

    ThirdPersonController controller;
    private void Start()
    {
        TryGetComponent(out controller);
    }
    public void AddCoin(int amt)
    {
        CurrentCoins += amt;
        
        OnCoinCollected?.Invoke(CurrentCoins);
    }
    public void RemoveCoin(int amt)
    {
        CurrentCoins  = Mathf.Max(0,CurrentCoins  - amt);
        OnCoinCollected?.Invoke(CurrentCoins);
    }
    public int TakeAllCoins()
    {
        int coins = CurrentCoins;
        CurrentCoins = 0;
        OnCoinCollected?.Invoke(CurrentCoins);
        return coins;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Coin"))
        {                  
            VFXManager.Instance.PlayCoinVFX(this.gameObject.transform.position);
            controller?.coinCollected?.Invoke();
            AddCoin(1);
            Debug.Log($"{other.name} Collected! Total Coins: " + CurrentCoins);
        
        }
    }
}
