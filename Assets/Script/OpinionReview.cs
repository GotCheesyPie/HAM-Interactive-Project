using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class OpinionReview : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject loadingPanel;
    public GameObject reviewPanel;

    [Header("UI Display")]
    public TextMeshProUGUI counterText;
    public TextMeshProUGUI opinionText;
    public TextMeshProUGUI authorText;

    [Header("UI Buttons (Simulasi Swipe)")]
    public Button agreeButton;
    public Button disagreeButton;

    private List<Opinion> opinionsToReview;
    private int currentOpinionIndex;
    private string currentTopicID;

    void Start()
    {
        // Setup awal
        loadingPanel.SetActive(true);
        reviewPanel.SetActive(false);
        counterText.text = "Memuat...";

        // Tambahkan listener ke tombol
        agreeButton.onClick.AddListener(OnAgree);
        disagreeButton.onClick.AddListener(OnDisagree);

        // 1. Bersihkan list opini tidak setuju dari sesi sebelumnya
        GameManager.Instance.disagreedOpinions.Clear();

        // 2. Ambil topik yang dipilih pemain dari GameManager
        currentTopicID = GameManager.Instance.currentPlayer.submittedTopicID;

        // 3. Panggil DatabaseManager untuk mengambil 10 opini
        DatabaseManager.Instance.GetOpinionsForTopic(
            currentTopicID,
            OnOpinionsReceived, // Callback jika sukses
            OnError              // Callback jika gagal
        );
    }

    // Callback jika Firebase SUKSES mengambil data
    void OnOpinionsReceived(List<Opinion> opinions)
    {
        if (opinions == null || opinions.Count == 0)
        {
            // Kasus jika database kosong
            Debug.LogWarning("Tidak ada opini ditemukan untuk topik ini.");
            // Langsung lompat ke Flow 3
            SceneLoader.Instance.LoadMoralChoice();
            return;
        }

        opinionsToReview = opinions;
        currentOpinionIndex = 0;

        // Tampilkan UI
        loadingPanel.SetActive(false);
        reviewPanel.SetActive(true);

        // Tampilkan opini pertama
        ShowOpinion(currentOpinionIndex);
    }

    // Callback jika Firebase GAGAL
    void OnError(string error)
    {
        Debug.LogError($"Gagal mengambil opini: {error}");
        loadingPanel.SetActive(true);
        reviewPanel.SetActive(false);
        counterText.text = "Gagal memuat opini. Coba lagi.";
    }

    // Menampilkan data opini ke UI
    void ShowOpinion(int index)
    {
        Opinion op = opinionsToReview[index];

        opinionText.text = op.opinionText;
        authorText.text = $"Ditulis oleh {op.authorName}, berumur {op.authorAge}, dari {op.authorCity}";
        counterText.text = $"{index + 1} / {opinionsToReview.Count}";
    }

    // Dipanggil oleh AgreeButton (Swipe Right) 
    void OnAgree()
    {
        Debug.Log($"Setuju dengan: {opinionsToReview[currentOpinionIndex].opinionText}");
        GoToNextOpinion();
    }

    // Dipanggil oleh DisagreeButton (Swipe Left) 
    void OnDisagree()
    {
        Opinion disagreedOp = opinionsToReview[currentOpinionIndex];
        Debug.Log($"Tidak setuju dengan: {disagreedOp.opinionText}");

        // --- INTI LOGIKA FLOW 2 ---
        // Simpan opini ini ke GameManager untuk digunakan di Flow 3 
        GameManager.Instance.disagreedOpinions.Add(disagreedOp);
        
        GoToNextOpinion();
    }

    // Pindah ke opini berikutnya atau ke scene selanjutnya
    void GoToNextOpinion()
    {
        currentOpinionIndex++;

        // Cek apakah masih ada opini
        if (currentOpinionIndex < opinionsToReview.Count)
        {
            ShowOpinion(currentOpinionIndex);
        }
        else
        {
            // Selesai! Pindah ke Flow 3
            Debug.Log("Selesai me-review 10 opini. Pindah ke Moral Choice.");
            SceneLoader.Instance.LoadMoralChoice();
        }
    }
}