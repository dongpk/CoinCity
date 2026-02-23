using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] GameObject UiCanvas;       
    [SerializeField] GameObject loadingCanvas;  
    [SerializeField] Slider progressSlider;   

    [Header("Timing Settings")]
    [SerializeField] float minLoadTime = 3f;
    [SerializeField] float maxLoadTime = 5f;

    [Header("Phase Settings")]
    [SerializeField] float fastPhaseEnd = 0.60f;
    [SerializeField] float slowPhaseEnd = 0.90f;

    [Header("Stall Settings")]
    [SerializeField] int stallCount = 2;
    [SerializeField] float stallMinTime = 0.3f;
    [SerializeField] float stallMaxTime = 0.8f;

    [Header("Smoothing")]
    [SerializeField] float smoothSpeed = 8f;

    [Header("Easing Curves")]
    [SerializeField] AnimationCurve fastCurve   = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] AnimationCurve slowCurve   = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] AnimationCurve finishCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private float _targetProgress    = 0f;
    private float _displayedProgress = 0f;
    private bool  _isLoading         = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

       
        DontDestroyOnLoad(gameObject);

       
        if (UiCanvas != null) UiCanvas.SetActive(false);

      
        if (loadingCanvas != null) loadingCanvas.SetActive(true);

        if (progressSlider != null)
        {
            progressSlider.minValue = 0f;
            progressSlider.maxValue = 1f;
            progressSlider.value    = 0f;
        }
    }

    private void Start()
    {
        StartFakeLoading();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var uiManager = GameObject.Find("UI Manager");
        if (uiManager != null)
        {
            UiCanvas = uiManager;
            UiCanvas.SetActive(false);
        }
    }

    private void Update()
    {
        if (!_isLoading) return;

        _displayedProgress = Mathf.Lerp(
            _displayedProgress,
            _targetProgress,
            Time.unscaledDeltaTime * smoothSpeed
        );

        ApplyToUI(_displayedProgress);
    }

    public void StartFakeLoading(string sceneName = null)
    {
        if (_isLoading)
        {
            Debug.LogWarning("[LoadingScreen] Đang loading, bỏ qua.");
            return;
        }
        StartCoroutine(FakeLoadRoutine(sceneName));
    }

    private IEnumerator FakeLoadRoutine(string sceneName)
    {
        _isLoading         = true;
        _targetProgress    = 0f;
        _displayedProgress = 0f;

        if (loadingCanvas != null) loadingCanvas.SetActive(true);
        if (UiCanvas != null)      UiCanvas.SetActive(false);

        ApplyToUI(0f);
        yield return null;

        float totalTime  = Random.Range(minLoadTime, maxLoadTime);
        float fastTime   = totalTime * 0.35f;
        float slowTime   = totalTime * 0.40f;
        float finishTime = totalTime * 0.25f;

        yield return RunPhase(0f,           fastPhaseEnd, fastTime,   fastCurve);
        yield return RunPhase(fastPhaseEnd, slowPhaseEnd, slowTime,   slowCurve, stallCount);

        AsyncOperation sceneOp = null;
        if (!string.IsNullOrEmpty(sceneName))
        {
            sceneOp = SceneManager.LoadSceneAsync(sceneName);
            if (sceneOp != null)
                sceneOp.allowSceneActivation = false;
            else
                Debug.LogError($"[LoadingScreen] Không tìm thấy scene: '{sceneName}'");
        }

        yield return RunPhase(slowPhaseEnd, 1f, finishTime, finishCurve);

        _targetProgress = 1f;
        yield return new WaitUntil(() => _displayedProgress >= 0.99f);
        ApplyToUI(1f);

        if (sceneOp != null)
        {
            sceneOp.allowSceneActivation = true;

            yield return new WaitUntil(() => sceneOp.isDone);
            yield return new WaitForSecondsRealtime(0.1f);

            if (loadingCanvas != null) loadingCanvas.SetActive(false);
        }
        else
        {
            yield return new WaitForSecondsRealtime(0.3f);
            if (loadingCanvas != null) loadingCanvas.SetActive(false);
            if (UiCanvas != null)      UiCanvas.SetActive(true);
        }

        _isLoading = false;
        Debug.Log("[LoadingScreen] Hoàn tất!");
    }

    private IEnumerator RunPhase(float startVal, float endVal, float duration,
                                  AnimationCurve curve, int stalls = 0)
    {
        float[] stallPoints = GenerateStallPoints(stalls);
        int   stallIndex  = 0;
        float elapsed     = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            _targetProgress = Mathf.Lerp(startVal, endVal, curve.Evaluate(t));

            if (stallIndex < stallPoints.Length && t >= stallPoints[stallIndex])
            {
                stallIndex++;
                float stallDuration = Random.Range(stallMinTime, stallMaxTime);
                float stallProgress = _targetProgress;
                float stallElapsed  = 0f;

                while (stallElapsed < stallDuration)
                {
                    stallElapsed    += Time.unscaledDeltaTime;
                    _targetProgress  = stallProgress + Mathf.Sin(stallElapsed * 8f) * 0.005f;
                    yield return null;
                }
                _targetProgress = stallProgress;
            }
            yield return null;
        }
        _targetProgress = endVal;
    }

    private float[] GenerateStallPoints(int count)
    {
        if (count <= 0) return new float[0];
        float[] points = new float[count];
        for (int i = 0; i < count; i++)
        {
            float segment = 1f / (count + 1f);
            points[i] = Mathf.Clamp(
                segment * (i + 1) + Random.Range(-segment * 0.3f, segment * 0.3f),
                0.1f, 0.9f
            );
        }
        System.Array.Sort(points);
        return points;
    }

    private void ApplyToUI(float value)
    {
        if (progressSlider != null)
            progressSlider.value = value;
    }
}