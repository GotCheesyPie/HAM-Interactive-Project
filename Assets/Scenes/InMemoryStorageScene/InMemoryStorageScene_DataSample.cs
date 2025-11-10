using UnityEngine;

public class InMemoryStorageScene_DataSample : MonoBehaviour
{
    string[] NamePool = { "John Doe", "Jane Doe" };
    string[] CityPool = { "Jakarta", "Bandung", "Surabaya" };

    public void GenerateUser()
    {
        if (EphemeralDataManager.Instance == null) { return; }

        EphemeralDataManager.Instance.playerIdData = new(
            Random.Range(0, 100),
            NamePool[Random.Range(0, NamePool.Length)],
            Random.Range(15, 30),
            CityPool[Random.Range(0, CityPool.Length)],
            Random.Range(1, 40));
    }
}
