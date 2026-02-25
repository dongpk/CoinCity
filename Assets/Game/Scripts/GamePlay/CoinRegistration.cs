using UnityEngine;

/// <summary>
/// Gán lên mỗi Coin prefab và Health prefab.
/// Tự đăng ký/hủy đăng ký với CharacterRegistry.
/// </summary>
public class CoinRegistration : MonoBehaviour
{
    [SerializeField] bool isHealth = false;

    private void OnEnable()
    {
        if (isHealth) CharacterRegistry.RegisterHealth(transform);
        else          CharacterRegistry.RegisterCoin(transform);
    }

    private void OnDisable()
    {
        if (isHealth) CharacterRegistry.UnregisterHealth(transform);
        else          CharacterRegistry.UnregisterCoin(transform);
    }
}