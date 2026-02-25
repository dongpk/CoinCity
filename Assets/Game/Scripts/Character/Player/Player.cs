using StarterAssets;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class Player : Character
{
    [SerializeField] float respawnTime = 3f;
    [SerializeField] int attackDmg = 20;
    [SerializeField] float atkCooldown = 1f;
    [SerializeField] float attackRange = 1f;
    [SerializeField] CinemachinePositionComposer cinemachineComposer;

    Character currentTarget;
    ThirdPersonController controller;
    float lastAtkTime = Mathf.NegativeInfinity;
    public UnityEvent damaged;

    private int currentSizeLevel = 0;
    private float baseSpeed=5f;
    private float defaultDistance = 17f;
    private float _findTimer = 0f;
    private const float FIND_INTERVAL = 0.15f;

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
        if (!IsAlive) return;

        IncreaseSize();

        // ✅ Throttle: 6-7 lần/s thay vì 60
        _findTimer += Time.deltaTime;
        if (_findTimer >= FIND_INTERVAL)
        {
            _findTimer = 0f;
            currentTarget = FindNearestEnemy();
        }

        if (currentTarget != null) TryAttack(currentTarget);
    }

    // ✅ Dùng Registry
    Character FindNearestEnemy()
    {
        var characters = CharacterRegistry.Instance?.Characters;
        if (characters == null) return null;

        Character nearest = null;
        float minSqr      = attackRange * attackRange;

        for (int i = 0; i < characters.Count; i++)
        {
            var c = characters[i];
            if (c == this || !c.IsAlive) continue;
            float sqr = (transform.position - c.transform.position).sqrMagnitude;
            if (sqr < minSqr) { minSqr = sqr; nearest = c; }
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




        VFXManager.Instance.PlayFireAtkVFXFollow(transform, new Vector3(0, 0, 0));
        controller?.playerDamaged?.Invoke();

        target.TakeDamageFrom(this, attackDmg);
        //Debug.Log($"Player attacked {target.name} for {attackDmg} damage!");
    }
    public override void TakeDamageFrom(Character attacker, int damage)
    {
        base.TakeDamageFrom(attacker, damage);
        damaged?.Invoke();
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
        currentSizeLevel = 0;
        cinemachineComposer.CameraDistance = defaultDistance;
        controller?.UpdateMoveSpeed(baseSpeed);
        transform.localScale = Vector3.one;
        if (animator != null)
        {
            animator.Play("Idle Walk Run", 0, 0f);
        }
        //Debug.Log("Player has respawned.");
    }

    protected override void IncreaseSize()
    {
        int coins = coinCollector.CurrentCoins;
        int newSizeLevel = 0;
        float newScale = 1f;
        float speedReduction = 0f;
        float cameraDistance =0f;
        if (coins >= 80)
        {
            newSizeLevel = 4;
            newScale = 2.0f;
            speedReduction = 2f;
            cameraDistance = 8f;
        }
        else if (coins >= 60)
        {
            newSizeLevel = 3;
            newScale = 1.8f;
            speedReduction = 1.5f;
            cameraDistance = 6f;
        }
        else if (coins >= 40)
        {
            newSizeLevel = 2;
            newScale = 1.5f;
            speedReduction = 1f;
            cameraDistance = 4f;
        }
        else if (coins >= 20)
        {
            newSizeLevel = 1;
            newScale = 1.2f;
            speedReduction = .5f;
            cameraDistance = 2f;
        }
        else
        {
            newSizeLevel = 0;
            newScale = 1f;
            speedReduction = 0;
            cameraDistance = defaultDistance;
        }

        if (newSizeLevel != currentSizeLevel)
        {
            currentSizeLevel = newSizeLevel;
            transform.localScale = new Vector3(newScale,newScale,newScale);

            float newCameraDistance = defaultDistance + cameraDistance;
            float newSpeed = baseSpeed - speedReduction;
            controller?.UpdateMoveSpeed(newSpeed);
            cinemachineComposer.CameraDistance = newCameraDistance;

        }
    }
}
