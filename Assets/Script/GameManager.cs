using System.Collections.Generic;
using UnityEngine;

// GameManager akan menyimpan data pemain saat ini
// dan akan ada di semua scene
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Data pemain untuk sesi ini 
    public PlayerData currentPlayer;

    // Menyimpan opini yang di-swipe left (tidak setuju)
    // untuk digunakan di Flow 3 
    public List<Opinion> disagreedOpinions = new List<Opinion>();

    [Header("Global Assets")]
    public List<Sprite> globalAvatarList;
    public List<TopicData> globalTopicDataList;

    public Sprite GetCurrentPlayerSprite()
    {
        int id = currentPlayer.selectedAvatarID;
        
        // Cek validasi agar tidak error
        if (id >= 0 && id < globalAvatarList.Count)
        {
            return globalAvatarList[id];
        }
        return null; // Atau return sprite default jika error
    }

    public TopicData GetCurrentSelectedTopicData()
    {
        string id = currentPlayer.submittedTopicID;

        if (!string.IsNullOrEmpty(id))
        {
            foreach (TopicData topic in globalTopicDataList)
            {
                if (topic.topicID == id)
                {
                    return topic;
                }
            }
        }
        return null;
    }

    void Awake()
    {
        // Setup Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Kunci agar data tidak hilang
            InitializePlayer();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializePlayer()
    {
        currentPlayer = new PlayerData();
        disagreedOpinions.Clear();
        currentPlayer.playerAge = -1;
        currentPlayer.selectedAvatarID = -1;
        SceneLoader.Instance.LoadCharacterCreation(); 
    }

    // Fungsi reset jika pemain ingin "Main Lagi" 
    public void ResetGame()
    {
        InitializePlayer();
    }
}