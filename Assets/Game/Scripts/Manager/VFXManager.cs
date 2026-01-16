using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;

    [SerializeField] GameObject coinVfxPrefab;
    [SerializeField] int poolSize = 10;

    private List<GameObject> poolList = new List<GameObject>();

    private void Awake()
    {
        if(Instance== null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        PreparePool();
    }

    // Tạo và chuẩn bị pool các hiệu ứng VFX
    void PreparePool()
    {
        for(int i=0; i< poolSize; i++)
        {
            CreateVFX();    
        }
    }
    GameObject CreateVFX()
    {
        GameObject vfx = Instantiate(coinVfxPrefab,transform);
        vfx.SetActive(false);
        poolList.Add(vfx);
        return vfx;
    }

    public void PlayCoinVFX(Vector3 pos)
    {
        GameObject vfxToPlay = null;

        foreach(var vfx in poolList)
        {
            if(!vfx.activeInHierarchy)
            {
                vfxToPlay = vfx;
                break;
            }
        }

        if(vfxToPlay == null)
        {
            vfxToPlay = CreateVFX();
        }

        vfxToPlay.transform.position = pos;
        vfxToPlay.SetActive(true);

        ParticleSystem ps = vfxToPlay.GetComponent<ParticleSystem>();
        if (ps != null) ps.Play();
    }

}
