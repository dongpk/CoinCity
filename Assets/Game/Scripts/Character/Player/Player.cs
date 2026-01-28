using StarterAssets;
using UnityEngine;

public class Player : Character
{
    [SerializeField] float respawnTime = 3f;
    [SerializeField] int attackDmg = 20;
    [SerializeField] float atkCooldown = 1f;
    [SerializeField] float attackRange = 1f;

    Character currentTarget;
    ThirdPersonController controller;
    float lastAtkTime = Mathf.NegativeInfinity;


    protected override void Start()
    {
        controller = GetComponent<ThirdPersonController>();
        base.Start();
        Invoke(nameof(RefreshAnimator), 0.01f);
    }
    void RefreshAnimator()
    {
       
        animator = GetComponentInChildren<Animator>();

    }
    private void Update()
    {
        if (!IsAlive) 
        { 
            return; 
        }

        currentTarget = FindNearestEnemy();
        if (currentTarget != null)
        {
            TryAttack(currentTarget);
        }
    }
    Character FindNearestEnemy()
    {
        Character nearest = null;
        float minDistance = attackRange;

        foreach (var character in FindObjectsByType<Character>(FindObjectsSortMode.None))
        {
            if(character == this || !character.IsAlive)
            {
                continue;
            }
            float distance = Vector3.Distance(transform.position,character.transform.position);
            if(distance < minDistance)
            {
                minDistance = distance;
                nearest = character;
            }
        }
        return nearest;
    }

    void TryAttack(Character target)
    {
        if (Time.time - lastAtkTime < atkCooldown)
        {
            return;
        }

        lastAtkTime = Time.time;

    

        
        VFXManager.Instance.PlayFireAtkVFXFollow(transform,new Vector3(0,0,0));
        controller?.playerDamaged?.Invoke();

        target.TakeDamageFrom(this, attackDmg);
        //Debug.Log($"Player attacked {target.name} for {attackDmg} damage!");
    }

 

    

    // Kiểm tra animator có null không
    protected override void Die()
    {
        //Debug.Log("Player has died.");
        
        
        controller?.playerDead?.Invoke();

        if (animator != null && animator.gameObject.activeInHierarchy)
        {
            animator.SetTrigger("Died");
            //Debug.Log("Trigger Died set!");
        }
        GetComponent<ThirdPersonController>().enabled = false;
        
        Invoke(nameof(Respawn), respawnTime);
    }

    protected override void Respawn()
    {
        currentHealth = maxHealth;
        healthBar.UpdateHealthBar(maxHealth, currentHealth);
        GetComponent<ThirdPersonController>().enabled = true;
        if (animator != null)
        {
            animator.Play("Idle Walk Run", 0, 0f);
        }
        //Debug.Log("Player has respawned.");
    }
}
