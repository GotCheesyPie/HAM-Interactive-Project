using System.Linq;
using TMPro;
using UnityEngine;

public class InMemoryStorageScene_DataUI : MonoBehaviour
{
    [Header("Player ID")]
    [SerializeField] TextMeshProUGUI Id;
    [SerializeField] TextMeshProUGUI Name;
    [SerializeField] TextMeshProUGUI Age;
    [SerializeField] TextMeshProUGUI City;
    [SerializeField] TextMeshProUGUI AvatarId;

    [Header("Session Data")]
    [SerializeField] TextMeshProUGUI Flow;
    [SerializeField] TextMeshProUGUI CurrentOpinion;
    [SerializeField] TextMeshProUGUI NOpinion;
    [SerializeField] TextMeshProUGUI OpinionSubmission;
    [SerializeField] GameObject OpiniListItem;
    [SerializeField] GameObject OpiniListParent;
    [SerializeField] GameObject ChoiceListItem;
    [SerializeField] GameObject ChoiceListParent;

    void Awake()
    {
        if (!OpiniListItem.TryGetComponent(out InMemoryStorageScene_OpiniUI _))
        {
            Debug.LogError("There's no OpiniUI component found in prefab!");
        }
    }
    void Update()
    {
        if (PersistenceManager.Instance == null) { return; }

        Id.text = PersistenceManager.Instance.GameData.Player.PlayerId.ToString();
        Name.text = PersistenceManager.Instance.GameData.Player.Name;
        Age.text = PersistenceManager.Instance.GameData.Player.Age.ToString();
        City.text = PersistenceManager.Instance.GameData.Player.City;
        AvatarId.text = PersistenceManager.Instance.GameData.Player.AvatarId.ToString();

        Flow.text = PersistenceManager.Instance.GameData.Session.CurrentFlow.ToString();
        CurrentOpinion.text = PersistenceManager.Instance.GameData.Session.CurrentReviewIndex.ToString();
        NOpinion.text = PersistenceManager.Instance.GameData.Session.OpinionsToReview.Count.ToString();
        OpinionSubmission.text = PersistenceManager.Instance.GameData.Session.OpinionSubmission.opinionText;
    }
    public void DisplayOpinions()
    {
        if (PersistenceManager.Instance == null) { return; }
        foreach (var item in PersistenceManager.Instance.GameData.Session.OpinionsToReview)
        {
            GameObject _listItem = Instantiate(OpiniListItem, OpiniListParent.transform);
            _listItem.GetComponent<InMemoryStorageScene_OpiniUI>().DisplayOpinion(item);
        }
    }

    public void DisplayChoices()
    {
        if (PersistenceManager.Instance == null) { return; }
        SessionData session = PersistenceManager.Instance.GameData.Session;
        foreach (var item in session.Choices.Keys)
        {
            GameObject _listItem = Instantiate(ChoiceListItem, ChoiceListParent.transform);
            _listItem.GetComponent<InMemoryStorageScene_ChoiceUI>().DisplayChoice(
                session.OpinionsToReview.ElementAt(item), session.Choices[item]
            );
        }
    }
}
