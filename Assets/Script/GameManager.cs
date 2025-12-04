using System.Collections.Generic;
using UnityEngine;

// GameManager akan menyimpan data pemain saat ini
// dan akan ada di semua scene
public class GameManager : MonoBehaviour, IPersistable
{
    public static GameManager Instance { get; private set; }

    // Data pemain untuk sesi ini 
    public PlayerData currentPlayer;
    // Data opini yang dibuang di Flow 3 (untuk Ending A)
    public Opinion finalVictimData;

    // Status ending (True = B/Good, False = A/Bad)
    public bool isPositiveEnding;

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

    public void Save(ref GameData data)
    {
        data.playerData = new()
        {
            playerName = currentPlayer.playerName,
            playerAge = currentPlayer.playerAge,
            playerCity = currentPlayer.playerCity,
            selectedAvatarID = currentPlayer.selectedAvatarID,

            // Opini yang baru saja ditulis pemain
            submittedTopicID = currentPlayer.submittedTopicID,
            submittedOpinionText = currentPlayer.submittedOpinionText,
            didSeePositiveEnding = currentPlayer.didSeePositiveEnding
        };

        data.finalVictimData = new()
        {
            opinionID = finalVictimData.opinionID,
            topicID = finalVictimData.topicID,
            opinionText = finalVictimData.opinionText,

            // Metadata penulis 
            authorName = finalVictimData.authorName,
            authorAge = finalVictimData.authorAge,
            authorCity = finalVictimData.authorCity
        };

        data.isPositiveEnding = isPositiveEnding;

        data.disagreedOpinions = new(disagreedOpinions);
    }

    public void Load(GameData data)
    {

        disagreedOpinions = new(data.disagreedOpinions);

        isPositiveEnding = data.isPositiveEnding;

        finalVictimData = new()
        {
            opinionID = data.finalVictimData.opinionID,
            topicID = data.finalVictimData.topicID,
            opinionText = data.finalVictimData.opinionText,

            // Metadata penulis 
            authorName = data.finalVictimData.authorName,
            authorAge = data.finalVictimData.authorAge,
            authorCity = data.finalVictimData.authorCity
        };

        currentPlayer = new()
        {
            playerName = data.playerData.playerName,
            playerAge = data.playerData.playerAge,
            playerCity = data.playerData.playerCity,
            selectedAvatarID = data.playerData.selectedAvatarID,

            // Opini yang baru saja ditulis pemain
            submittedTopicID = data.playerData.submittedTopicID,
            submittedOpinionText = data.playerData.submittedOpinionText,
            didSeePositiveEnding = data.playerData.didSeePositiveEnding
        };
    }
}