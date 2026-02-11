using UnityEngine;

[RequireComponent(typeof(BotAI))]
[RequireComponent(typeof(CoinCollector))]
public class Bot : Character
{
    BotAI aiBot;
    [SerializeField] BotConfig configBot;

    private int currentSizeLevel = 0;
    private float baseSpeed = 5f;
    public string BotName => configBot != null ? configBot.botName : "Bot";

    private void Awake()
    {
        aiBot = GetComponent<BotAI>();
        
        maxHealth = configBot != null ? configBot.maxHealth : 100;
    }
    
    protected override void Start()
    {
        base.Start();
        Invoke(nameof(RefreshAnimator), 0.01f);
    }
    
    private void Update()
    {
        IncreaseSize();
    }
    
    void RefreshAnimator()
    {
        animator = GetComponentInChildren<Animator>();
    }
   
    public void Initialized(BotConfig botConfig)
    {
        configBot = botConfig;
        currentHealth = maxHealth;
        gameObject.name = configBot.botName;

        if (aiBot != null)
        {
            aiBot.SetConfig(configBot);
            baseSpeed = configBot.moveSpeed;
        }
        
        RefreshAnimator();
    }

    protected override void Die()
    {
        Debug.Log($"{BotName} hẹo.");
        
        if (animator != null && animator.gameObject.activeInHierarchy)
        {
            animator.SetTrigger("Died");
        }
        
        if (aiBot != null)
        {
            aiBot.enabled = false;
        }
        
        var navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = false;
        }

        Invoke(nameof(Respawn), 3f);
    }
    
    protected override void Respawn()
    {
        currentHealth = maxHealth;
        healthBar.UpdateHealthBar(maxHealth, currentHealth);
        
        currentSizeLevel = 0;
        transform.localScale = Vector3.one;
        
        if (aiBot != null)
        {
            aiBot.enabled = true;
            aiBot.SetSpeed(baseSpeed); 
        }
        
        var navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = true;
        }
        
        RefreshAnimator();
        if (animator != null)
        {
            animator.Play("Idle Walk Run", 0, 0f);
        }
       
        Debug.Log($"{BotName} đã hồi sinh.");
    }

    protected override void IncreaseSize()
    {
        int coins = coinCollector.CurrentCoins;
        int newSizeLevel = 0;
        float newScale = 1f;
        float speedReduction = 0f;
        
        if (coins >= 80)
        {
            newSizeLevel = 4;
            newScale = 2.0f;
            speedReduction = 2f;
        }
        else if (coins >= 60)
        {
            newSizeLevel = 3;
            newScale = 1.8f;
            speedReduction = 1.5f;
        }
        else if (coins >= 40)
        {
            newSizeLevel = 2;
            newScale = 1.5f;
            speedReduction = 1f;
        }
        else if (coins >= 20)
        {
            newSizeLevel = 1;
            newScale = 1.2f;
            speedReduction = 0.5f;
        }
        else
        {
            newSizeLevel = 0;
            newScale = 1f;
            speedReduction = 0;
        }

        if (newSizeLevel != currentSizeLevel)
        {
            currentSizeLevel = newSizeLevel;
            transform.localScale = new Vector3(newScale, newScale, newScale);

            float newSpeed = baseSpeed - speedReduction;
            
            if (aiBot != null)
            {
                aiBot.SetSpeed(newSpeed);
            }
        }
    }
}
