using System;
using UnityEngine;

public abstract class Character : MonoBehaviour, IDamageable
{
    [SerializeField] protected int maxHealth;
    [SerializeField] protected HealthBar healthBar;
    protected int currentHealth;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;

    public bool IsAlive => currentHealth > 0;
    public float HealthPercent => (float)currentHealth / maxHealth;

    protected CoinCollector coinCollector;
    protected Animator animator;

    public event Action<Character> OnDeath;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        healthBar?.UpdateHealthBar(maxHealth, currentHealth);
        coinCollector = GetComponent<CoinCollector>();
        animator = GetComponentInChildren<Animator>();
    }
    public virtual void ResetHealth()
    {
        currentHealth = maxHealth;
        healthBar?.UpdateHealthBar(maxHealth, currentHealth);
    }
    public virtual void TakeDamage(int damageAmount)
    {
        if (!IsAlive) { return; }

        currentHealth -= damageAmount;
        //Debug.Log($"{gameObject.name} bị mất {damageAmount} máu. Còn lại: {currentHealth}");
        healthBar?.UpdateHealthBar(maxHealth, currentHealth);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            OnDeath?.Invoke(this);
            Die();
        }
    }
    public virtual void TakeDamageFrom(Character atker, int dmgAmount)
    {
        TakeDamage(dmgAmount);
        if (!IsAlive && atker != null)
        {
            int coinStolen = coinCollector.TakeAllCoins();
            atker.GetComponent<CoinCollector>()?.AddCoin(coinStolen);
            //Debug.Log($"{atker.name} lay duoc {coinStolen} xu tu {gameObject.name}");
        }
    }
    public virtual void Heal(int healAmount)
    {
        if (!IsAlive)
        {
            return;
        }
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        healthBar?.UpdateHealthBar(maxHealth, currentHealth);
        //Debug.Log($"{gameObject.name} được hồi {healAmount} máu. Hiện tại: {currentHealth}");
    }

    protected abstract void Die();
    protected abstract void Respawn();
    protected int GetCoins() => coinCollector?.CurrentCoins ?? 0;

}
