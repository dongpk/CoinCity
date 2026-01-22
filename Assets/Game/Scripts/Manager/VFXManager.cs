using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public enum VFXType
{
    Coin,
    FireAttack,
    IceAttack,
    DefaultAttack
}
public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;

    [Header("VFX Prefabs")]
    [SerializeField] GameObject coinVfxPrefab;
    [SerializeField] GameObject fireAtkVfxPrefab;
    [SerializeField] GameObject iceAtkVfxPrefab;
    [SerializeField] GameObject defaultAtkVfxPrefab;

    [Space]
    [Header("Pool Settings")]
    [SerializeField] int poolSize = 10;

    private Dictionary<VFXType,List<GameObject>> poolDict = new Dictionary<VFXType, List<GameObject>>();
    private Dictionary<VFXType, GameObject> prefabDict = new Dictionary<VFXType, GameObject>();
    private Dictionary<GameObject, List<GameObject>> customPrefabPool = new Dictionary<GameObject, List<GameObject>>();


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
        InitializePrefabDict();
        PreparePool();
    }

    void InitializePrefabDict()
    {
        prefabDict[VFXType.Coin] = coinVfxPrefab;
        prefabDict[VFXType.FireAttack] = fireAtkVfxPrefab;
        prefabDict[VFXType.IceAttack] = iceAtkVfxPrefab;
        prefabDict[VFXType.DefaultAttack] = defaultAtkVfxPrefab;
    }

    // Tạo và chuẩn bị pool các hiệu ứng VFX
    void PreparePool()
    {
        foreach(VFXType type in System.Enum.GetValues(typeof(VFXType)))
        {
            poolDict[type] = new List<GameObject>();
            for (int i = 0; i < poolSize; i++)
            {
                CreateVFX(type);
            }
        }
    }
    GameObject CreateVFX(VFXType type)
    {
        if (prefabDict[type] == null)
        {
            Debug.LogError($"Prefab for VFXType {type} is not assigned.");
            return null;
        }
        GameObject vfx = Instantiate(prefabDict[type],transform);
        vfx.SetActive(false);
        poolDict[type].Add(vfx);
        return vfx;
    }
    GameObject CreateCustomVFX(GameObject prefab)
    {
        if (prefab == null) return null;

        GameObject vfx = Instantiate(prefab, transform);
        vfx.SetActive(false);

        if (!customPrefabPool.ContainsKey(prefab))
        {
            customPrefabPool[prefab] = new List<GameObject>();
        }
        customPrefabPool[prefab].Add(vfx);
        return vfx;
    }

    // Phát hiệu ứng VFX tại vị trí cố định
    public void PlayVFX(VFXType type, Vector3 pos)
    {
        GameObject vfxToPlay = GetVFXFromPool(type);

        if (vfxToPlay == null)
        {
            return;
        }

        vfxToPlay.transform.SetParent(transform);
        vfxToPlay.transform.position = pos;
        //vfxToPlay.transform.rotation = Quaternion.identity;
        ActivateVFX(vfxToPlay);
    }

    // Phát hiệu ứng VFX theo đối tượng
    public void PlayVFXFollow(VFXType type, Transform followTarget, Vector3 offset = default)
    {
        GameObject vfxToPlay = GetVFXFromPool(type);

        if (vfxToPlay == null)
        {
            return;
        }
         vfxToPlay.transform.SetParent(followTarget);
        vfxToPlay.transform.localPosition = offset;
        //vfxToPlay.transform.localRotation = Quaternion.identity;

        ActivateVFX(vfxToPlay);
    }
    void ActivateVFX(GameObject vfx)
    {
        vfx.SetActive(true);
        ParticleSystem ps = vfx.GetComponent<ParticleSystem>();
        if (ps != null) ps.Play();

    }
    GameObject GetVFXFromPool(VFXType type)
    {
        foreach (var vfx in poolDict[type])
        {
            if (!vfx.activeInHierarchy)
            {
                return vfx;
            }
        }
        return CreateVFX(type);
    }
    
    public void PlayCoinVFX(Vector3 pos) => PlayVFX(VFXType.Coin, pos);
    public void PlayFireAtkVFX(Vector3 pos) => PlayVFX(VFXType.FireAttack, pos);    
    public void PlayIceAtkVFX(Vector3 pos) => PlayVFX(VFXType.IceAttack, pos);
    public void PlayDefaultAtkVFX(Vector3 pos) => PlayVFX(VFXType.DefaultAttack, pos);

    public void PlayFireAtkVFXFollow(Transform target,Vector3 offset = default) 
        => PlayVFXFollow(VFXType.FireAttack, target, offset);
    public void PlayIceAtkVFXFollow(Transform target,Vector3 offset = default) 
        => PlayVFXFollow(VFXType.IceAttack, target, offset);
    public void PlayDefaultAtkVFXFollow(Transform target,Vector3 offset = default) 
        => PlayVFXFollow(VFXType.DefaultAttack, target, offset);

    /// <summary>
    /// for bot
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="pos"></param>
    public void PlayVFX(GameObject prefab, Vector3 pos)
    {
        if (prefab == null) return;

        GameObject vfxToPlay = GetCustomVFXFromPool(prefab);
        if (vfxToPlay == null) return;

        vfxToPlay.transform.SetParent(transform);
        vfxToPlay.transform.position = pos;
        //vfxToPlay.transform.localRotation = Quaternion.identity;
        ActivateVFX(vfxToPlay);
    }
    GameObject GetCustomVFXFromPool(GameObject prefab)
    {
        if (customPrefabPool.TryGetValue(prefab, out var pool))
        {
            foreach (var vfx in pool)
            {
                if (!vfx.activeInHierarchy)
                {
                    return vfx;
                }
            }
        }
        return CreateCustomVFX(prefab);
    }
    public void PlayVFXFollow(GameObject prefab, Transform followTarget, Vector3 offset = default)
    {
        if (prefab == null) return;

        GameObject vfxToPlay = GetCustomVFXFromPool(prefab);
        if (vfxToPlay == null) return;

        vfxToPlay.transform.SetParent(followTarget);
        vfxToPlay.transform.localPosition = offset;
        //vfxToPlay.transform.localRotation = Quaternion.identity;
        ActivateVFX(vfxToPlay);
    }


}
