using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingTextVFX : MonoBehaviour
{
    [SerializeField] float moveSpeed = .5f;
    [SerializeField] float fadeDuration = 1f;
     
    private TextMeshProUGUI floatingText;
    private RectTransform rectTransform;
    private Canvas parentCanvas;

    private void Awake()
    {
        floatingText = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
    }

    // ✅ Method gốc - dùng World Position
    public void ShowText(string text, Vector3 position, Color color)
    {
        floatingText.text = text;
        floatingText.color = color;

        if (parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            Vector3 Pos = Camera.main.WorldToScreenPoint(position);
            rectTransform.position = Pos;
        }
        else if (parentCanvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            Vector3 Pos = Camera.main.WorldToScreenPoint(position);
            rectTransform.position = Pos;
        }
        else if (parentCanvas.renderMode == RenderMode.WorldSpace)
        {
            rectTransform.position = position;
        }

        StopAllCoroutines();
        StartCoroutine(FloatAndFade());
    }

    // ✅ Method mới - dùng UI Position trực tiếp
    public void ShowTextAtUIPosition(string text, Vector3 uiPosition, Color color)
    {
        floatingText.text = text;
        floatingText.color = color;

        // Dùng UI position trực tiếp (không cần convert)
        rectTransform.position = uiPosition;

        StopAllCoroutines();
        StartCoroutine(FloatAndFade());
    }

    IEnumerator FloatAndFade()
    {
        float elapse = 0f;
        Vector3 startPosition = rectTransform.position;

        while (elapse < fadeDuration)
        {
            elapse += Time.deltaTime;
            float progress = elapse / fadeDuration;

            Vector3 newPosition = startPosition + Vector3.up * moveSpeed * progress;
            rectTransform.position = newPosition;

            Color newColor = floatingText.color;
            newColor.a = Mathf.Lerp(1f, 0f, progress);
            floatingText.color = newColor;

            yield return null;
        }

        gameObject.SetActive(false);
    }
}
