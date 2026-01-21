using UnityEngine;

[RequireComponent(typeof(BotAI))]
[RequireComponent(typeof(CoinCollector))]
public class Bot : Character
{
    BotAI aiBot;
    BotConfig configBot;
 

    public string BotName => configBot != null ? configBot.botName : "Bot";

    protected override void Start()
    {
        base.Start();
        Invoke(nameof(RefreshAnimator), 0.01f);
    }
    void RefreshAnimator()
    {
        animator = GetComponentInChildren<Animator>();

    }
    public void Initalize(BotConfig botConfig)
    {
        configBot = botConfig;
        maxHealth = configBot.maxHealth;
        currentHealth = maxHealth;
        gameObject.name = configBot.botName;

        aiBot = GetComponent<BotAI>();
        aiBot.SetConfig(configBot);
        RefreshAnimator();
    }

    protected override void Die()
    {
        Debug.Log($"{BotName} hẹo.");
        
        
        if (animator != null && animator.gameObject.activeInHierarchy)
        {
            animator.SetTrigger("Died");
        }
        GetComponent<BotAI>().enabled = false;
        GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
       


        Invoke(nameof(Respawn), 3f);
    }
    protected override void Respawn()
    {
        currentHealth = maxHealth;
        GetComponent<BotAI>().enabled = true;
        GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
        RefreshAnimator();
        if (animator != null)
        {
            animator.Play("Idle Walk Run", 0, 0f);
        }
       
        Debug.Log($"{BotName} đã hồi sinh.");

        //TODO: làm game manager để respawn bot ở vị trí hợp lý 
    }

   
}
