using UnityEngine;

public class CoinCreate : MonoBehaviour
{
    [SerializeField] GameObject coinPrefab;
    [SerializeField] int minCoin = 1;
    [SerializeField] int maxCoin = 5;

    private void Start()
    {
        // Reset danh sách vị trí khi bắt đầu game
        CoinRespawnRandom.ClearAllOccupiedPositions();
        CreateRandomNumCoin();
    }

    void CreateRandomNumCoin()
    {
        int coinCount = Random.Range(minCoin, maxCoin + 1);

        for (int i = 0; i < coinCount; i++)
        {
            GameObject coin = Instantiate(coinPrefab, Vector3.zero, Quaternion.identity);
            CoinRespawnRandom respawner = coin.GetComponent<CoinRespawnRandom>();
            
            // Lấy component và spawn tại vị trí hợp lệ
            if (respawner != null)
            {
                respawner.RespawnRandomOnRoads();
            }
        }
    }
}
