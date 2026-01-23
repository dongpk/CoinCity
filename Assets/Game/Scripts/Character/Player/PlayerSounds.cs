using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    [SerializeField] AudioClip coinCollectSound;
    [SerializeField] AudioClip healthCollectSound;
    [SerializeField] AudioClip attackSound;
    [SerializeField] AudioClip deathSound;


    [Space(10)]
    [SerializeField] AudioSource audioSource;

    public void OnCoinCollected()
    {
        audioSource.PlayOneShot(coinCollectSound);
    }
    public void OnHealthCollected()
    {
        audioSource.PlayOneShot(healthCollectSound);
    }
    public void OnAttack()
    {
        audioSource.PlayOneShot(attackSound);
    }
    public void OnDeath()
    {
        audioSource.PlayOneShot(deathSound);
    }
}
