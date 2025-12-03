using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class EndingManager : MonoBehaviour
{
    [Header("Panel References")]
    public GameObject redScreenPanel;   // Panel Merah (Fase 1)
    public GameObject listScreenPanel;  // Panel Hitam List (Fase 2)

    [Header("List Screen UI")]
    public Transform victimListContainer; // Tempat spawn text list
    public GameObject victimTextPrefab;   // Prefab text "- [Nama]..."
    public TextMeshProUGUI warningText;   // Teks merah "Mereka menulis ini..."
    public GameObject buttonGroup;        // Grup tombol restart/exit

    [Header("Buttons")]
    public Button restartButton;
    public Button exitButton;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip jarringSound; // Suara kaget saat layar merah

    void Start()
    {
        // Setup Button Listeners
        restartButton.onClick.AddListener(OnRestartClicked);
        exitButton.onClick.AddListener(OnExitClicked);

        // Pastikan kedua panel mati dulu di awal frame
        redScreenPanel.SetActive(false);
        listScreenPanel.SetActive(false);

        // Cek Logic GameManager
        if (GameManager.Instance.isPositiveEnding)
        {
            // PlayEndingB(); // (Logika Good Ending - nanti)
        }
        else
        {
            // Mainkan Sequence Bad Ending (Merah -> List)
            StartCoroutine(PlayEndingA());
        }
    }

    IEnumerator PlayEndingA()
    {
        // =================================================
        // PHASE 1: THE ACCUSATION (LAYAR MERAH)
        // =================================================
        
        // 1. Munculkan Layar Merah
        redScreenPanel.SetActive(true);
        
        // 2. Mainkan Suara Kaget
        if (audioSource != null && jarringSound != null)
            audioSource.PlayOneShot(jarringSound);

        // 3. Tunggu pemain membaca teks besar (misal 4 detik)
        yield return new WaitForSeconds(4.0f);

        // =================================================
        // PHASE 2: THE CONSEQUENCE (LAYAR LIST HITAM)
        // =================================================

        // 1. Matikan Layar Merah, Nyalakan Layar List
        redScreenPanel.SetActive(false);
        listScreenPanel.SetActive(true);

        // 2. Generate List Korban (Animasi satu per satu)
        List<Opinion> victims = GameManager.Instance.disagreedOpinions;
        
        // Batasi tampilan (misal max 5 nama agar muat di layar)
        int displayCount = Mathf.Min(victims.Count, 5); 

        // Bersihkan container jika ada isinya
        foreach (Transform child in victimListContainer) Destroy(child.gameObject);

        // Loop untuk memunculkan teks satu per satu
        for (int i = 0; i < displayCount; i++)
        {
            Opinion op = victims[i];
            
            // Spawn Text
            GameObject textObj = Instantiate(victimTextPrefab, victimListContainer);
            TextMeshProUGUI tmp = textObj.GetComponent<TextMeshProUGUI>();
            
            // Set Format Teks: "- [Nama], umur [X], dari [Kota]"
            tmp.text = $"- {op.authorName}, umur {op.authorAge}, dari {op.authorCity}";
            
            // Jeda sedikit antar baris (efek mengetik/muncul)
            yield return new WaitForSeconds(0.5f);
        }

        // (Opsional) Jika data kosong saat testing, munculkan dummy
        if (displayCount == 0)
        {
            CreateDummyText("- Budi, umur 20, dari Jakarta"); yield return new WaitForSeconds(0.3f);
            CreateDummyText("- Siti, umur 24, dari Bandung"); yield return new WaitForSeconds(0.3f);
            CreateDummyText("- Andi, umur 19, dari Surabaya"); yield return new WaitForSeconds(0.3f);
        }

        yield return new WaitForSeconds(0.5f);

        // 3. Tampilkan Teks Merah (Pesan Moral)
        if (warningText != null)
        {
            warningText.gameObject.SetActive(true);
            // Animasi Fade In sederhana (opsional)
            warningText.alpha = 0;
            float timer = 0;
            while(timer < 1f)
            {
                timer += Time.deltaTime;
                warningText.alpha = Mathf.Lerp(0, 1, timer);
                yield return null;
            }
        }

        yield return new WaitForSeconds(1.0f);

        // 4. Tampilkan Tombol
        buttonGroup.SetActive(true);
    }

    void CreateDummyText(string content)
    {
        GameObject textObj = Instantiate(victimTextPrefab, victimListContainer);
        textObj.GetComponent<TextMeshProUGUI>().text = content;
    }

    void OnRestartClicked()
    {
        GameManager.Instance.ResetGame();
    }

    void OnExitClicked()
    {
        Application.Quit();
    }
}