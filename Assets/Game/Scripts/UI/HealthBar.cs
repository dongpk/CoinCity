using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image healthBarSprite;
    [SerializeField] float reduceSpeed = 2f;
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
}
