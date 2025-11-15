using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InMemoryStorageScene_ChoiceUI : MonoBehaviour
{
    [SerializeField] GameObject Opini;
    [SerializeField] Image Background;
    [SerializeField] TextMeshProUGUI ChoiceText;

    public void DisplayChoice(Opinion opinion, bool agree)
    {
        Opini.GetComponent<InMemoryStorageScene_OpiniUI>().DisplayOpinion(opinion);
        if (agree)
        {
            Background.color = Color.green;
            ChoiceText.text = "Agree";
        }
        else
        {
            Background.color = Color.red;
            ChoiceText.text = "Disagree";
        }
    }
}