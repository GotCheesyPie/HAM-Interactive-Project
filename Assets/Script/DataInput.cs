using UnityEngine;
using UnityEngine.UI;
using TMPro; // Gunakan TextMeshPro

public class DataInput : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField nameInput; // "Nama" 
    public TMP_InputField ageInput; // "Umur" 
    public TMP_InputField cityInput; // "Kota" 
    public Button continueButton; // "Lanjut" 
    public TextMeshProUGUI errorText; // Teks untuk menampilkan error

    void Start()
    {
        // Tambahkan listener ke tombol
        continueButton.onClick.AddListener(OnContinueClicked);
        if (errorText != null) errorText.text = "";
    }

    public void OnContinueClicked()
    {
        // Validasi data
        if (string.IsNullOrWhiteSpace(nameInput.text))
        {
            ShowError("Nama tidak boleh kosong.");
            return;
        }
        if (string.IsNullOrWhiteSpace(cityInput.text))
        {
            ShowError("Kota tidak boleh kosong.");
            return;
        }

        int age;
        if (!int.TryParse(ageInput.text, out age) || age <= 0 || age > 150)
        {
            ShowError("Umur tidak valid.");
            return;
        }

        // Validasi berhasil
        if (errorText != null) errorText.text = "";

        // Simpan data (Misalnya ke 'GameDataManager' singleton)
        // PlayerData data = GameDataManager.Instance.CurrentPlayerData;
        // data.playerName = nameInput.text;
        // data.playerAge = age;
        // data.playerCity = cityInput.text;
        
        Debug.Log($"Data disimpan: {nameInput.text}, {age}, {cityInput.text}");

        // Pindah ke scene berikutnya (Topic Selection)
        // SceneManager.LoadScene("TopicSelectionScene");
    }

    void ShowError(string message)
    {
        Debug.LogWarning(message);
        if (errorText != null) errorText.text = message;
    }
}