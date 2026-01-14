using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    [SerializeField] AudioClip coinCollectSound;


    [Space(10)]
    [SerializeField] AudioSource audioSource;

    public void OnCoinCollected()
    {
        audioSource.PlayOneShot(coinCollectSound);
    }
}
