using System.Collections.Generic;
using UnityEngine;

// GameManager akan menyimpan data pemain saat ini
// dan akan ada di semua scene
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Data pemain untuk sesi ini 
    public PlayerData currentPlayer;

    // Menyimpan opini yang di-swipe left (tidak setuju)
    // untuk digunakan di Flow 3 
    public List<Opinion> disagreedOpinions = new List<Opinion>();

    void Awake()
    {
        // Setup Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Kunci agar data tidak hilang
            InitializePlayer();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializePlayer()
    {
        currentPlayer = new PlayerData();
        disagreedOpinions.Clear();
        SceneLoader.Instance.LoadCharacterCreation(); 
    }

    // Fungsi reset jika pemain ingin "Main Lagi" 
    public void ResetGame()
    {
        InitializePlayer();
    }
}