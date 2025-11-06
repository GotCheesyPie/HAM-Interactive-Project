using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AvatarSelection : MonoBehaviour
{
    [Header("Avatar Assets")]
    // Masukkan 40 sprite avatar Anda di sini 
    public List<Sprite> avatarPresets; 

    [Header("UI References")]
    public Transform avatarGridContainer; // Objek "Content" dari Scroll View
    public GameObject avatarButtonPrefab; // Prefab tombol avatar
    public Button continueButton; // Tombol "Lanjut" 

    private int selectedAvatarID = -1;
    private List<Button> avatarButtons = new List<Button>();

    void Start()
    {
        // Nonaktifkan tombol lanjut sampai avatar dipilih
        continueButton.interactable = false;
        PopulateAvatarGrid();
    }

    // Mengisi grid dengan 40 tombol avatar
    void PopulateAvatarGrid()
    {
        for (int i = 0; i < avatarPresets.Count; i++)
        {
            // Buat tombol baru
            GameObject buttonObj = Instantiate(avatarButtonPrefab, avatarGridContainer);
            Button avatarButton = buttonObj.GetComponent<Button>();
            
            // Set gambar
            avatarButton.image.sprite = avatarPresets[i];
            avatarButtons.Add(avatarButton);

            // Tambahkan listener
            int index = i; // Penting untuk menyimpan 'i' dalam variabel lokal
            avatarButton.onClick.AddListener(() => OnAvatarSelected(index));
        }
    }

    // Dipanggil saat tombol avatar diklik
    public void OnAvatarSelected(int avatarID)
    {
        selectedAvatarID = avatarID;

        // Visual feedback (Highlight)
        for(int i = 0; i < avatarButtons.Count; i++)
        {
            // Set highlight/normal color
            var colors = avatarButtons[i].colors;
            if (i == avatarID) {
                colors.normalColor = Color.yellow; // Contoh highlight
            } else {
                colors.normalColor = Color.white;
            }
            avatarButtons[i].colors = colors;
        }

        // Aktifkan tombol "Lanjut" 
        continueButton.interactable = true;
    }

    public void OnContinueClicked()
    {
        if (selectedAvatarID != -1)
        {
            // Simpan data (Misalnya ke 'GameDataManager' singleton)
            // GameDataManager.Instance.CurrentPlayerData.selectedAvatarID = selectedAvatarID;
            
            // Pindah ke scene berikutnya (Data Input)
            // SceneManager.LoadScene("DataInputScene");
        }
    }
}