using StarterAssets;
using UnityEngine;

public class Player : Character
{
    [SerializeField] float respawnTime = 3f;
    [SerializeField] int attackDmg = 20;
    [SerializeField] float atkCooldown = 1f;
    [SerializeField] float attackRange = 1f;

    Character currentTarget;
    float lastAtkTime = Mathf.NegativeInfinity;


    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
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

        // Quay về phía mục tiêu
        //LookAtTarget(target);

        // Debug kiểm tra animator
        VFXManager.Instance.PlayFireAtkVFXFollow(transform,new Vector3(0,0,0));


        target.TakeDamageFrom(this, attackDmg);
        Debug.Log($"Player attacked {target.name} for {attackDmg} damage!");
    }

 

    void LookAtTarget(Character target)
    {
        Vector3 direction = (target.transform.position - transform.position).normalized;
        direction.y = 0;
        if (direction!= Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void PerformAttack()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up;
        Vector3 direction = transform.forward;
        if (Physics.Raycast(origin, direction, out hit, attackRange))
        {
            Character target = hit.collider.GetComponent<Character>();
            if (target == null)
            {
                target = hit.collider.GetComponentInParent<Character>();
            }
            if (target != null && target != this)
            {
                target.TakeDamageFrom(this, attackDmg);
                Debug.Log($"Player attacked {hit.collider.name}");
            }

        }
    }

    protected override void Die()
    {
        Debug.Log("Player has died.");
        GetComponent<ThirdPersonController>().enabled = false;
        if (animator != null && animator.gameObject.activeInHierarchy)
        {
            animator.SetTrigger("Died");
        }
        Invoke(nameof(Respawn), respawnTime);
    }

    protected override void Respawn()
    {
        currentHealth = maxHealth;
        GetComponent<ThirdPersonController>().enabled = true;
        if (animator != null)
        {
            animator.Play("Idle Walk Run", 0, 0f);
        }
        Debug.Log("Player has respawned.");
    }
}
