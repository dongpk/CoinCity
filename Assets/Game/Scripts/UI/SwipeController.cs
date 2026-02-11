using UnityEngine;

public class SwipeController : MonoBehaviour
{
    [SerializeField] int maxPages = 2;
    [SerializeField] Vector3 pageStep;
    [SerializeField] RectTransform pageRect;
    [SerializeField] float tweenTime;
    [SerializeField] LeanTweenType tweenType;
    
    int currentPage;
    Vector3 targetPosition;

    private void Awake()
    {
        currentPage = 1; // ✅ 1 = Play Page (phải), 2 = Skin Page (trái)
        targetPosition = pageRect.localPosition;
        Debug.Log($"Start at Page {currentPage}, Target: {targetPosition}");
    }

    // ✅ Gọi khi nhấn nút "Skin" → chuyển sang Skin Page (trái)
    public void GoToNextPage()
    {
        if(currentPage < maxPages)
        {
            currentPage++;
            targetPosition += pageStep; // pageStep.x = -978 → đi trái
            Debug.Log($"Go to Skin: Page {currentPage}, Target: {targetPosition}");
            MovePage();
        }
    }

    // ✅ Gọi khi nhấn nút "Play" → chuyển về Play Page (phải)
    public void GoToPreviousPage()
    {
        if (currentPage > 1)
        {
            currentPage--;
            targetPosition -= pageStep; // Trừ -978 = +978 → đi phải
            Debug.Log($"Go to Play: Page {currentPage}, Target: {targetPosition}");
            MovePage();
        }
    }

    void MovePage()
    {
        pageRect.LeanMoveLocal(targetPosition, tweenTime).setEase(tweenType);
    }
}
