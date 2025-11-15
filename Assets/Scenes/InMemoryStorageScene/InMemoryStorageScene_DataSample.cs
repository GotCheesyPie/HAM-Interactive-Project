using UnityEngine;

public class InMemoryStorageScene_DataSample : MonoBehaviour
{
    public PlayerIdData PlayerID;
    public SessionData Session;
    string[] NamePool = { "John Doe", "Jane Doe" };
    string[] CityPool = { "Jakarta", "Bandung", "Surabaya" };
    string[] OpinionPool = {
        "Hotdog adalah sandwich",
        "Bubur wajib diaduk",
        "Sereal harus dituang sebelum susu"
    };
    public void SaveToPersistence() //FIXME this is a temporary solution, not abiding to pub-sub
    {
        if (PersistenceManager.Instance != null)
        {
            PlayerID.Save(ref PersistenceManager.Instance.GameData);
            Session.Save(ref PersistenceManager.Instance.GameData);
        }
    }
    public void AssignUser()
    {
        PlayerID = GenerateUser();
    }

    public void AssignOpinionList()
    {
        var _ListCount = Random.Range(0, 15);
        for (int i = 0; i < _ListCount; i++)
        {
            Session.OpinionsToReview.Add(GenerateOpinionFromOtherUser());
        }
    }

    public void AssignOpinionSubmission()
    {
        Session.OpinionSubmission = new()
        {
            opinionID = Random.value.ToString(),
            topicID = Random.Range(1, 10).ToString(),
            opinionText = OpinionPool[Random.Range(0, OpinionPool.Length)],
            authorName = PlayerID.Name,
            authorAge = PlayerID.Age,
            authorCity = PlayerID.City
        };
    }
    public void AssignChoices()
    {
        for (Session.CurrentReviewIndex = 0;
        Session.CurrentReviewIndex < Session.OpinionsToReview.Count;
        Session.CurrentReviewIndex++)
        {
            Session.Choices.Add(Session.CurrentReviewIndex, GenerateAgreeDisagree());
        }
    }

    PlayerIdData GenerateUser()
    {
        return new(
            Random.Range(0, 100),
            NamePool[Random.Range(0, NamePool.Length)],
            Random.Range(15, 30),
            CityPool[Random.Range(0, CityPool.Length)],
            Random.Range(1, 40));
    }
    Opinion GenerateOpinionFromOtherUser()
    {
        PlayerIdData id = GenerateUser();
        return new()
        {
            opinionID = Random.value.ToString(),
            topicID = Random.Range(1, 10).ToString(),
            opinionText = OpinionPool[Random.Range(0, OpinionPool.Length)],
            authorName = id.Name,
            authorAge = id.Age,
            authorCity = id.City
        };
    }

    bool GenerateAgreeDisagree()
    {
        return Random.value < 0.5;
    }
}
