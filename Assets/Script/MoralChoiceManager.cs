using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MoralChoiceManager : MonoBehaviour
{
    [Header("UI Grid References")]
    public Transform cardGridContainer; // Ganti spawnPoint jadi Container
    public GameObject opinionCardSpritePrefab; // Prefab kartu
    private int gridCardCount;

    [Header("UI Text References")]
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI warningSubText;
    public TextMeshProUGUI timerText;

    [Header("Pressure Effects")]
    public Slider timerBar;
    public Image vignetteOverlay;
    public AudioSource tickingAudio;

    [Header("Settings")]
    public float timeLimit = 60.0f; // Sesuai gambar "60s"
    public float maxAudioPitch = 3.0f;

    [Header("Trash Cans")]
    public MoralChoiceDrag trashCan1; // Tong Sampah (Tengah)
    public GameObject trashCan2; // Kardus Arsip (Bawah)

    [Header("Mascot")]
    public GameObject mascot;

    private float timer;
    private bool secretOptionUnlocked = false;
    private bool gameEnded = false;

    void Start()
    {
        // 1. Setup Awal
        timer = timeLimit;
        trashCan2.SetActive(false);
        mascot.SetActive(true);
        trashCan1.SetDraggable(false);
        if (GameManager.Instance != null)
        {
            gridCardCount = GameManager.Instance.disagreedOpinions.Count;
        }

        // Konfigurasi Target Drag
        // Kartu dibuang ke Trash1 (Tong Sampah)
        // Trash1 dibuang ke Trash2 (Kardus)
        trashCan1.targetTag = "Trash2";
        trashCan1.OnValidDrop += OnTrash1Dropped; // Listener Ending B

        // 2. Setup Teks Sesuai Gambar
        instructionText.text = "Drag opini yang kamu tidak setuju ke tempat sampah."; // [cite: 82]

        if (warningSubText != null)
        {
            warningSubText.text = "Opini yang kamu buang tidak akan dilihat oleh orang lain lagi."; // [cite: 83]
            warningSubText.gameObject.SetActive(true);
        }

        // 3. Setup Audio
        if (tickingAudio != null)
        {
            tickingAudio.pitch = 1.0f;
            tickingAudio.Play();
        }

        // 4. Spawn Grid Kartu (Hanya Visual Sprite)
        SpawnCardGrid();
    }

    void Update()
    {
        if (gameEnded) return;

        // --- LOGIKA TIMER & TEKANAN PSIKOLOGIS ---
        if (!secretOptionUnlocked && timer > 0)
        {
            timer -= Time.deltaTime;

            // Update UI Timer (Opsional: Tambahkan Text angka 60s jika perlu)
            if (timerBar != null)
                timerBar.value = timer / timeLimit;

            if (timerText != null)
                timerText.text = timer.ToString("F0") + "s";

            // Efek Vignette
            if (vignetteOverlay != null)
            {
                Color c = vignetteOverlay.color;
                c.a = Mathf.Lerp(0f, 0.8f, 1 - (timer / timeLimit));
                vignetteOverlay.color = c;
            }

            // Audio Tempo
            if (tickingAudio != null)
            {
                tickingAudio.pitch = Mathf.Lerp(1.0f, maxAudioPitch, 1 - (timer / timeLimit));
            }

            if (timer <= 0)
            {
                UnlockSecretOption();
            }
        }
    }

    // --- PERUBAHAN UTAMA: SPAWN GRID KARTU TANPA DATA ---
    void SpawnCardGrid()
    {
        // Kita spawn 9 kartu (atau sesuai gridCardCount)
        for (int i = 0; i < gridCardCount; i++)
        {
            GameObject card = Instantiate(opinionCardSpritePrefab, cardGridContainer);

            // --- HAPUS LOGIKA OpinionCardUI ---
            // Kita tidak mengisi teks apa pun karena request "hanya sprite tanpa data tulisan"

            // Tambahkan komponen Drag
            MoralChoiceDrag drag = card.GetComponent<MoralChoiceDrag>();
            if (drag == null) drag = card.AddComponent<MoralChoiceDrag>();

            drag.targetTag = "Trash1"; // Kartu harus dibuang ke Trash 1
            drag.OnValidDrop += OnOpinionDropped; // Listener Ending A
        }
    }

    // --- LOGIKA SECRET OPTION (Trash #2 Muncul) ---
    void UnlockSecretOption()
    {
        secretOptionUnlocked = true;

        // Hentikan efek tekanan
        if (tickingAudio != null) tickingAudio.Stop();

        // Munculkan Trash Can #2 (Kardus Bawah)
        mascot.SetActive(false);
        trashCan2.SetActive(true);

        // Ubah Instruksi
        instructionText.text = "Kalau tidak mau membuang, drag tempat sampahnya ke tong sampah di bawah"; // [cite: 93]
        if (warningSubText != null) warningSubText.gameObject.SetActive(false); // Sembunyikan warning merah

        // Izinkan Trash #1 digerakkan
        trashCan1.SetDraggable(true);

        Debug.Log("Secret Option Terbuka!");
    }

    // --- LOGIKA ENDING ---

    // Ending A: Pemain membuang SALAH SATU kartu ke Sampah
    void OnOpinionDropped(GameObject target)
    {
        // Logika game: Apakah membuang 1 kartu langsung trigger ending?
        // Sesuai GDD "Player drags opinion card to Trash", biasanya trigger ending.
        if (gameEnded) return;
        gameEnded = true;

        // Ambil data korban (ambil yang pertama dari list disagreed sebagai perwakilan)
        if (GameManager.Instance.disagreedOpinions.Count > 0)
        {
            GameManager.Instance.finalVictimData = GameManager.Instance.disagreedOpinions[0];
        }

        GameManager.Instance.isPositiveEnding = false;
        SceneLoader.Instance.LoadEnding(false); // Bad Ending
    }

    // Ending B: Pemain membuang Trash #1 ke Trash #2
    void OnTrash1Dropped(GameObject target)
    {
        if (gameEnded) return;
        gameEnded = true;

        Debug.Log("ENDING B: Menghargai Perbedaan.");
        GameManager.Instance.isPositiveEnding = true;
        SceneLoader.Instance.LoadEnding(true); // Good Ending
    }
}