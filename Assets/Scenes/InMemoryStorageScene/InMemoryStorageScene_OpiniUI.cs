using TMPro;
using UnityEngine;

public class InMemoryStorageScene_OpiniUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Author;
    [SerializeField] TextMeshProUGUI OpinionText;
    public void DisplayOpinion(Opinion opinion)
    {
        Author.text = $"{opinion.authorName}, {opinion.authorAge}, {opinion.authorCity}";
        OpinionText.text = opinion.opinionText;
    }
}
