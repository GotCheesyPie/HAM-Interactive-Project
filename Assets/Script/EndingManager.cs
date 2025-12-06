using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class EndingManager : MonoBehaviour
{
    [Header("--- ENDING A: BAD (Accusation Phase) ---")]
    public GameObject redScreenPanel;   // Panel Merah "SELAMAT KAMU BARU SAJA..."

    [Header("--- ENDING A: BAD (Consequence Phase) ---")]
    public GameObject listScreenPanel;  // Panel Hitam List Korban
    public Transform victimListContainer; // Container dengan Vertical Layout Group
    public GameObject victimTextPrefab;   // Prefab text "- [Nama]..."
    public TextMeshProUGUI warningText;   // Teks Merah "Mereka menulis ini..."
    public GameObject buttonGroup;        // Grup tombol (Restart & Exit)

    [Header("--- ENDING B: GOOD ---")]
    public GameObject endingBPanel;     // Panel "TERIMA KASIH..."

    [Header("--- BUTTONS ---")]
    public Button restartButton; // Tombol Orange (Icon Refresh)
    public Button exitButton;    // Tombol Merah (Icon X)

    [Header("--- AUDIO ---")]
    public AudioSource audioSource;
    public AudioClip jarringSound;  // Suara Kaget (Ending A)
    public AudioClip positiveSound; // Suara Tenang (Ending B)

    void Start()
    {
        // 1. Matikan semua panel di awal agar bersih
        if(redScreenPanel) redScreenPanel.SetActive(false);
        if(listScreenPanel) listScreenPanel.SetActive(false);
        if(endingBPanel) endingBPanel.SetActive(false);
        if(buttonGroup) buttonGroup.SetActive(false);

        // 2. Setup Tombol
        restartButton.onClick.AddListener(OnRestartClicked);
        exitButton.onClick.AddListener(OnExitToCreditClicked);

        // 3. Cek Status Ending dari GameManager
        // (Pastikan GameManager ada, jika testing langsung di scene ini, default ke Bad Ending)
        bool isGoodEnding = false;
        if (GameManager.Instance != null) 
        {
            isGoodEnding = GameManager.Instance.isPositiveEnding;
        }

        // 4. Jalankan Sequence yang sesuai
        if (isGoodEnding)
        {
            StartCoroutine(PlayEndingBSequence());
        }
        else
        {
            StartCoroutine(PlayEndingASequence());
        }
    }

    // =================================================================
    // LOGIKA ENDING A (BAD): Merah -> List Nama -> Tombol
    // =================================================================
    IEnumerator PlayEndingASequence()
    {
        // PHASE 1: LAYAR MERAH
        redScreenPanel.SetActive(true);
        
        if (audioSource && jarringSound) 
            audioSource.PlayOneShot(jarringSound);

        // Tahan selama 4 detik untuk efek dramatis [cite: 99-101]
        yield return new WaitForSeconds(4.0f);

        // PHASE 2: LAYAR LIST HITAM
        redScreenPanel.SetActive(false);
        listScreenPanel.SetActive(true);

        // Ambil data korban dari GameManager
        List<Opinion> victims = new List<Opinion>();
        if (GameManager.Instance != null)
        {
            victims = GameManager.Instance.disagreedOpinions;
        }

        // Batasi tampilan maksimal 5 nama agar UI rapi
        int displayCount = Mathf.Min(victims.Count, 5);

        // Bersihkan container (hapus dummy text editor jika ada)
        foreach (Transform child in victimListContainer) Destroy(child.gameObject);

        // Loop spawn nama satu per satu [cite: 102-103]
        for (int i = 0; i < displayCount; i++)
        {
            Opinion op = victims[i];
            
            GameObject textObj = Instantiate(victimTextPrefab, victimListContainer);
            TextMeshProUGUI tmp = textObj.GetComponent<TextMeshProUGUI>();
            
            // Format teks: "- Nama, Umur, dari Kota"
            tmp.text = $"- {op.authorName}, umur {op.authorAge}, dari {op.authorCity}";
            
            // Jeda 0.5 detik antar nama
            yield return new WaitForSeconds(0.5f);
        }

        // (Opsional) Dummy Data jika list kosong saat testing
        if (displayCount == 0)
        {
            SpawnDummyText("- Budi, umur 20, dari Jakarta"); yield return new WaitForSeconds(0.3f);
            SpawnDummyText("- Siti, umur 24, dari Bandung"); yield return new WaitForSeconds(0.3f);
            SpawnDummyText("- Andi, umur 19, dari Surabaya"); yield return new WaitForSeconds(0.3f);
        }

        yield return new WaitForSeconds(0.5f);

        // Tampilkan Teks Peringatan Merah
        if (warningText)
        {
            warningText.text = "Mereka menulis ini\ndengan harapan suaranya didengar.";
            warningText.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(1.0f);

        // Tampilkan Tombol (Restart & X)
        buttonGroup.SetActive(true);
    }

    // Helper untuk dummy text
    void SpawnDummyText(string text)
    {
        GameObject textObj = Instantiate(victimTextPrefab, victimListContainer);
        textObj.GetComponent<TextMeshProUGUI>().text = text;
    }


    // =================================================================
    // LOGIKA ENDING B (GOOD): Terima Kasih -> Auto Credit
    // =================================================================
    IEnumerator PlayEndingBSequence()
    {
        // Tampilkan Panel Terima Kasih [cite: 107-108]
        endingBPanel.SetActive(true);

        if (audioSource && positiveSound)
            audioSource.PlayOneShot(positiveSound);

        // Biarkan pemain membaca selama 5 detik
        yield return new WaitForSeconds(5.0f);

        // Pindah otomatis ke Credit Scene
        SceneLoader.Instance.LoadCreditScene();
    }


    // =================================================================
    // FUNGSI TOMBOL
    // =================================================================
    void OnRestartClicked()
    {
        // Reset Game Total
        GameManager.Instance.ResetGame();
    }

    void OnExitToCreditClicked()
    {
        // Tombol X mengarah ke Credit Scene (bukan Quit)
        SceneLoader.Instance.LoadCreditScene();
    }
}