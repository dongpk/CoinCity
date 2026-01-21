using UnityEngine;
using UnityEngine.AI;

public enum BotState
{
    Idle,
    Collection,
    ChaseEnemy,
    AttackEnemy,
    Flee,
}
[RequireComponent(typeof(NavMeshAgent))]
public class BotAI : MonoBehaviour
{
    [SerializeField] BotConfig botConfig;


    NavMeshAgent agent;
    Character character;
    CoinCollector coinCollector;
    SkinSelector skinSelector;
    Animator animator;

    BotState currentState = BotState.Idle;
    Transform currentTarget;
    Character targetEnemy;
    float lastAttack;
    float stateTimer;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        character = GetComponent<Character>();
        coinCollector = GetComponent<CoinCollector>();
        skinSelector = GetComponent<SkinSelector>();



        if (botConfig != null)
        {
            //Debug.Log($"{name}: apply config skinIndex = {botConfig.skinIndex}");
            ApplyConfig();
        }
        animator = GetComponentInChildren<Animator>();
    }

  
    private void ApplyConfig()
    {
        agent.speed = botConfig.moveSpeed;
        skinSelector?.SetSkin(botConfig.skinIndex);
        //Debug.Log($"current skinIndex = {skinSelector?.GetCurrentSkinIndex()}");
    }
    public void SetConfig(BotConfig newConfig)
    {
        botConfig = newConfig;
        ApplyConfig();
    }

    private void Update()
    {
        if (!character.IsAlive)
        {
            return;

        }

        stateTimer += Time.deltaTime;

        EvaluateSituation();

        ExecuteState();
        UpdateAnimator();
    }
    void UpdateAnimator()
    {
        if (animator == null)
        {
            return;
        }
        float speed = agent.velocity.magnitude;
       
        animator.SetFloat("Speed", speed);
    }
   

    private void EvaluateSituation()
    {
        Character nearestEnemy = FindNearestEnemy();
        if (nearestEnemy != null)
        {
            float distance = Vector3.Distance(transform.position, nearestEnemy.transform.position);
            targetEnemy = nearestEnemy;

            bool isLowHealth = character.CurrentHealth <= (botConfig.feelHealthThreshHold * 100);
            bool isEnemyStronger = nearestEnemy.CurrentHealth > character.CurrentHealth;

            if (isLowHealth && isEnemyStronger)
            {
                ChangeState(BotState.Flee);
                return;
            }

            if (distance <= botConfig.attackRange && !isLowHealth)
            {
                ChangeState(BotState.AttackEnemy);
                return;
            }



            bool isEnemyWeaker = nearestEnemy.CurrentHealth <= character.CurrentHealth;
            bool inDetectionRange = distance <= botConfig.dectecionRange;
            if (isEnemyWeaker && inDetectionRange && Random.value < botConfig.aggressiveness)
            {
                ChangeState(BotState.ChaseEnemy);
                if (distance <= botConfig.attackRange)
                {
                    ChangeState(BotState.AttackEnemy);
                }
                return;
            }

        }

        FindAndCollectionCoin();
    }
    void FindAndCollectionCoin()
    {
        GameObject nearestCoin = FindNearstCoin();
        if (nearestCoin != null)
        {
            currentTarget = nearestCoin.transform;
            ChangeState(BotState.Collection);
        }
        else
        {
            ChangeState(BotState.Idle);
        }
    }
    void ChangeState(BotState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
            stateTimer = 0f;

        }
    }
    void ExecuteState()
    {
        switch (currentState)
        {
            case BotState.Idle:
                if (stateTimer > .1f)
                {
                    MoveToRandomPosition();
                    stateTimer = 0f;
                }
                break;
            case BotState.Collection:
                if (currentTarget != null)
                {
                    agent.SetDestination(currentTarget.position);
                }
                else
                {
                    ChangeState(BotState.Idle);
                }
                break;
            case BotState.ChaseEnemy:
                if (targetEnemy != null && targetEnemy.IsAlive)
                {
                    agent.SetDestination(targetEnemy.transform.position);
                    float distance = Vector3.Distance(transform.position, targetEnemy.transform.position);
                    if (distance <= botConfig.attackRange)
                    {
                        ChangeState(BotState.AttackEnemy);
                    }
                }
                else
                {
                    ChangeState(BotState.Idle);
                }
                break;

            case BotState.AttackEnemy:
                if (targetEnemy != null && targetEnemy.IsAlive)
                {
                    agent.SetDestination(targetEnemy.transform.position);
                    TryAttack();

                }
                else
                {
                    ChangeState(BotState.Idle);
                }

                break;


            case BotState.Flee:
                if (targetEnemy != null && targetEnemy.IsAlive)
                {
                    float distance = Vector3.Distance(transform.position, targetEnemy.transform.position);
                    if (distance <= botConfig.attackRange)
                    {
                        TryAttack();
                    }
                    Vector3 fleeDirection = (transform.position - targetEnemy.transform.position).normalized;
                    Vector3 fleePostion = transform.position + fleeDirection * botConfig.patrolRadius;
                    agent.SetDestination(fleePostion);
                }


                if (stateTimer > 3f)
                {
                    ChangeState(BotState.Idle);
                }
                break;
        }
    }
    void TryAttack()
    {
        if (Time.time - lastAttack >= botConfig.attackCooldown)
        {
            float distance = Vector3.Distance(transform.position, targetEnemy.transform.position);
            if (distance <= botConfig.attackRange)
            {
                targetEnemy.TakeDamageFrom(character, botConfig.attackDmg);
                lastAttack = Time.time;
                Debug.Log($"{name} tan cong {targetEnemy.name}");

            }
        }
    }
    Character FindNearestEnemy()
    {
        Character nearest = null;
        float minDistance = botConfig.dectecionRange;
        foreach (var character in FindObjectsByType<Character>(FindObjectsSortMode.None))
        {
            if (character == this.character || !character.IsAlive)
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, character.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = character;
            }
        }
        return nearest;
    }

    Character FindWeaker()
    {
        Character weaker = null;
        float minHealth = character.CurrentHealth;

        foreach (var enemy in FindObjectsByType<Character>(FindObjectsSortMode.None))
        {
            if (enemy == this.character || !enemy.IsAlive)
            {
                continue;
            }
            float distance = Vector3.Distance(transform.position, character.transform.position);
            if (distance <= botConfig.dectecionRange && enemy.CurrentHealth < minHealth)
            {
                minHealth = enemy.CurrentHealth;
                weaker = enemy;
            }
        }
        return weaker;
    }

    private GameObject FindNearstCoin()
    {
        GameObject nearest = null;
        float minDistance = Mathf.Infinity;
        foreach (var coin in GameObject.FindGameObjectsWithTag("Coin"))
        {
            if (!coin.activeInHierarchy)
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, coin.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = coin;
            }
        }
        return nearest;
    }

    void MoveToRandomPosition()
    {
        Vector3 randomDirecton = Random.insideUnitSphere * botConfig.patrolRadius;
        randomDirecton += transform.position;
        if (NavMesh.SamplePosition(randomDirecton, out NavMeshHit hit, botConfig.patrolRadius,
            NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}
