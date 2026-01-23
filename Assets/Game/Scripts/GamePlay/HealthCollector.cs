using StarterAssets;
using UnityEngine;

public class HealthCollector : MonoBehaviour
{
    [SerializeField]int plusHealth = 30;

    Character character;
    ThirdPersonController controller;


    private void Start()
    {
        character  = GetComponent<Character>();
        controller = GetComponent<ThirdPersonController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Health"))
        {
           
            if(character != null && character.IsAlive)
            {
                character.Heal(plusHealth);
                VFXManager.Instance.PlayHealthVFX(this.gameObject.transform.position);
                controller?.healthCollected?.Invoke();
                Debug.Log($"{other.name} Collected! +{plusHealth} Health");
                
            }
        }
    }
}
