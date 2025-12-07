using UnityEngine;
using UnityEngine.SceneManagement; // Wajib ada

// Singleton untuk mengontrol perpindahan scene
public class SceneLoader : MonoBehaviour, IPersistable
{
    public static SceneLoader Instance { get; private set; }

    // --- Nama-nama Scene Anda (harus sama dengan di Build Settings) ---
    [Header("Scene Names")]
    public string homeScreenScene = "HomeScreenScene";
    public string characterCreationScene = "CharacterCreationScene";
    public string dataInputScene = "DataInputScene";
    public string topicSelectScene = "TopicSelectionScene";
    public string opinionWriteScene = "OpinionWritingScene";
    public string opinionReviewScene = "OpinionReviewScene"; // Flow 2 
    public string moralChoiceScene = "MoralChoiceScene"; // Flow 3 
    public string endingScene = "EndingScene";
    public string thankYouScene = "ThankYouScene";
    public string creditScene = "CreditScene";

    void Awake()
    {
        // Setup Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- Metode Alur Game ---

    public void LoadCharacterCreation()
    {
        SceneManager.LoadScene(characterCreationScene);
    }

    public void LoadDataInput()
    {
        SceneManager.LoadScene(dataInputScene);
    }

    public void LoadTopicSelection()
    {
        SceneManager.LoadScene(topicSelectScene);
    }

    public void LoadOpinionWriting()
    {
        SceneManager.LoadScene(opinionWriteScene);
    }

    // Dipanggil setelah menulis opini dan memilih "Ya" 
    public void LoadOpinionReview()
    {
        SceneManager.LoadScene(opinionReviewScene);
    }

    // Dipanggil setelah selesai Flow 2
    public void LoadMoralChoice()
    {
        SceneManager.LoadScene(moralChoiceScene);
    }

    public void LoadEnding(bool isPositiveEnding)
    {
        GameManager.Instance.currentPlayer.didSeePositiveEnding = isPositiveEnding;
        GameManager.Instance.isPositiveEnding = isPositiveEnding;
        SceneManager.LoadScene(endingScene);
    }
    public void LoadThankYouScreen()
    {
        SceneManager.LoadScene(thankYouScene);
    }

    public void Save(ref GameData data)
    {
        // Only save at progressed runs
        if (
            SceneManager.GetActiveScene().name == homeScreenScene ||
            SceneManager.GetActiveScene().name == "InitializationScene"
        )
        { return; }
        data.CurrentScene = SceneManager.GetActiveScene().name;
    }

    public void Load(GameData data)
    {
        SceneManager.LoadScene(string.IsNullOrEmpty(data.CurrentScene) ? "InitializationScene" : data.CurrentScene);
    }

    public void LoadHomeScreen()
    {
        SceneManager.LoadScene(homeScreenScene);
    }
}