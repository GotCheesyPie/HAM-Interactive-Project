using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class AvatarSelection : MonoBehaviour
{
    // [Header("Avatar Assets")]
    // // Masukkan 40 sprite avatar Anda di sini 
    // public List<Sprite> avatarPresets; 

    [Header("UI References")]
    public Transform avatarGridContainer; // Objek "Content" dari Scroll View
    public GameObject avatarButtonPrefab; // Prefab tombol avatar
    public Button continueButton; // Tombol "Lanjut" 
    public Image selectedAvatarDisplay;
    public TextMeshProUGUI selectedAvatarLabel;

    private int selectedAvatarID = -1;

    void Start()
    {
        // Nonaktifkan tombol lanjut sampai avatar dipilih
        PopulateAvatarGrid();
        continueButton.onClick.AddListener(OnContinueClicked);
        
        // Ambil ID yang tersimpan di GameManager
        int savedID = GameManager.Instance.currentPlayer.selectedAvatarID;

        // Cek apakah ID valid (artinya pemain sudah pernah memilih sebelumnya)
        if (savedID != -1)
        {
            // Jika ada data, kita "pura-pura" klik avatar tersebut
            // Ini akan otomatis menampilkan gambar besar & mengaktifkan tombol lanjut
            OnAvatarSelected(savedID);
            
            Debug.Log($"Memuat kembali avatar yang dipilih sebelumnya: ID {savedID}");
        }
        else
        {
            // Jika belum ada data (-1), set tampilan default (kosong)
            continueButton.interactable = false;
            if (selectedAvatarDisplay != null) 
                selectedAvatarDisplay.gameObject.SetActive(false);
        }
    }

    // Mengisi grid dengan 40 tombol avatar
    void PopulateAvatarGrid()
    {
        List<Sprite> avatarPresets = GameManager.Instance.globalAvatarList;
        
        for (int i = 0; i < avatarPresets.Count; i++)
        {
            // Buat tombol baru
            GameObject buttonObj = Instantiate(avatarButtonPrefab, avatarGridContainer);
            Button avatarButton = buttonObj.GetComponent<Button>();
            Image avatarImage = buttonObj.transform.Find("Image").GetComponent<Image>();
            TextMeshProUGUI avatarLabel = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            // Set gambar
            avatarImage.sprite = avatarPresets[i];

            // Set label
            avatarLabel.text = "Avatar " + (i + 1);

            // Tambahkan listener
            int index = i; // Penting untuk menyimpan 'i' dalam variabel lokal
            avatarButton.onClick.AddListener(() => OnAvatarSelected(index));
        }
    }

    // Dipanggil saat tombol avatar diklik
    public void OnAvatarSelected(int avatarID)
    {
        selectedAvatarID = avatarID;if (selectedAvatarDisplay != null)
        {
            // 1. Atur sprite-nya
            selectedAvatarDisplay.sprite = GameManager.Instance.globalAvatarList[avatarID];

            // 2. Tampilkan kotaknya
            selectedAvatarDisplay.gameObject.SetActive(true);

            // 3. Ubah labelnya
            selectedAvatarLabel.text = "Avatar " + (avatarID + 1);
        }

        // Aktifkan tombol "Lanjut" 
        continueButton.interactable = true;
        Debug.Log($"Selected Avatar with index {avatarID}");
    }

    public void OnContinueClicked()
    {
        if (selectedAvatarID != -1)
        {
            GameManager.Instance.currentPlayer.selectedAvatarID = selectedAvatarID;
            Debug.Log($"Selected Avatar ID with number {selectedAvatarID}");

            // Pindah ke scene berikutnya (Data Input)
            SceneLoader.Instance.LoadDataInput();
        }
    }
}