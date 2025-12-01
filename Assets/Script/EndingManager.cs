using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class EndingManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI mainTitleText; // Teks Besar
    public TextMeshProUGUI humanizationText; // Teks Detail Penulis
    public TextMeshProUGUI subMessageText; // "Mereka menulis ini..."
    public Image flashOverlay; // Panel Merah untuk Flash
    public Button restartButton;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip jarringSound; // Suara Ending A 
    public AudioClip positiveSound; // Suara Ending B (untuk nanti)

    void Start()
    {
        // Setup Awal: Sembunyikan semua teks & overlay
        mainTitleText.alpha = 0;
        humanizationText.alpha = 0;
        subMessageText.alpha = 0;
        flashOverlay.color = new Color(1, 0, 0, 0); // Merah transparan
        restartButton.gameObject.SetActive(false);
        restartButton.onClick.AddListener(OnRestartClicked);

        // Cek Ending mana yang harus dimainkan
        if (GameManager.Instance.isPositiveEnding)
        {
            // PlayEndingB(); // (Belum diimplementasi)
        }
        else
        {
            StartCoroutine(PlayEndingASequence());
        }
    }

     // --- LOGIKA ENDING A  ---
    IEnumerator PlayEndingASequence()
    {
         // 1. Siapkan Teks Sesuai GDD
        mainTitleText.text = "SELAMAT, KAMU BARU SAJA MEMBUNGKAM HAK BERPENDAPAT MANUSIA LAINNYA";
        
         // Siapkan Data Humanisasi
        Opinion victim = GameManager.Instance.finalVictimData;
        if (victim != null)
        {
            humanizationText.text = $"Opini yang kamu buang ditulis oleh {victim.authorName}, umur {victim.authorAge}, dari {victim.authorCity}.";
        }
        else
        {
            humanizationText.text = "Opini yang kamu buang ditulis oleh seseorang yang ingin didengar.";
        }

        subMessageText.text = "\"Mereka menulis ini dengan harapan suaranya didengar.\"";

        yield return new WaitForSeconds(0.5f);

         // 2. AUDIO JARRING & SCREEN FLASH
        if (audioSource != null && jarringSound != null)
        {
            audioSource.PlayOneShot(jarringSound);
        }

        // Efek Flash Merah (Cepat: Muncul -> Hilang)
        float flashDuration = 0.2f;
        float elapsed = 0;
        
        // Flash In
        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 0.8f, elapsed / flashDuration); // Max alpha 0.8
            flashOverlay.color = new Color(0.8f, 0, 0, alpha); // Warna Merah Gelap
            yield return null;
        }
        
        // Flash Out
        elapsed = 0;
        while (elapsed < flashDuration * 2) // Fade out lebih pelan sedikit
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0.8f, 0, elapsed / (flashDuration * 2));
            flashOverlay.color = new Color(0.8f, 0, 0, alpha);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

         // 3. TEXT REVEAL ANIMATION 
        // Munculkan teks utama perlahan
        yield return StartCoroutine(FadeInText(mainTitleText, 2.0f));
        
        yield return new WaitForSeconds(1.0f);

         // 4. HUMANIZATION DATA DISPLAY 
        // Munculkan detail korban
        yield return StartCoroutine(FadeInText(humanizationText, 1.5f));
        
        yield return new WaitForSeconds(1.0f);

        // 5. SUB MESSAGE
        yield return StartCoroutine(FadeInText(subMessageText, 1.5f));

        yield return new WaitForSeconds(2.0f);
        restartButton.gameObject.SetActive(true);
    }

    // Helper untuk animasi Fade In Text
    IEnumerator FadeInText(TextMeshProUGUI textUI, float duration)
    {
        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            textUI.alpha = Mathf.Lerp(0, 1, timer / duration);
            yield return null;
        }
        textUI.alpha = 1;
    }

    void OnRestartClicked()
    {
        GameManager.Instance.ResetGame();
    }
}