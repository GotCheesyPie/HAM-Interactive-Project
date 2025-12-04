using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

// Ini adalah Manager baru untuk Scene Review
public class OpinionReviewSwiper : MonoBehaviour, IPersistable
{
    [Header("System References")]
    public GameObject cardPrefab; // Prefab OpinionCard Anda
    public Transform cardSpawnParent; // Tempat kartu akan muncul

    [Header("UI References")]
    public GameObject loadingPanel;

    // --- Card Stack System ---
    private List<Opinion> opinionsToReview;
    private int currentOpinionIndex;
    private Dictionary<int, bool> choices; // key = index list, value = isSetuju
    private CountdownSpriteSwapper counterSprite;

    void Start()
    {
        loadingPanel.SetActive(true);

        // 1. Bersihkan list
        GameManager.Instance.disagreedOpinions.Clear(); //FIXME add null check for testing scene

        // 2. Ambil topik
        string currentTopicID = GameManager.Instance.currentPlayer.submittedTopicID;

        // 3. Panggil Database
        DatabaseManager.Instance.GetOpinionsForTopic(
            currentTopicID,
            OnOpinionsReceived, // Sukses
            OnError             // Gagal
        );

        // 4. Set Counter Sprite
        counterSprite = GetComponent<CountdownSpriteSwapper>();
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
        counterSprite.IncrementIndex();

        // 5. Dengarkan event OnCardSwiped dari kartu
        card.OnCardSwiped += HandleCardSwipe;
    }

    // Dipanggil oleh event dari OpinionCard.cs
    void HandleCardSwipe(Opinion swipedOpinion, bool swipedRight)
    {
        if (swipedRight)
        {
            Debug.Log($"Setuju dengan: {swipedOpinion.opinionText}");
            choices.Add(currentOpinionIndex, true);
        }
        else
        {
            Debug.Log($"Tidak setuju dengan: {swipedOpinion.opinionText}");
            GameManager.Instance.disagreedOpinions.Add(swipedOpinion);
            choices.Add(currentOpinionIndex, false);
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

    public void Save(ref GameData data)
    {
        data.Session.CurrentOpinionIndex = currentOpinionIndex;
        data.Session.OpinionsToReview = new(opinionsToReview);
        data.Session.Choices = new(choices);
    }

    public void Load(GameData data)
    {
        currentOpinionIndex = data.Session.CurrentOpinionIndex;
        opinionsToReview = new(opinionsToReview);
        choices = new(data.Session.Choices);
    }
}