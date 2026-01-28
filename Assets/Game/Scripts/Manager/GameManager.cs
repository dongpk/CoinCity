using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;
using System.Collections;
using UnityEngine.SceneManagement;
public enum GameState
{
    Menu,
    CountDown,
    Playing,
    Ended
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance{get; private set; }

    [Header("Game Ssttings")]
    [SerializeField] float gameDuration = 120f;
    [SerializeField] float countdownTimer = 3f;

    [Space]
    [SerializeField] GameObject TouchScreenInput;
    [SerializeField] Player player;
    [SerializeField] List<Bot > bots = new List<Bot>();

    [Space]
    [Header("Game Events")]
    public UnityEvent OnShowMenu;
    public UnityEvent<int> OnCountdown;
    public UnityEvent OnGameStart;
    public UnityEvent OnGameEnd;
    public UnityEvent<float> OnTimerUpdate;
    public UnityEvent<List<RankEntry>> OnRankingUpdate;

    public Player GetPlayer() => player;

    public GameState CurrentGameState {get; private set; } = GameState.Menu;
    public float GameTimer {get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple GameManager instances found! Destroying duplicate.", gameObject);
            Destroy(gameObject);
            return;
        }

        // Set instance
        Instance = this;
        Debug.Log("GameManager initialized", gameObject);
    }
    private void Start()
    {
        if(player == null)
        {
            player = FindFirstObjectByType<Player>();
        }
        if (bots.Count == 0)
        {
            bots = FindObjectsByType<Bot>(FindObjectsSortMode.None).ToList();


        }

        ShowMenu();
    }
    private void Update()
    {
        if (CurrentGameState != GameState.Playing)
        {
            return;
        }

        GameTimer -= Time.deltaTime;
        OnTimerUpdate?.Invoke(GameTimer);

        if (Time.frameCount % 30 == 0)
        {
            UpdateRank();
        }
        if (GameTimer <= 0f)
        {
            EndGame();
        }
    }

    public void ShowMenu()
    {
        CurrentGameState = GameState.Menu;
        TouchScreenInput.SetActive(false);
        Time.timeScale = 0f;
        OnShowMenu?.Invoke();

        SetCharacterActive(false);
    }

    public void OnClickedPlayButton()
    {
        StartCoroutine(CountdownAndStartGame());
    }
    IEnumerator CountdownAndStartGame()
    {
        CurrentGameState = GameState.CountDown;
        Time.timeScale = 1f;
        for (int i = (int)countdownTimer; i > 0; i--)
        {
            OnCountdown?.Invoke(i);
            yield return new WaitForSeconds(1f);
        }
        StartGame();
    }
    public void StartGame()
    {
        CurrentGameState = GameState.Playing;
        GameTimer = gameDuration;
        Time.timeScale = 1f;
        TouchScreenInput.SetActive(true);
        SetCharacterActive(true);
        //reset coins
        ResetALLCoins();


        OnGameStart?.Invoke();
        //rankings update
        UpdateRank();
        Debug.Log("Game Started");

    }
    public void EndGame()
    {
        CurrentGameState = GameState.Ended;
        GameTimer = 0f;
        Time.timeScale = 0f;

        TouchScreenInput.SetActive(false);
        SetCharacterActive(false);
        var rankings = GetRank();
        OnRankingUpdate?.Invoke(rankings);
        OnGameEnd?.Invoke();

        Debug.Log("Game Ended");

        for (int i = 0; i < rankings.Count; i++)
        {
            Debug.Log($"{i + 1}. {rankings[i].Name} - {rankings[i].Coins} coins");
        }
        
    }
    public void OnExitButton()
    {
        ResetGame();
        SceneManager.LoadScene("City1");

        //ShowMenu();
    }
    void ResetALLCoins()
    {
        player?.GetComponent<CoinCollector>()?.ResetCoins();
        foreach(var bot in bots)
        {
            bot?.GetComponent<CoinCollector>()?.ResetCoins();
        }
    }
    void ResetGame()
    {
        if (player != null)
        {
            player.GetComponent<CoinCollector>()?.ResetCoins();
            player.GetComponent<Character>()?.ResetHealth();
        }
        foreach (var bot in bots)
        {
            if (bot != null)
            {
                bot.GetComponent<CoinCollector>()?.ResetCoins();
                bot.GetComponent<Character>()?.ResetHealth();
            }
        }
    }
    void SetCharacterActive(bool isActive)
    {
        if (player != null)
        {
            var controller = player.GetComponent<StarterAssets.ThirdPersonController>();
            if (controller != null)
            {
                controller.enabled = isActive;
            }
        }
        foreach (var bot in bots)
        {
            if (bot != null)
            {
                var ai = bot.GetComponent<BotAI>();
                if (ai != null)
                {
                    ai.enabled = isActive;
                }

                var agent = bot.GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (agent != null)
                {
                    agent.enabled = isActive;
                }
            }
        }
    }
    void UpdateRank()
    {
        var rankings = GetRank();
        OnRankingUpdate?.Invoke(rankings);
    }
    public List<RankEntry> GetRank()
    {
        List<RankEntry> rankList = new List<RankEntry>();
        if (player != null)
        {
            var collector = player.GetComponent<CoinCollector>();
            rankList.Add(new RankEntry()
            {
                Name = "You",
                Coins = collector != null ? collector.CurrentCoins : 0,
                IsPlayer = true
            });
        }

        foreach (var bot in bots)
        {
            if (bot == null)
            {
                continue;
            }
            var collector = bot.GetComponent<CoinCollector>();
            rankList.Add(new RankEntry()
            {
                Name = bot.BotName,
                Coins = collector != null ? collector.CurrentCoins : 0,
                IsPlayer = false
            });
        }


        return rankList.OrderByDescending(e=>e.Coins).ToList();
    }
    public int GetPlayerRank()
    {
        var rankList = GetRank();
        for(int i=0;i< rankList.Count;i++)
        {
            if(rankList[i].IsPlayer)
            {
                return i + 1;
            }
        }
        return -1;
    }
}

[System.Serializable]
public class  RankEntry
{
    public string Name;
    public int Coins;
    public bool IsPlayer;
}
