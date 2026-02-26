using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioSource sfxSource;

    [Space]
    [Header("Audio Clips")]
    [SerializeField] AudioClip bgmClip;
    [SerializeField] AudioClip touchSfxClip;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    [SerializeField] float bgmVolume = 0.5f;

    [Range(0f, 1f)]
    [SerializeField] float sfxVolume = 1f;

    private void Start()
    {
        PlayBGM();
    }

    private void Update()
    {
        if (IsTouchOrClick())
        {
            PlayTouchSFX();
        }
    }

    private bool IsTouchOrClick()
    {
        // ✅ Mobile: ưu tiên kiểm tra Touch trước
        if (Input.touchCount > 0)
        {
            return Input.GetTouch(0).phase == TouchPhase.Began;
        }

        // ✅ Editor/PC: GetMouseButtonDown = chỉ true 1 frame khi nhấn
        return Input.GetMouseButtonDown(0);
    }

    private void PlayBGM()
    {
        if (bgmSource == null || bgmClip == null)
        {
            Debug.LogWarning("bgmsource hoac bgm chua dc gan");
            return;
        }
        bgmSource.clip   = bgmClip;
        bgmSource.volume = bgmVolume;
        bgmSource.loop   = true;
        bgmSource.Play();
    }

    private void PlayTouchSFX()
    {
        if (sfxSource == null || touchSfxClip == null)
        {
            return;
        }
        sfxSource.PlayOneShot(touchSfxClip, sfxVolume);
    }

    public void OnPlayButtonClicked()
    {
        if (bgmSource != null && bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }
        PlayTouchSFX();
    }
}
