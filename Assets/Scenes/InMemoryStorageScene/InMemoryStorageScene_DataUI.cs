using TMPro;
using UnityEngine;

public class InMemoryStorageScene_DataUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Id;
    [SerializeField] TextMeshProUGUI Name;
    [SerializeField] TextMeshProUGUI Age;
    [SerializeField] TextMeshProUGUI City;
    [SerializeField] TextMeshProUGUI AvatarId;
    void Update()
    {
        if (EphemeralDataManager.Instance == null) { return; }
        Id.text = EphemeralDataManager.Instance.playerIdData.PlayerId.ToString();
        Name.text = EphemeralDataManager.Instance.playerIdData.Name;
        Age.text = EphemeralDataManager.Instance.playerIdData.Age.ToString();
        City.text = EphemeralDataManager.Instance.playerIdData.City;
        AvatarId.text = EphemeralDataManager.Instance.playerIdData.AvatarId.ToString();
    }
}
