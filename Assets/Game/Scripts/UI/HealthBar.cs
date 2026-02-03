using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Health Bar Settings")]
    [SerializeField] Image healthBarSprite;
    [SerializeField] float reduceSpeed = 2f;

    [Space]
    [Header("Floating Text Settings")]
    [SerializeField] GameObject floatingCoinPrefab;
    [SerializeField] RectTransform coinText;

    private float targetFill;
    private Camera cam;
    private void Start()
    {
        cam = Camera.main;
    }
    private void Update()
    {
        //transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position,Vector3.up);
        transform.rotation = Quaternion.identity;
        healthBarSprite.fillAmount = Mathf.MoveTowards(healthBarSprite.fillAmount,
                                                        targetFill,
                                                        reduceSpeed * Time.deltaTime);
    }

    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        targetFill= currentHealth / maxHealth;
    }
    public void ShowCoinGain(int amt, Color color)
    {
        Debug.Log("show coin success");
        if (floatingCoinPrefab == null || coinText == null)
        {
            return;
        }
        GameObject floatText = Instantiate(floatingCoinPrefab, coinText);
        floatText.transform.localPosition = Vector3.zero;

        var temp = floatText.GetComponent<TextMeshProUGUI>();
        if (temp != null)
        {
            temp.text = $"+{amt}";
            temp.color = color;
        }
        StartCoroutine(FloatAndDestroy(floatText));
    }
    IEnumerator FloatAndDestroy(GameObject obj)
    {
        float duration = 1f;
        float elapsed = 0;
        RectTransform rectTransform = obj.GetComponent<RectTransform>();
        Vector3 startPosition = rectTransform.localPosition;
        var temp = obj.GetComponent<TextMeshProUGUI>();
        Color startColor = temp.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float time = elapsed / duration;
            rectTransform.localPosition = startPosition + Vector3.up * (50 * time);
            temp.color = new Color(startColor.r, startColor.g, startColor.b, 1f - time);
            yield return null;
        }
        Destroy(obj);
    }
}
