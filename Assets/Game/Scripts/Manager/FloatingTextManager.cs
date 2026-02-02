using UnityEngine;
using System.Collections.Generic;

public class FloatingTextManager : MonoBehaviour
{
    public static FloatingTextManager Instance { get; private set; }

    [SerializeField] GameObject plusCoinPrefab;
    [SerializeField] int poolSize = 10;
    [SerializeField] Canvas parentCanvas;
    [SerializeField] RectTransform coinDisplayText;  // ✅ Thêm reference tới Count Coin text

    private List<FloatingTextVFX> pool = new List<FloatingTextVFX>();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (parentCanvas == null)
        {
            parentCanvas = FindObjectsByType<Canvas>(FindObjectsSortMode.None)[0];
        }

        InitializePool();
    }

    void InitializePool()
    {
        for(int i = 0; i < poolSize; i++)
        {
            CreateFloatingTextVFX();
        }
    }

    FloatingTextVFX CreateFloatingTextVFX()
    {
        GameObject ins = Instantiate(plusCoinPrefab, parentCanvas.transform);
        ins.SetActive(false);
        FloatingTextVFX vfx = ins.GetComponent<FloatingTextVFX>();
        pool.Add(vfx);
        return vfx;
    }

    FloatingTextVFX GetFloatingTextVFXFromPool()
    {
        foreach (var vfx in pool)
        {
            if(!vfx.gameObject.activeInHierarchy)
            {
                return vfx;
            }
        }
        return CreateFloatingTextVFX();
    }

    public void ShowPlusCoin(string text, Vector3 worldPosition, Color color)
    {
        FloatingTextVFX vfx = GetFloatingTextVFXFromPool();
        vfx.gameObject.SetActive(true);
        vfx.ShowText(text, worldPosition, color);
    }

    // ✅ Method mới - hiển thị tại UI position (Count Coin)
    public void ShowPlusCoinAtUI(string text, Color color)
    {
        if (coinDisplayText == null)
        {
            Debug.LogWarning("[FloatingTextManager] coinDisplayText not assigned!");
            return;
        }

        FloatingTextVFX vfx = GetFloatingTextVFXFromPool();
        vfx.gameObject.SetActive(true);
        vfx.ShowTextAtUIPosition(text, coinDisplayText.position, color);
    }

    public void ShowCoinPickup(int coins, Vector3 pos)  
    {
        // ✅ Dùng UI position thay vì world position
        ShowPlusCoinAtUI($"+{coins}", Color.yellow);
    }

    public void ShowCoinSteal(int coins, Vector3 pos)
    {
        // ✅ Dùng UI position thay vì world position
        ShowPlusCoinAtUI($"+{coins}", new Color(1f, 0.5f, 0f)); // Orange
    }
}
