using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
public class UIManager : MonoBehaviour
{
    [Header("Menu Panel")]
    [SerializeField] GameObject menuPanel;
    [SerializeField] Button playButton;

    [Space]
    [Header("Skin select")]
    [SerializeField] Button prevSkin;
    [SerializeField] Button nextSkin;

    [Space]
    [Header("Countdown")]
    [SerializeField] GameObject countdownPanel;
    [SerializeField] TextMeshProUGUI countdownText;

    [Space]
    [Header("HUD")]
    [SerializeField] GameObject HUD;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] Transform rankingContainer;
    [SerializeField] GameObject rankEntryPrefab;
    [SerializeField] int maxDisplayRank = 6;

    [Space]
    [Header("GameEnd Panel")]
    [SerializeField] GameObject gameEndPanel;
    [SerializeField] GameObject endTextBG;
    [SerializeField] TextMeshProUGUI endText;
    [SerializeField] Transform finalRankingContainer;
    [SerializeField] Button exitButton;

    List<GameObject> rankEntries = new List<GameObject>();
    List<GameObject> finalRankEntries = new List<GameObject>();
    SkinSelector playerSkinSelector;

    private void Awake()
    {
        
    }
    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance is null! Make sure GameManager exists in scene.");
            return;
        }
        var player = GameManager.Instance.GetPlayer();
        if (player != null)
        {
            playerSkinSelector = player.GetComponent<SkinSelector>();
        }
        GameManager.Instance.OnShowMenu.AddListener(ShowMenuPanel);
        GameManager.Instance.OnCountdown.AddListener(ShowCountdown);
        GameManager.Instance.OnGameStart.AddListener(ShowHUD);
        GameManager.Instance.OnGameEnd.AddListener(ShowEndGamePanel);
        GameManager.Instance.OnTimerUpdate.AddListener(UpdateTimer);
        GameManager.Instance.OnRankingUpdate.AddListener(UpdateRanking);

        if (playButton != null)
        {
            playButton.onClick.RemoveListener(OnPlayClick);  // Bỏ listener cũ
            playButton.onClick.AddListener(OnPlayClick);     // Thêm mới
        }

        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(OnExitClick);
            exitButton.onClick.AddListener(OnExitClick);
        }

       

        UpdateSkinUI();
    }

    #region Panel
    public void ShowMenuPanel()
    {
        menuPanel?.SetActive(true);
        countdownPanel?.SetActive(false);
        HUD?.SetActive(false);
        gameEndPanel?.SetActive(false);

        //updateskin
        UpdateSkinUI();
    }

    public void ShowCountdown(int sec)
    {
        menuPanel?.SetActive(false);
        countdownPanel?.SetActive(true);
        HUD?.SetActive(false);
        gameEndPanel?.SetActive(false);

        if (countdownText != null)
        {
            countdownText.text = sec.ToString();

            countdownText.transform.localScale = Vector3.one * 1.5f;
            StartCoroutine(AnimateCountdown(countdownText));
        }
    }
    IEnumerator AnimateCountdown(TextMeshProUGUI text)
    {
        float duration = .3f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float time = elapsed / duration;
            text.transform.localScale = Vector3.Lerp(Vector3.one * 1.5f, Vector3.one, time);
            yield return null;
        }
        text.transform.localScale = Vector3.one;
    }

    public void ShowHUD()
    {
        menuPanel?.SetActive(false);
        countdownPanel?.SetActive(false);
        HUD?.SetActive(true);
        gameEndPanel?.SetActive(false);

        if (timerText != null)
        {
            timerText.color = Color.white;
        }
    }
    public void ShowEndGamePanel()
    {
        menuPanel?.SetActive(false);
        countdownPanel?.SetActive(false);
        HUD?.SetActive(true);
        endTextBG?.SetActive(false);
        gameEndPanel?.SetActive(true);

        //result
        int playerRank = GameManager.Instance.GetPlayerRank();
        if (endText != null)
        {
            if (playerRank == 1)
            {
                endText.text = " YOU WIN! ";
                endText.color = Color.yellow;

            }
            else
            {
                endText.text = $"You finished at #{playerRank}";
                endText.color = Color.black;
            }
        }
        //showfinalranking
        ShowFinalRanking(GameManager.Instance.GetRank());
    }


    #endregion

    #region button handler
    public void OnPlayClick()
    {
        GameManager.Instance.OnClickedPlayButton();
    }
    public void OnExitClick()
    {
        GameManager.Instance.OnExitButton();
    }
    public void SelectPreviousSkin()
    {
        if (playerSkinSelector != null)
        {
            int current = playerSkinSelector.GetCurrentSkinIndex();
            playerSkinSelector.PrevSkin();
            //updateskinui
            UpdateSkinUI();
        }
    }
    public void SelectNextSkin()
    {
        if (playerSkinSelector != null)
        {
            int current = playerSkinSelector.GetCurrentSkinIndex();
            playerSkinSelector.NextSkin();
            //updateskinui
            UpdateSkinUI();
        }
    }
    void UpdateSkinUI()
    {
        if (playerSkinSelector != null)
        {
            int skinIndex = playerSkinSelector.GetCurrentSkinIndex();

        }
    }


    #endregion

    #region ranking & timer
    public void UpdateTimer(float timeRemaing)
    {
        if (timerText == null)
        {
            return;
        }
        int min = Mathf.FloorToInt(timeRemaing / 60);
        int sec = Mathf.FloorToInt(timeRemaing % 60);
        timerText.text = $"{min:00}:{sec:00}";

        if (timeRemaing <= 10)
        {
            timerText.color = Color.red;
        }else
        {
            timerText.color = Color.white;
        }

    }
    public void UpdateRanking(List<RankEntry> ranking)
    {
        if (rankingContainer == null || rankEntryPrefab == null)
        {
            return;
        }

        foreach (var entry in rankEntries)
        {
            Destroy(entry);
        }
        rankEntries.Clear();
        int displayCount = Mathf.Min(ranking.Count, maxDisplayRank);
        for (int i = 0; i < displayCount; i++)
        {
            var goEntry = Instantiate(rankEntryPrefab, rankingContainer);
            var text = goEntry.GetComponent<TextMeshProUGUI>();

            if (text != null)
            {
                text.text = $"{i + 1} {ranking[i].Name}: {ranking[i].Coins}";
                text.color = ranking[i].IsPlayer ? Color.yellow : Color.white;
            }
            rankEntries.Add(goEntry);
        }



    }

    void ShowFinalRanking(List<RankEntry> ranking)
    {
        if (finalRankingContainer == null || rankEntryPrefab == null)
        {
            return;
        }
        foreach (var entry in finalRankEntries)
        {
            Destroy(entry);
        }
        finalRankEntries.Clear();

        for (int i = 0; i < ranking.Count; i++)
        {
            var goEntry = Instantiate(rankEntryPrefab, finalRankingContainer);
            var text = goEntry.GetComponent<TextMeshProUGUI>();

            if (text != null)
            {
                text.text = $"{i + 1} {ranking[i].Name}: {ranking[i].Coins}";
                text.color = ranking[i].IsPlayer ? Color.yellow : Color.white;
                text.fontSize = ranking[i].IsPlayer ? 50:45;
                //text.fontSize = 50;
            }

            finalRankEntries.Add(goEntry);
        }
    }



    #endregion

    private void OnDestroy()
    {
        if (playButton != null)
            playButton.onClick.RemoveListener(OnPlayClick);
        if (exitButton != null)
            exitButton.onClick.RemoveListener(OnExitClick);
       
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnShowMenu.RemoveListener(ShowMenuPanel);
            GameManager.Instance.OnCountdown.RemoveListener(ShowCountdown);
            GameManager.Instance.OnGameStart.RemoveListener(ShowHUD);
            GameManager.Instance.OnGameEnd.RemoveListener(ShowEndGamePanel);
            GameManager.Instance.OnTimerUpdate.RemoveListener(UpdateTimer);
            GameManager.Instance.OnRankingUpdate.RemoveListener(UpdateRanking);
        }
    }
}
