using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))] 
public class SceneLoader : MonoBehaviour, IPersistable
{
    public static SceneLoader Instance { get; private set; }

    [Header("Scene Names")]
    public string homeScreenScene = "HomeScreenScene";
    public string characterCreationScene = "CharacterCreationScene";
    public string dataInputScene = "DataInputScene";
    public string topicSelectScene = "TopicSelectionScene";
    public string opinionWriteScene = "OpinionWritingScene";
    public string opinionReviewScene = "OpinionReviewScene"; 
    public string moralChoiceScene = "MoralChoiceScene"; 
    public string endingScene = "EndingScene";
    public string thankYouScene = "ThankYouScene";
    // public string creditScene = "CreditScene"; // DIHAPUS

    [Header("Audio Clips (BGM)")]
    public AudioClip homeMusic;      
    public AudioClip flow1Music;     
    public AudioClip flow2Music;     
    public AudioClip flow3Music;     
    public AudioClip endingAMusic;   
    public AudioClip endingBMusic;   
    public AudioClip creditMusic; // Tetap ada untuk BGM saat panel muncul

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.playOnAwake = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (audioSource.clip == clip && audioSource.isPlaying) return;

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }

    // --- FUNGSI PUBLIC AUDIO ---
    // Dipanggil oleh HomeManager/EndingManager saat membuka panel credit
    public void PlayCreditMusic()
    {
        PlayMusic(creditMusic);
    }
    
    public void PlayHomeMusic()
    {
        PlayMusic(homeMusic);
    }

    // --- NAVIGASI SCENE ---

    public void LoadHomeScreen()
    {
        PlayMusic(homeMusic);
        SceneManager.LoadScene(homeScreenScene);
    }

    public void LoadCharacterCreation()
    {
        PlayMusic(flow1Music); 
        SceneManager.LoadScene(characterCreationScene);
    }

    public void LoadDataInput()
    {
        PlayMusic(flow1Music); 
        SceneManager.LoadScene(dataInputScene);
    }

    public void LoadTopicSelection()
    {
        PlayMusic(flow1Music); 
        SceneManager.LoadScene(topicSelectScene);
    }

    public void LoadOpinionWriting()
    {
        PlayMusic(flow1Music); 
        SceneManager.LoadScene(opinionWriteScene);
    }

    public void LoadOpinionReview()
    {
        PlayMusic(flow2Music); 
        SceneManager.LoadScene(opinionReviewScene);
    }

    public void LoadMoralChoice()
    {
        PlayMusic(flow3Music); 
        SceneManager.LoadScene(moralChoiceScene);
    }

    public void LoadEnding(bool isPositiveEnding)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.currentPlayer.didSeePositiveEnding = isPositiveEnding;
            GameManager.Instance.isPositiveEnding = isPositiveEnding;
        }

        if (isPositiveEnding)
            PlayMusic(endingBMusic);
        else
            PlayMusic(endingAMusic);

        SceneManager.LoadScene(endingScene);
    }

    public void LoadThankYouScreen()
    {
        PlayMusic(homeMusic);
        SceneManager.LoadScene(thankYouScene);
    }

    // --- SAVE/LOAD ---
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
        string sceneToLoad = data.CurrentScene;
        if (sceneToLoad == homeScreenScene) PlayMusic(homeMusic);
        else if (sceneToLoad == opinionReviewScene) PlayMusic(flow2Music);
        else if (sceneToLoad == moralChoiceScene) PlayMusic(flow3Music);
        else PlayMusic(flow1Music); 
        SceneManager.LoadScene(sceneToLoad);
    }
}