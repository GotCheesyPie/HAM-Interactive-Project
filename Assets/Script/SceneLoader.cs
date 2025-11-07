using UnityEngine;
using UnityEngine.SceneManagement; // Wajib ada

// Singleton untuk mengontrol perpindahan scene
public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    // --- Nama-nama Scene Anda (harus sama dengan di Build Settings) ---
    [Header("Scene Names")]
    public string avatarSelectionScene = "AvatarSelectionScene";
    public string dataInputScene = "DataInputScene";
    public string topicSelectScene = "TopicSelectionScene";
    public string opinionWriteScene = "OpinionWritingScene";
    public string opinionReviewScene = "OpinionReviewScene"; // Flow 2 
    public string moralChoiceScene = "MoralChoiceScene"; // Flow 3 
    public string endingScene = "EndingScene";

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
        SceneManager.LoadScene(avatarSelectionScene);
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

    // Dipanggil jika pemain memilih "Tidak" 
    // atau setelah Flow 3
    public void LoadEnding(bool isPositiveEnding)
    {
        // Kita bisa gunakan PlayerPrefs atau GameManager 
        // untuk memberitahu scene ending mana yang harus ditampilkan
        GameManager.Instance.currentPlayer.didSeePositiveEnding = isPositiveEnding;
        SceneManager.LoadScene(endingScene);
    }
}