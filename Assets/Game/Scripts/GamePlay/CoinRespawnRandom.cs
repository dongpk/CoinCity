using UnityEngine;

public class CoinRespawnRandom : MonoBehaviour
{
    private GameObject[] allRoads;

    private void Start()
    {
        allRoads = GameObject.FindGameObjectsWithTag("Road");
        if (allRoads.Length == 0)
        {
            Debug.LogWarning("No roads found with the 'Road' tag at Start.");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RespawnRandomOnRoads();
        }
    }
    void RespawnRandomOnRoads()
    {
       

        if (allRoads.Length == 0 || allRoads==null)
        {
            return;
        }
        int randomIndex = Random.Range(0, allRoads.Length);
        GameObject selectedRoad = allRoads[randomIndex];

        Vector3 newPos = selectedRoad.transform.position;

        

        transform.position = newPos;
    }
}
