using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

// Ini adalah Manager baru untuk Scene Review
public class OpinionReviewSwiper : MonoBehaviour
{
    [Header("System References")]
    public GameObject cardPrefab; // Prefab OpinionCard Anda
    public Transform cardSpawnParent; // Tempat kartu akan muncul

    [Header("UI References")]
    public GameObject loadingPanel;
    public TextMeshProUGUI counterText;

    // --- Card Stack System ---
    private List<Opinion> opinionsToReview;
    private int currentOpinionIndex;

    void Start()
    {
        loadingPanel.SetActive(true);
        counterText.text = "Memuat...";

        // 1. Bersihkan list
        GameManager.Instance.disagreedOpinions.Clear();

        // 2. Ambil topik
        string currentTopicID = GameManager.Instance.currentPlayer.submittedTopicID;

        // 3. Panggil Database
        DatabaseManager.Instance.GetOpinionsForTopic(
            currentTopicID,
            OnOpinionsReceived, // Sukses
            OnError             // Gagal
        );
    }

    void OnOpinionsReceived(List<Opinion> opinions)
    {
        if (opinions == null || opinions.Count == 0)
        {
            Debug.LogWarning("Tidak ada opini ditemukan.");
            SceneLoader.Instance.LoadMoralChoice(); // Langsung ke Flow 3
            return;
        }

        opinionsToReview = opinions;
        currentOpinionIndex = 0;
        loadingPanel.SetActive(false);

        // Mulai stack
        SpawnNextCard();
    }

    void OnError(string error)
    {
        Debug.LogError($"Gagal mengambil opini: {error}");
        loadingPanel.SetActive(true);
        counterText.text = "Gagal memuat opini.";
    }

    // --- Sistem Card Stack ---
    void SpawnNextCard()
    {
        // 1. Cek jika kartu sudah habis
        if (currentOpinionIndex >= opinionsToReview.Count)
        {
            AllCardsSwiped();
            return;
        }

        // 2. Buat kartu baru
        GameObject cardObj = Instantiate(cardPrefab, cardSpawnParent);
        OpinionCard card = cardObj.GetComponent<OpinionCard>();

        // 3. Set data
        Opinion data = opinionsToReview[currentOpinionIndex];
        card.SetData(data);

        // 4. Update counter
        counterText.text = $"{currentOpinionIndex + 1} / {opinionsToReview.Count}";

        // 5. Dengarkan event OnCardSwiped dari kartu
        card.OnCardSwiped += HandleCardSwipe;
    }

    // Dipanggil oleh event dari OpinionCard.cs
    void HandleCardSwipe(Opinion swipedOpinion, bool swipedRight)
    {
        if (swipedRight)
        {
            Debug.Log($"Setuju dengan: {swipedOpinion.opinionText}");
        }
        else
        {
            Debug.Log($"Tidak setuju dengan: {swipedOpinion.opinionText}");
            GameManager.Instance.disagreedOpinions.Add(swipedOpinion);
        }

        // Lanjut ke kartu berikutnya
        currentOpinionIndex++;
        SpawnNextCard();
    }

    void AllCardsSwiped()
    {
        // Selesai! Pindah ke Flow 3
        Debug.Log("Selesai me-review 10 opini. Pindah ke Moral Choice.");
        SceneLoader.Instance.LoadMoralChoice();
    }
}