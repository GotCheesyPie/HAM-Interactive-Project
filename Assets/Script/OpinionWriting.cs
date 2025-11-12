using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class OpinionWriting : MonoBehaviour
{
    [Header("Data Assets")]
    // Masukkan SEMUA 10 ScriptableObject TopicData Anda ke sini
    public List<TopicData> allTopics;

    [Header("Opinion Panel UI")]
    public GameObject opinionPanel;
    public TMP_InputField opinionInput;
    public TextMeshProUGUI counterText;
    public TextMeshProUGUI promptText;
    public Button submitButton;
    public TextMeshProUGUI errorText; // Opsional, untuk error submit

    [Header("Choice Panel UI")]
    public GameObject choicePanel; // Dialog "Ya/Tidak" [cite: 59]
    public Button choiceYesButton;
    public Button choiceNoButton;

    private string currentTopicID;
    private TopicData currentTopic;

    void Start()
    {
        // Setup state awal
        opinionPanel.SetActive(true);
        choicePanel.SetActive(false);
        if (errorText != null) errorText.text = "";

        // 1. Ambil data dari GameManager
        currentTopicID = GameManager.Instance.currentPlayer.submittedTopicID;
        currentTopic = allTopics.FirstOrDefault(t => t.topicID == currentTopicID);

        if (currentTopic == null)
        {
            Debug.LogError("Tidak bisa menemukan TopicData! Pastikan 'allTopics' terisi.");
            return;
        }

        // 2. Setup UI
        promptText.text = currentTopic.prompt;
        
        // --- FITUR 1: 280 Char Limit ---
        opinionInput.characterLimit = 280; 

        // 3. Tambahkan Listeners
        opinionInput.onValueChanged.AddListener(OnInputChanged);
        submitButton.onClick.AddListener(OnSubmitClicked);
        choiceYesButton.onClick.AddListener(OnChoiceYes);
        choiceNoButton.onClick.AddListener(OnChoiceNo);

        // 4. Inisialisasi counter dan validasi
        OnInputChanged("");
    }

    /// <summary>
    /// Dipanggil setiap kali input field berubah.
    /// Menangani Character Counter dan Form Validation.
    /// </summary>
    private void OnInputChanged(string newText)
    {
        // --- FITUR 3: Character Counter Implementation ---
        counterText.text = $"{newText.Length} / 280";

        // --- FITUR 2: Form Validation ---
        // Hanya aktifkan tombol jika teks tidak kosong (bukan spasi saja)
        submitButton.interactable = !string.IsNullOrWhiteSpace(newText);
    }

    /// <summary>
    /// Dipanggil saat tombol Submit diklik.
    /// Menangani Opinion Submission.
    /// </summary>
    private void OnSubmitClicked()
    {
        // --- FITUR 1: Opinion Submission System ---
        
        // Nonaktifkan tombol untuk mencegah submit ganda
        submitButton.interactable = false;
        
        string text = opinionInput.text;
        PlayerData author = GameManager.Instance.currentPlayer;

        // Panggil DatabaseManager
        DatabaseManager.Instance.SubmitOpinion(text, currentTopicID, author,
            // Callback OnSuccess:
            () => {
                Debug.Log("Opini berhasil disubmit!");
                // Simpan opini pemain di GameManager (opsional)
                GameManager.Instance.currentPlayer.submittedOpinionText = text;
                // Lanjut ke Step 5: Choice Point [cite: 59]
                ShowChoicePoint();
            },
            // Callback OnError:
            (error) => {
                Debug.LogError($"Submit gagal: {error}");
                if (errorText != null) errorText.text = "Gagal submit. Coba lagi.";
                submitButton.interactable = true; // Izinkan coba lagi
            }
        );
    }

    // --- (Penanganan Flow 1, Step 5) ---

    private void ShowChoicePoint()
    {
        opinionPanel.SetActive(false);
        choicePanel.SetActive(true);
    }

    private void OnChoiceYes()
    {
        // Lanjut ke Flow 2 
        SceneLoader.Instance.LoadOpinionReview();
    }

    private void OnChoiceNo()
    {
        // Game end 
        SceneLoader.Instance.LoadThankYouScreen();
    }
}