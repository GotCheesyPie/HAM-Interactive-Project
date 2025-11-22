using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class MoralChoiceManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform cardSpawnPoint;
    public GameObject opinionCardPrefab; // Prefab OpinionCard (gunakan yang UI simple saja)
    
    [Header("Trash Cans")]
    public MoralChoiceDrag trashCan1; // Tong Sampah Utama [cite: 81]
    public GameObject trashCan2; // Tong Sampah Rahasia [cite: 94]
    
    [Header("Timer & Feedback")]
    public float timeUntilSecretOption = 5.0f; // 5 detik [cite: 86]
    public Slider timerBar; // Visual timer bar [cite: 87]
    public TextMeshProUGUI instructionText;
    public Image vignetteOverlay; // Efek gelap [cite: 89]

    private float timer = 0f;
    private bool secretOptionUnlocked = false;
    private List<Opinion> disagreedOpinions;

    void Start()
    {
        // 1. Setup Awal
        trashCan2.SetActive(false); // Sembunyikan Trash #2
        trashCan1.SetDraggable(false); // Trash #1 belum bisa digerakkan
        
        // Atur target drag:
        // Kartu Opini -> harus di-drop ke "Trash1"
        // Trash Can 1 -> harus di-drop ke "Trash2"
        trashCan1.targetTag = "Trash2"; 
        
        // Dengarkan jika Trash #1 berhasil dibuang ke Trash #2
        trashCan1.OnValidDrop += OnTrash1Dropped;

        instructionText.text = "Drag opini yang kamu tidak setuju ke tempat sampah.";

        // 2. Ambil data opini yang "Tidak Disetujui" dari Flow 2 
        disagreedOpinions = GameManager.Instance.disagreedOpinions;

        if (disagreedOpinions.Count > 0)
        {
            SpawnOpinionCard(disagreedOpinions[0]); // Tampilkan 1 saja sebagai simbol
        }
        else
        {
            Debug.LogWarning("Tidak ada opini disagreed. Menggunakan dummy.");
        }
    }

    void Update()
    {
        if (!secretOptionUnlocked)
        {
            timer += Time.deltaTime;
            
            // Update Timer Bar (Opsional)
            if (timerBar != null)
                timerBar.value = timer / timeUntilSecretOption;

            // [cite_start]// Efek Vignette (Semakin lama semakin gelap)
            if (vignetteOverlay != null)
            {
                Color c = vignetteOverlay.color;
                c.a = Mathf.Clamp01(timer / timeUntilSecretOption * 0.8f);
                vignetteOverlay.color = c;
            }

            // Cek Timer 5 Detik
            if (timer >= timeUntilSecretOption)
            {
                UnlockSecretOption();
            }
        }
    }

    void SpawnOpinionCard(Opinion op)
    {
        GameObject card = Instantiate(opinionCardPrefab, cardSpawnPoint);
        
        // Setup visual kartu
        // (Pastikan prefab kartu punya komponen TextMeshProUGUI untuk isi teks)
        // ... kode setup text ...
        
        // Pasang komponen Drag
        MoralChoiceDrag dragScript = card.AddComponent<MoralChoiceDrag>(); // Atau sudah ada di prefab
        dragScript.targetTag = "Trash1"; // Kartu harus dibuang ke Trash1
        dragScript.OnValidDrop += OnOpinionDropped;
    }

    void UnlockSecretOption()
    {
        secretOptionUnlocked = true;
        
        // Munculkan Trash Can #2
        trashCan2.SetActive(true);
        
        // [cite_start]// [cite: 93] Ubah instruksi
        instructionText.text = "Kalau tidak mau membuang, drag tempat sampahnya ke tong sampah di bawah.";
        
        // Izinkan Trash #1 untuk di-drag
        trashCan1.SetDraggable(true);
        
        Debug.Log("SECRET OPTION UNLOCKED!");
    }

    // --- ENDING LOGIC ---

    // [cite_start]// Ending A: Pemain membuang Opini ke Trash #1 [cite: 96]
    void OnOpinionDropped(GameObject target)
    {
        Debug.Log("ENDING A: KAMU MEMBUNGKAM SUARA ORANG LAIN.");
        // Panggil SceneLoader untuk Ending A (Negatif)
        SceneLoader.Instance.LoadEnding(false); 
    }

    // [cite_start]// Ending B: Pemain membuang Trash #1 ke Trash #2 [cite: 104]
    void OnTrash1Dropped(GameObject target)
    {
        Debug.Log("ENDING B: KAMU MENGHARGAI PERBEDAAN.");
        // Panggil SceneLoader untuk Ending B (Positif)
        SceneLoader.Instance.LoadEnding(true);
    }
}