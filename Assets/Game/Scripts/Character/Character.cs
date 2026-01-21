using System;
using UnityEngine;

public abstract class Character : MonoBehaviour,IDamageable
{
    [SerializeField] protected int maxHealth;
    protected int currentHealth;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public bool IsAlive => currentHealth > 0;
    public float HealthPercent => (float)currentHealth / maxHealth;

    protected CoinCollector coinCollector;

    public event Action<Character> OnDeath;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        coinCollector = GetComponent<CoinCollector>();
    }

    public virtual void TakeDamage(int damageAmount)
    {
        if (!IsAlive) { return; }

        currentHealth -= damageAmount;
        Debug.Log($"{gameObject.name} bị mất {damageAmount} máu. Còn lại: {currentHealth}");
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }
    public virtual void TakeDamageFrom(Character atker, int dmgAmount)
    {
        TakeDamage(dmgAmount);
        if(!IsAlive && atker!= null)
        {
            int coinStolen = coinCollector.TakeAllCoins();
            atker.GetComponent<CoinCollector>()?.AddCoin(coinStolen);
            Debug.Log($"{atker.name} lay duoc {coinStolen} xu tu {gameObject.name}");
        }
    }

    protected abstract void Die();
    protected int GetCoins() => coinCollector?.CurrentCoins ?? 0;

}
