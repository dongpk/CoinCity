using UnityEngine;

public class Player : Character
{
    private void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            TakeDamage(1);
        }
    }
    protected override void Die()
    {
        Debug.Log("Player has died.");
    }
   

}
