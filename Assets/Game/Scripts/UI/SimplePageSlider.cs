using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SimplePageSlider : MonoBehaviour
{
    [Header("References")]
    public ScrollRect scrollRect;
    public Button[] navButtons;

    [Header("Settings")]
    public float slideDuration = 0.35f;
    public Ease slideEase = Ease.OutCubic;
    public int defaultPageIndex = 0;

    private int totalPages;
    private int currentPageIndex = 0;

    void Start()
    {
        totalPages = scrollRect.content.childCount;
        GoToPage(defaultPageIndex, false);
    }

    public void GoToPlayPage()
    {
        //Debug.Log("GoToPlayPage() được gọi!");
        GoToPage(0);
    }

    public void GoToSkinPage()
    {
        //Debug.Log("GoToSkinPage() được gọi!");
        GoToPage(1);
    }

    public void GoToPage(int pageIndex, bool playAnimation = true)
    {
        currentPageIndex = Mathf.Clamp(pageIndex, 0, totalPages - 1);

        float targetNormalizedPos = totalPages > 1 
            ? (float)currentPageIndex / (totalPages - 1) 
            : 0f;

        DOTween.Kill(scrollRect);

        if (playAnimation)
        {
            // s.SetUpdate(true) để DOTween chạy khi timeScale = 0
            scrollRect.DOHorizontalNormalizedPos(targetNormalizedPos, slideDuration)
                      .SetEase(slideEase)
                      .SetId(scrollRect)
                      .SetUpdate(true);
        }
        else
        {
            scrollRect.horizontalNormalizedPosition = targetNormalizedPos;
        }

        UpdateButtonVisuals();
        //Debug.Log($"Chuyển trang {currentPageIndex + 1}/{totalPages}, target: {targetNormalizedPos}");
    }

    private void UpdateButtonVisuals()
    {
        for (int i = 0; i < navButtons.Length; i++)
        {
            Image btnImage = navButtons[i].GetComponent<Image>();
            if (i == currentPageIndex)
            {
                btnImage.color = Color.white;
                // ✅ FIX: Thêm .SetUpdate(true)
                navButtons[i].transform.DOScale(1.3f, 0.2f).SetUpdate(true);
            }
            else
            {
                btnImage.color = Color.gray;
                navButtons[i].transform.DOScale(1.0f, 0.2f).SetUpdate(true);
            }
        }
    }
}