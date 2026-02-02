using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OffScreenTargetIndicator : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject indicatorPrefab;
    [SerializeField] Canvas uiCanvas;

    [Header("Settings")]
    [SerializeField] float circleRadius = 150f; // ✅ Bán kính vòng tròn xung quanh player
    [SerializeField] Color indicatorColor = Color.red;
    [SerializeField] Transform playerTransform;

    private Camera mainCamera;

    private Dictionary<Bot,BotIndicator> botIndicators = new Dictionary<Bot,BotIndicator>();

    private class BotIndicator
    {
        public GameObject indicator;
        public RectTransform rectTransform;
        public Image arrowImage;
        public TextMeshProUGUI coinText;
        public CoinCollector coinCollector;
        public Transform botTransform;
        
    }

    private void Start()
    {
        mainCamera = Camera.main;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStart.AddListener(InitializeBotIndicators);
            GameManager.Instance.OnGameEnd.AddListener(CleanIndicators);
        }
    }
    void InitializeBotIndicators()
    {
        CleanIndicators();

        Bot[] bots = FindObjectsByType<Bot>(FindObjectsSortMode.None);

        foreach (Bot bot in bots)
        {
            if (bot == null)
            {
                continue;
            }

            GameObject indicator = Instantiate(indicatorPrefab, uiCanvas.transform);
            indicator.SetActive(false);

            BotIndicator botIndicator = new BotIndicator
            {
                indicator = indicator,
                rectTransform = indicator.GetComponent<RectTransform>(),
                arrowImage = indicator.GetComponent<Image>(),
                coinText  = indicator.GetComponentInChildren<TextMeshProUGUI>(),
                coinCollector = bot.GetComponent<CoinCollector>(),
                botTransform = bot.transform
            };

            if(botIndicator.arrowImage != null)
            {
                botIndicator.arrowImage.color = indicatorColor;
            }

            botIndicators.Add(bot, botIndicator);
        }
    }
    private void LateUpdate()
    {
        if(mainCamera== null|| playerTransform== null)
        {
       
            return;
        }

        if(GameManager.Instance?.CurrentGameState != GameState.Playing)
        {
            HideAllIndicators();
            return;
        }

        foreach (var i in botIndicators)
        {
            Bot bot = i.Key;
            BotIndicator indicator = i.Value;

            if (bot == null || !bot.gameObject.activeInHierarchy || !bot.IsAlive)
            {
                indicator.indicator.SetActive(false);
                continue;
            }
            UpdateBotIndicator(indicator);
        }
    }
    void UpdateBotIndicator(BotIndicator i) 
    {
        // ✅ Lấy vị trí player và bot trên màn hình
        Vector3 playerScreenPos = mainCamera.WorldToScreenPoint(playerTransform.position);
        Vector3 botScreenPos = mainCamera.WorldToScreenPoint(i.botTransform.position);

        // ✅ Kiểm tra bot có nằm ngoài tầm nhìn không
        bool isOffScreen = botScreenPos.x < 0 || botScreenPos.x > Screen.width ||
                           botScreenPos.y < 0 || botScreenPos.y > Screen.height ||
                           botScreenPos.z < 0;

        // ✅ Tính khoảng cách giữa player và bot trên màn hình
        Vector2 directionToBot = new Vector2(botScreenPos.x - playerScreenPos.x, 
                                             botScreenPos.y - playerScreenPos.y);
        float distanceToBot = directionToBot.magnitude;

        // ✅ Chỉ hiển thị khi bot nằm ngoài vòng tròn hoặc ngoài màn hình
        if (isOffScreen )
        {
            i.indicator.SetActive(true);

                // ✅ Normalize hướng và đặt indicator ở rìa vòng tròn
            Vector2 normalizedDirection = directionToBot.normalized;
            Vector2 indicatorPosition = new Vector2(playerScreenPos.x, playerScreenPos.y) 
                                       + normalizedDirection * circleRadius;

            i.rectTransform.position = indicatorPosition;

            // ✅ Quay mũi tên hướng về bot
            float angle = Mathf.Atan2(normalizedDirection.y, normalizedDirection.x) * Mathf.Rad2Deg;
            i.rectTransform.rotation = Quaternion.Euler(0, 0, angle - 90);

            // ✅ Cập nhật số coin
            if(i.coinText!= null && i.coinCollector != null)
            {
                i.coinText.text = i.coinCollector.CurrentCoins.ToString();
            }
        }
        else
        {
            i.indicator.SetActive(false);
        }
    }
    void HideAllIndicators()
    {
        foreach (var i in botIndicators.Values)
        {
            if (i.indicator != null)
            {
                i.indicator.SetActive(false);
            }
        }
    }
    private void CleanIndicators()
    {
        foreach (var i in botIndicators.Values)
        {
            if (i.indicator != null)
            {
                Destroy(i.indicator);
            }
        }
        botIndicators.Clear();
    }
}
