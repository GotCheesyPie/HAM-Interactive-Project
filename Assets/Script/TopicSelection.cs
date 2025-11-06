using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TopicSelection : MonoBehaviour
{
    [Header("Topic Data")]
    // Masukkan 10 ScriptableObject TopicData Anda di sini 
    public List<TopicData> topics; 

    [Header("UI References")]
    public Transform topicContainer; // Objek untuk menampung toggle
    public ToggleGroup toggleGroup; // Referensi ke Toggle Group
    public GameObject topicTogglePrefab; // Prefab Toggle
    public Button selectButton; // Tombol "Pilih" 

    private TopicData selectedTopic = null;

    void Start()
    {
        selectButton.interactable = false;
        PopulateTopicList();
    }

    void PopulateTopicList()
    {
        foreach (TopicData topic in topics)
        {
            // Buat Toggle baru
            GameObject toggleObj = Instantiate(topicTogglePrefab, topicContainer);
            Toggle toggle = toggleObj.GetComponent<Toggle>();
            
            // Set grup agar berfungsi sebagai radio button 
            toggle.group = toggleGroup;
            
            // Set label teks
            TextMeshProUGUI label = toggleObj.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
            {
                label.text = topic.topicName; // e.g., "Hak atas Pendidikan" 
            }

            // Tambahkan listener
            toggle.onValueChanged.AddListener((isOn) => {
                if (isOn)
                {
                    OnTopicSelected(topic);
                }
            });
        }
    }

    // Dipanggil saat toggle diubah
    void OnTopicSelected(TopicData topic)
    {
        selectedTopic = topic;
        selectButton.interactable = true;
        Debug.Log($"Topik dipilih: {topic.topicName}");
    }

    public void OnSelectClicked()
    {
        if (selectedTopic != null)
        {
            // Simpan data (Misalnya ke 'GameDataManager' singleton)
            // GameDataManager.Instance.CurrentPlayerData.submittedTopicID = selectedTopic.topicID;
            
            // Pindah ke scene berikutnya (Opinion Writing)
            // SceneManager.LoadScene("OpinionWritingScene");
        }
    }
}