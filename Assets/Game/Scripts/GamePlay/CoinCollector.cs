using StarterAssets;
using System;
using TMPro;
using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI coinText;
    [SerializeField] HealthBar healthBar;
    public int CurrentCoins { get; private set; } = 0;

    public event Action<int> OnCoinCollected;

    ThirdPersonController controller;
    private void Start()
    {
        TryGetComponent(out controller);
        UpdateCoinUI();
    }

    public void ResetCoins()
    {
        CurrentCoins = 0;
        UpdateCoinUI();
        OnCoinCollected?.Invoke(CurrentCoins);
    }
    public void AddCoin(int amt)
    {
        CurrentCoins += amt;
        if (healthBar != null)
        {
            healthBar.ShowCoinGain(amt, Color.black);
        }
        UpdateCoinUI();
        OnCoinCollected?.Invoke(CurrentCoins);
    }
    public void RemoveCoin(int amt)
    {
        CurrentCoins  = Mathf.Max(0,CurrentCoins  - amt);
        UpdateCoinUI();
        OnCoinCollected?.Invoke(CurrentCoins);
    }
    public int TakeAllCoins()
    {
        int coins = CurrentCoins;
        CurrentCoins = 0;
        UpdateCoinUI();
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
            
            //Debug.Log($"{other.name} Collected! Total Coins: " + CurrentCoins);

        }
    }

    private void UpdateCoinUI()
    {
        if(coinText != null)
        {
            coinText.text = CurrentCoins.ToString();
        }
    }
}
