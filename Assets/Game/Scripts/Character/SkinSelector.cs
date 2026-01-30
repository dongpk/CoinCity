using UnityEngine;

public class SkinSelector : MonoBehaviour
{
    [SerializeField] GameObject[] skins;
    int currentSkinIndex = -1;

    private void Awake()
    {
        if (skins == null || skins.Length == 0)
        {
            Debug.LogWarning($"{name}: No skins assigned in SkinSelector!");
            return;
        }
        
        HideAllSkins();
        
        // Hiện skin đầu tiên mặc định để không bị ẩn hoàn toàn
        SetSkin(0);
    }

    public void SetSkin(int index)
    {
        if (skins == null || skins.Length == 0) 
        {
            Debug.LogWarning($"{name}: Cannot set skin - skins array is empty!");
            return;
        }

        index = Mathf.Clamp(index, 0, skins.Length - 1);
        HideAllSkins();
        skins[index].SetActive(true);
        currentSkinIndex = index;
    }

    public void SetRandomSkin()
    {
        if (skins == null || skins.Length == 0) 
        {
            Debug.LogWarning($"{name}: Cannot set random skin - skins array is empty!");
            return;
        }
        int randomIndex = Random.Range(0, skins.Length);
        SetSkin(randomIndex);
    }

    public void NextSkin()
    {
        int nextSkinIndex = (currentSkinIndex +1) % skins.Length;
        SetSkin(nextSkinIndex);
    }

    public void PrevSkin()
    {
        int prevSkinIndex = currentSkinIndex - 1;
        if (prevSkinIndex < 0)
        {
            prevSkinIndex = skins.Length - 1;
        }
        SetSkin(prevSkinIndex);
    }

    void HideAllSkins()
    {
        foreach (GameObject skin in skins)
        {
            if (skin != null)
            {
                skin.SetActive(false);
            }
        }
    }

    public int GetCurrentSkinIndex() => currentSkinIndex;
    public int GetSkinCount() => skins != null ? skins.Length : 0;
}
    