using UnityEngine;
using UnityEngine.AI;

public enum BotState { Idle, Collection, ChaseEnemy, AttackEnemy, Flee }

[RequireComponent(typeof(NavMeshAgent))]
public class BotAI : MonoBehaviour
{
    [SerializeField] BotConfig botConfig;

    NavMeshAgent  agent;
    Character     character;
    CoinCollector coinCollector;
    SkinSelector  skinSelector;
    Animator      animator;

    BotState  currentState = BotState.Idle;
    Transform currentTarget;
    Character targetEnemy;
    float     lastAttack;
    float     stateTimer;

    // ✅ THROTTLE TIMERS - không chạy mỗi frame nữa
    float _evalTimer;
    float _navTimer;
    const float EVAL_INTERVAL = 0.2f;   // AI suy nghĩ 5 lần/s
    const float NAV_INTERVAL  = 0.25f;  // NavMesh update 4 lần/s

    private void Start()
    {
        agent         = GetComponent<NavMeshAgent>();
        character     = GetComponent<Character>();
        coinCollector = GetComponent<CoinCollector>();
        skinSelector  = GetComponent<SkinSelector>();

        if (botConfig != null) ApplyConfig();
        animator = GetComponentInChildren<Animator>();

        // ✅ Stagger: mỗi bot bắt đầu ở thời điểm khác nhau
        _evalTimer = Random.Range(0f, EVAL_INTERVAL);
        _navTimer  = Random.Range(0f, NAV_INTERVAL);
    }

    private void ApplyConfig()
    {
        agent.speed = botConfig.moveSpeed;
        skinSelector?.SetSkin(botConfig.skinIndex);
    }

    public void SetConfig(BotConfig newConfig) { botConfig = newConfig; ApplyConfig(); }
    public void SetSpeed(float s) { if (agent != null) agent.speed = s; }

    private void Update()
    {
        if (!character.IsAlive) return;

        float dt = Time.deltaTime;
        stateTimer  += dt;
        _evalTimer  += dt;
        _navTimer   += dt;

        // ✅ AI evaluate: 5 lần/s thay vì 60 lần/s
        if (_evalTimer >= EVAL_INTERVAL)
        {
            _evalTimer = 0f;
            EvaluateSituation();
        }

        // ✅ Navigation: 4 lần/s
        if (_navTimer >= NAV_INTERVAL)
        {
            _navTimer = 0f;
            ExecuteStateNav();
        }

        UpdateAnimator();
    }

    void UpdateAnimator()
    {
        if (animator != null)
            animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    private void EvaluateSituation()
    {
        Character nearestEnemy = FindNearestEnemy();

        if (character.CurrentHealth <= 50 && nearestEnemy == null)
        {
            FindAndCollectionHealth();
            return;
        }

        if (nearestEnemy != null)
        {
            // ✅ Dùng sqrMagnitude thay Vector3.Distance (tránh sqrt)
            float sqrDist = (transform.position - nearestEnemy.transform.position).sqrMagnitude;
            targetEnemy   = nearestEnemy;

            bool isLowHealth    = character.CurrentHealth <= (botConfig.feelHealthThreshHold * 100);
            bool isEnemyStronger = nearestEnemy.CurrentHealth > character.CurrentHealth;
            float atkRangeSqr   = botConfig.attackRange * botConfig.attackRange;
            float detRangeSqr   = botConfig.dectecionRange * botConfig.dectecionRange;

            if (isLowHealth && isEnemyStronger) { ChangeState(BotState.Flee); return; }

            if (sqrDist <= atkRangeSqr)
            {
                TryAttack();
                if (!isLowHealth) { ChangeState(BotState.AttackEnemy); return; }
            }

            bool isWeaker = nearestEnemy.CurrentHealth <= character.CurrentHealth;
            if (isWeaker && sqrDist <= detRangeSqr && Random.value < botConfig.aggressiveness)
            {
                ChangeState(sqrDist <= atkRangeSqr ? BotState.AttackEnemy : BotState.ChaseEnemy);
                return;
            }
        }

        FindAndCollectionCoin();
    }

    // ✅ Tách navigation riêng để throttle
    void ExecuteStateNav()
    {
        switch (currentState)
        {
            case BotState.Idle:
                if (stateTimer > 0.1f) { MoveToRandomPosition(); stateTimer = 0f; }
                break;

            case BotState.Collection:
                if (currentTarget != null) agent.SetDestination(currentTarget.position);
                else ChangeState(BotState.Idle);
                break;

            case BotState.ChaseEnemy:
            case BotState.AttackEnemy:
                if (targetEnemy != null && targetEnemy.IsAlive)
                {
                    agent.SetDestination(targetEnemy.transform.position);
                    if (currentState == BotState.AttackEnemy) TryAttack();
                }
                else ChangeState(BotState.Idle);
                break;

            case BotState.Flee:
                if (targetEnemy != null && targetEnemy.IsAlive)
                {
                    Vector3 fleeDir = (transform.position - targetEnemy.transform.position).normalized;
                    agent.SetDestination(transform.position + fleeDir * botConfig.patrolRadius);
                }
                if (stateTimer > 3f) ChangeState(BotState.Idle);
                break;
        }
    }

    void TryAttack()
    {
        if (targetEnemy == null) return;
        if (Time.time - lastAttack < botConfig.attackCooldown) return;

        float sqrDist = (transform.position - targetEnemy.transform.position).sqrMagnitude;
        if (sqrDist <= botConfig.attackRange * botConfig.attackRange)
        {
            targetEnemy.TakeDamageFrom(character, botConfig.attackDmg);
            VFXManager.Instance.PlayVFXFollow(botConfig.attackVFXPrefab, transform, new Vector3(0, .2f, 0));
            lastAttack = Time.time;
        }
    }

    void ChangeState(BotState s) { if (currentState != s) { currentState = s; stateTimer = 0f; } }

    // ✅ Dùng Registry thay FindObjectsByType
    Character FindNearestEnemy()
    {
        var characters = CharacterRegistry.Instance?.Characters;
        if (characters == null) return null;

        Character nearest = null;
        float minSqr      = botConfig.dectecionRange * botConfig.dectecionRange;

        for (int i = 0; i < characters.Count; i++)
        {
            var c = characters[i];
            if (c == character || !c.IsAlive) continue;

            float sqr = (transform.position - c.transform.position).sqrMagnitude;
            if (sqr < minSqr) { minSqr = sqr; nearest = c; }
        }
        return nearest;
    }

    // ✅ Dùng Registry thay FindGameObjectsWithTag("Coin")
    void FindAndCollectionCoin()
    {
        var coins = CharacterRegistry.Instance?.Coins;
        if (coins == null || coins.Count == 0) { ChangeState(BotState.Idle); return; }

        Transform nearest = null;
        float minSqr       = Mathf.Infinity;

        for (int i = 0; i < coins.Count; i++)
        {
            if (coins[i] == null) continue;
            float sqr = (transform.position - coins[i].position).sqrMagnitude;
            if (sqr < minSqr) { minSqr = sqr; nearest = coins[i]; }
        }

        if (nearest != null) { currentTarget = nearest; ChangeState(BotState.Collection); }
        else ChangeState(BotState.Idle);
    }

    void FindAndCollectionHealth()
    {
        var healths = CharacterRegistry.Instance?.Healths;
        if (healths == null || healths.Count == 0) { ChangeState(BotState.Idle); return; }

        Transform nearest = null;
        float minSqr       = Mathf.Infinity;

        for (int i = 0; i < healths.Count; i++)
        {
            if (healths[i] == null) continue;
            float sqr = (transform.position - healths[i].position).sqrMagnitude;
            if (sqr < minSqr) { minSqr = sqr; nearest = healths[i]; }
        }

        if (nearest != null) { currentTarget = nearest; ChangeState(BotState.Collection); }
        else ChangeState(BotState.Idle);
    }

    void MoveToRandomPosition()
    {
        Vector3 randomDir = Random.insideUnitSphere * botConfig.patrolRadius + transform.position;
        if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, botConfig.patrolRadius, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
    }
}
