using UnityEngine;

[RequireComponent(typeof(BotAI))]
[RequireComponent(typeof(CoinCollector))]
public class Bot : Character
{
    BotAI aiBot;
    BotConfig configBot;

    public string BotName => configBot != null ? configBot.botName : "Bot";

    public void Initalize(BotConfig botConfig)
    {
        configBot = botConfig;
        maxHealth = configBot.maxHealth;
        currentHealth = maxHealth;
        gameObject.name = configBot.botName;

        aiBot = GetComponent<BotAI>();
        aiBot.SetConfig(configBot);
    }

    protected override void Die()
    {
        Debug.Log($"{BotName} hẹo.");

        GetComponent<BotAI>().enabled = false;
        GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;

        Invoke(nameof(Respawn), 3f);
    }
    void Respawn()
    {
        currentHealth = maxHealth;
        GetComponent<BotAI>().enabled = true;
        GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
        Debug.Log($"{BotName} đã hồi sinh.");

        //TODO: làm game manager để respawn bot ở vị trí hợp lý 
    }
}
