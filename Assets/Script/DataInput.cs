using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System; // Gunakan TextMeshPro

public class DataInput : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField nameInput; // "Nama" 
    public TMP_InputField ageInput; // "Umur" 
    public TMP_InputField cityInput; // "Kota" 
    public Button continueButton; // "Lanjut" 
    public Button backButton; // "Kembali"
    public TextMeshProUGUI errorText; // Teks untuk menampilkan error

    [Header("Display")]
    public Image avatarDisplay; // Tempat menampilkan avatar

    void Start()
    {
        // Tambahkan listener ke tombol
        continueButton.onClick.AddListener(OnContinueClicked);
        backButton.onClick.AddListener(OnBackClicked);
        if (errorText != null) errorText.text = "";
        ShowSelectedAvatar();
        
        PlayerData current = GameManager.Instance.currentPlayer;
        
        if (!string.IsNullOrEmpty(current.playerName))
        {
            nameInput.text = current.playerName;
        }
        if (current.playerAge > 0)
        {
            ageInput.text = current.playerAge.ToString();
        }
        if (!string.IsNullOrEmpty(current.playerCity))
        {
            cityInput.text = current.playerCity;
        }
    }

    public void OnContinueClicked()
    {
        // Validasi data
        if (string.IsNullOrWhiteSpace(nameInput.text))
        {
            ShowError("Nama tidak boleh kosong.");
            return;
        }
        
        int age;
        if (!int.TryParse(ageInput.text, out age) || age <= 0 || age > 150)
        {
            ShowError("Umur tidak valid.");
            return;
        }

        if (string.IsNullOrWhiteSpace(cityInput.text))
        {
            ShowError("Kota tidak boleh kosong.");
            return;
        }

        // Validasi berhasil
        if (errorText != null) errorText.text = "";

        // Simpan data (Misalnya ke 'GameDataManager' singleton)
        PlayerData data = GameManager.Instance.currentPlayer;
        data.playerName = nameInput.text;
        data.playerAge = age;
        data.playerCity = cityInput.text;

        Debug.Log($"Data disimpan: {nameInput.text}, {age}, {cityInput.text}");

        // Pindah ke scene berikutnya (Topic Selection)
        SceneLoader.Instance.LoadTopicSelection();
    }
    
    private void OnBackClicked()
    {
        SceneLoader.Instance.LoadCharacterCreation();
    }

    void ShowError(string message)
    {
        Debug.LogWarning(message);
        if (errorText != null) errorText.text = message;
    }

    private void ShowSelectedAvatar()
    {
        if (avatarDisplay != null)
        {
            // Minta gambar ke GameManager
            Sprite selectedSprite = GameManager.Instance.GetCurrentPlayerSprite();
            
            if (selectedSprite != null)
            {
                avatarDisplay.sprite = selectedSprite;
                // Pastikan aspect ratio gambar tidak gepeng
                avatarDisplay.preserveAspect = true; 
                avatarDisplay.SetNativeSize();
            }
        }
    }
}