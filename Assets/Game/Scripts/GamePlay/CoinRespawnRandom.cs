using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinRespawnRandom : MonoBehaviour
{
    private static HashSet<int> occupiedRoadIndexes = new HashSet<int>();
    private static GameObject[] allRoads; // Đổi thành static để dùng chung

    [SerializeField] private float delay = 1f;
    [SerializeField] private int maxAttempts = 10;


    private int currentRoadIndex = -1;

    // Thêm cache trong Start hoặc Awake
    private Collider _collider;
    private Renderer[] _renderers;

    private void Awake()
    {
        _collider = GetComponent<Collider>();           // ✅ Cache 1 lần
        _renderers = GetComponentsInChildren<Renderer>(); // ✅ Cache 1 lần
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")||other.CompareTag("Enemy"))
        {
            
            StartCoroutine(CoinRespawnDelay());            
        }
    }

    IEnumerator CoinRespawnDelay()
    {
        _collider.enabled = false;
        for (int i = 0; i < _renderers.Length; i++) _renderers[i].enabled = false;

        // 2. Giải phóng vị trí cũ
        ReleaseCurrentPosition();

        // 3. Đợi delay
        yield return new WaitForSeconds(delay);

        // 4. Di chuyển đến vị trí mới
        RespawnRandomOnRoads();

        _collider.enabled = true;
        for (int i = 0; i < _renderers.Length; i++) _renderers[i].enabled = true;
    }

    private void ReleaseCurrentPosition()
    {
        if (currentRoadIndex >= 0)
        {
            occupiedRoadIndexes.Remove(currentRoadIndex);
            currentRoadIndex = -1;
        }
    }

    // Đảm bảo allRoads luôn được khởi tạo trước khi sử dụng
    private static void EnsureRoadsInitialized()
    {
        if (allRoads == null || allRoads.Length == 0)
        {
            allRoads = GameObject.FindGameObjectsWithTag("Road");
            if (allRoads.Length == 0)
            {
                Debug.LogWarning("No roads found with the 'Road' tag.");
            }
        }
    }

    public void RespawnRandomOnRoads()
    {
        EnsureRoadsInitialized(); // Khởi tạo nếu chưa có

        if (allRoads == null || allRoads.Length == 0)
        {
            return;
        }

        int newIndex = GetValidRoadIndex();
        if (newIndex >= 0)
        {
            currentRoadIndex = newIndex;
            occupiedRoadIndexes.Add(newIndex);
            transform.position = allRoads[newIndex].transform.position;
        }
    }

    private int GetValidRoadIndex()
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            int randomIndex = Random.Range(0, allRoads.Length);

            if (!occupiedRoadIndexes.Contains(randomIndex))
            {
                return randomIndex;
            }
        }

        Debug.LogWarning("Could not find empty position after max attempts.");
        return Random.Range(0, allRoads.Length);
    }

    private void OnDestroy()
    {
        ReleaseCurrentPosition();
    }

    public static void ClearAllOccupiedPositions()
    {
        occupiedRoadIndexes.Clear();
        allRoads = null; // Reset để tìm lại roads mới
    }
}
