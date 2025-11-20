using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TopicSelection : MonoBehaviour
{
    [Header("UI References")]
    // Masukkan 10 Button yang ada di Scene ke sini
    public List<Button> topicButtons; 
    public Button selectButton; // Tombol "Pilih" yang besar di bawah
    public TextMeshProUGUI topicText;

    [Header("Visual Settings")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow; // Warna saat tombol dipilih

    private TopicData selectedTopic = null;

    void Start()
    {
        // Matikan tombol "Pilih" di awal
        selectButton.interactable = false;
        selectButton.onClick.AddListener(OnSelectClicked);

        SetupTopicButtons();
    }

    void SetupTopicButtons()
    {
        List<TopicData> topics = GameManager.Instance.globalTopicDataList; 
        // Pastikan jumlah tombol dan topik sama (atau minimal tombol cukup)
        if (topicButtons.Count != topics.Count)
        {
            Debug.LogWarning("Jumlah Tombol dan Topik tidak sama! Cek Inspector.");
        }

        int count = Mathf.Min(topics.Count, topicButtons.Count);

        for (int i = 0; i < count; i++)
        {
            // Ambil referensi lokal agar aman di dalam lambda listener
            int index = i;
            TopicData data = topics[i];
            Button btn = topicButtons[i];
            btn.image.color = normalColor;

            // Kita hapus dulu listener lama agar tidak menumpuk
            btn.onClick.RemoveAllListeners(); 
            btn.onClick.AddListener(() => OnTopicButtonClicked(data, btn));
        }
    }

    // Dipanggil saat salah satu tombol topik diklik
    void OnTopicButtonClicked(TopicData data, Button clickedButton)
    {
        selectedTopic = data;
        
        // Aktifkan tombol "Pilih" utama
        selectButton.interactable = true;
        
        Debug.Log($"Topik dipilih: {data.topicName}");

        topicText.text = data.topicName;

        // Update Visual (Efek Radio Button)
        UpdateButtonVisuals(clickedButton);
    }

    // Mengubah warna tombol: yang diklik jadi Kuning, sisanya Putih
    void UpdateButtonVisuals(Button selectedBtn)
    {
        foreach (Button btn in topicButtons)
        {
            Image btnImage = btn.transform.Find("Image").GetComponent<Image>();
            if (btn == selectedBtn)
            {
                // Ini tombol yang dipilih
                btnImage.color = selectedColor;
            }
            else
            {
                // Ini tombol lain (reset)
                btnImage.color = normalColor;
            }
        }
    }

    public void OnSelectClicked()
    {
        if (selectedTopic != null)
        {
            // Simpan data ke GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.currentPlayer.submittedTopicID = selectedTopic.topicID;            
                
                // Pindah ke scene berikutnya
                SceneLoader.Instance.LoadOpinionWriting();
            }
            else
            {
                Debug.LogError("GameManager belum ada. Jalankan dari Scene awal.");
            }
        }
    }
}