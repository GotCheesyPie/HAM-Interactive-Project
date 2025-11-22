using UnityEngine;
using TMPro;

public class OpinionCardUI : MonoBehaviour
{
    [Header("UI References (Seret dari Hierarchy)")]
    public TextMeshProUGUI topicText;      // Untuk "TopikText"
    public TextMeshProUGUI opinionText;    // Untuk "OpinionText"
    public TextMeshProUGUI nameAgeText;    // Untuk "NameAgeText"
    public TextMeshProUGUI cityText;       // Untuk "CityText"

    // Fungsi ini dipanggil oleh Manager untuk mengisi data
    public void SetData(Opinion op)
    {
        topicText.text = op.topicID;
        opinionText.text = op.opinionText;
        nameAgeText.text = $"{op.authorName} ({op.authorAge} Tahun)";
        cityText.text = $"Dari {op.authorCity}";
    }
}