using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MoralChoiceManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform cardSpawnPoint;
    public GameObject opinionCardPrefab; 
    public TextMeshProUGUI instructionText;

    [Header("Pressure Effects")]
    public Slider timerBar; // 
    public Image vignetteOverlay; // 
    public AudioSource tickingAudio; // 
    
    [Header("Settings")]
    public float timeLimit = 5.0f; // 5 Detik [cite: 86]
    public float maxAudioPitch = 3.0f; // Seberapa cepat detak jam di akhir

    [Header("Trash Cans")]
    public MoralChoiceDrag trashCan1; // Trash Utama
    public GameObject trashCan2; // Trash Rahasia 

    private float timer;
    private bool secretOptionUnlocked = false;
    private bool gameEnded = false;

    void Start()
    {
        // 1. Setup Awal
        timer = timeLimit;
        trashCan2.SetActive(false); // Sembunyikan Trash 2
        trashCan1.SetDraggable(false); // Trash 1 belum bisa digerakkan
        
        // Konfigurasi Target Drag
        trashCan1.targetTag = "Trash2"; 
        trashCan1.OnValidDrop += OnTrash1Dropped; // Listener Ending B

        // Setup Instruksi Awal
        instructionText.text = "Drag opini yang kamu tidak setuju ke tempat sampah."; // [cite: 82]

        // Setup Audio
        if (tickingAudio != null)
        {
            tickingAudio.pitch = 1.0f;
            tickingAudio.Play();
        }

        // 2. Spawn Opini Dummy/Nyata dari Flow 2
        SpawnDisagreedOpinion();
    }

    void Update()
    {
        if (gameEnded) return;

        // --- LOGIKA TIMER & TEKANAN PSIKOLOGIS ---
        if (!secretOptionUnlocked && timer > 0)
        {
            timer -= Time.deltaTime;

             // 1. Animasi Timer Bar 
            if (timerBar != null)
                timerBar.value = timer / timeLimit;

             // 2. Efek Vignette (Layar menggelap) 
            if (vignetteOverlay != null)
            {
                Color c = vignetteOverlay.color;
                // Semakin sedikit waktu, semakin gelap (max alpha 0.8)
                c.a = Mathf.Lerp(0f, 0.8f, 1 - (timer / timeLimit));
                vignetteOverlay.color = c;
            }

             // 3. Audio Tempo Meningkat 
            if (tickingAudio != null)
            {
                // Pitch naik dari 1.0 ke maxAudioPitch seiring waktu habis
                tickingAudio.pitch = Mathf.Lerp(1.0f, maxAudioPitch, 1 - (timer / timeLimit));
            }

            // Cek Waktu Habis
            if (timer <= 0)
            {
                UnlockSecretOption();
            }
        }
    }

    void SpawnDisagreedOpinion()
    {
        // Ambil data dari GameManager (hasil Flow 2)
        List<Opinion> disagreed = GameManager.Instance.disagreedOpinions;
        
        // Jika list kosong (misal langsung main scene ini), buat dummy
        Opinion opToShow = (disagreed.Count > 0) ? disagreed[0] : new Opinion { 
            topicID = "CONTOH_TOPIK",
            opinionText = "Ini adalah contoh opini karena data kosong.",
            authorName = "Anonim",
            authorAge = 20,
            authorCity = "Jakarta"
        };

        GameObject card = Instantiate(opinionCardPrefab, cardSpawnPoint);
        
        // Setup Teks Kartu (Pastikan prefab punya komponen yang benar)
        OpinionCardUI cardUI = card.GetComponent<OpinionCardUI>();
        
        if (cardUI != null)
        {
            cardUI.SetData(opToShow);
        }
        else
        {
            Debug.LogWarning("Skrip 'OpinionCardUI' belum dipasang di Prefab OpinionCard_Trash!");
        }

        // Tambahkan komponen Drag
        MoralChoiceDrag drag = card.GetComponent<MoralChoiceDrag>();
        if (drag == null) drag = card.AddComponent<MoralChoiceDrag>();
        
        drag.targetTag = "Trash1"; // Kartu -> Trash 1
        drag.OnValidDrop += OnOpinionDropped; // Listener Ending A
    }

     // --- LOGIKA SECRET OPTION REVEAL [cite: 90-94] ---
    void UnlockSecretOption()
    {
        secretOptionUnlocked = true;
        
        // 1. Hentikan efek tekanan
        if (tickingAudio != null) tickingAudio.Stop();
        if (vignetteOverlay != null) vignetteOverlay.color = new Color(0,0,0,0.8f); // Tetap gelap dramatis
        
         // 2. Munculkan Trash Can #2 
        trashCan2.SetActive(true);
        
         // 3. Ubah Instruksi 
        instructionText.text = "Kalau tidak mau membuang, drag tempat sampahnya ke tong sampah di bawah";
        
        // 4. Izinkan Trash #1 digerakkan
        trashCan1.SetDraggable(true);
        
        Debug.Log("Secret Option Terbuka!");
    }

    // --- ENDING DETERMINATION LOGIC ---

     // Ending A: Pemain membuang Opini ke Sampah (Gagal Moral) [cite: 96]
    void OnOpinionDropped(GameObject target)
    {
        if (gameEnded) return;
        gameEnded = true;

        Debug.Log("ENDING A: Membungkam Suara.");
        // Simpan state ke GameManager jika perlu
        SceneLoader.Instance.LoadEnding(false); // false = Bad/Normal Ending
    }

     // Ending B: Pemain membuang Trash #1 ke Trash #2 (Sukses Moral) [cite: 104]
    void OnTrash1Dropped(GameObject target)
    {
        if (gameEnded) return;
        gameEnded = true;

        Debug.Log("ENDING B: Menghargai Perbedaan.");
        // Simpan state ke GameManager
        SceneLoader.Instance.LoadEnding(true); // true = Good Ending
    }
}