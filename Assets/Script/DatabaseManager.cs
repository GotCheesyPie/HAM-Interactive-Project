using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Untuk mengambil data
using System; // Diperlukan untuk System.Action

// Namespace Firebase yang diperlukan
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;

// Singleton untuk koneksi database
public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }

    private FirebaseFirestore db;
    private bool isFirebaseInitialized = false;

    // --- SIMULASI DATABASE LOKAL ---
    // (Dari skrip Anda)
    private List<Opinion> localOpinionDB;

    void Awake()
    {
        // Setup Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // 1. Inisialisasi Firebase
            InitializeFirebase(); 
            
            // 2. Siapkan data seed lokal Anda
            InitializeSeedData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- FUNGSI BARU UNTUK INISIALISASI FIREBASE ---
    void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Firebase siap
                db = FirebaseFirestore.DefaultInstance;
                isFirebaseInitialized = true;
                Debug.Log("DatabaseManager: Firebase Siap.");
            }
            else
            {
                Debug.LogError($"DatabaseManager: Gagal inisialisasi Firebase: {dependencyStatus}");
            }
        });
    }

    // --- KODE SEED DATA ANDA (Tidak berubah) ---
    void InitializeSeedData()
    {
        localOpinionDB = new List<Opinion>
        {
            // ... (data seed Anda ada di sini) ...
            new Opinion { opinionID = "seed001", topicID = "hak_pendidikan", 
                          opinionText = "Pendidikan harusnya gratis untuk semua.", 
                          authorName = "Andi", authorAge = 20, authorCity = "Jakarta" },
            new Opinion { opinionID = "seed002", topicID = "hak_pendidikan", 
                          opinionText = "Kurikulum sekarang terlalu berat.", 
                          authorName = "Bunga", authorAge = 17, authorCity = "Surabaya" },
            new Opinion { opinionID = "seed003", topicID = "hak_pendidikan", 
                          opinionText = "Yang penting bukan gratis, tapi kualitas guru.", 
                          authorName = "Candra", authorAge = 35, authorCity = "Medan" },
            new Opinion { opinionID = "seed004", topicID = "hak_berpendapat", 
                          opinionText = "Bebas berpendapat boleh, tapi harus sopan.", 
                          authorName = "Dewi", authorAge = 22, authorCity = "Bandung" }
        };
    }

    // --- FUNGSI SUBMIT BARU (Menggantikan yang lama) ---
    // Fungsi ini dikirim dari skrip OpinionWriting.cs
    public void SubmitOpinion(string opinionText, string topicID, PlayerData authorData, 
                              System.Action onSuccess, System.Action<string> onError)
    {
        if (!isFirebaseInitialized)
        {
            onError?.Invoke("Firebase belum siap. Coba lagi sebentar.");
            return;
        }

        // 1. Buat data opini (pakai Dictionary agar bisa pakai ServerTimestamp)
        // Ini cocok dengan struktur database tes Anda
        var opinionData = new Dictionary<string, object>
        {
            { "authorAge", authorData.playerAge },
            { "authorCity", authorData.playerCity },
            { "authorName", authorData.playerName },
            { "createdAt", FieldValue.ServerTimestamp }, // Waktu dari Server
            { "isSeedData", false },
            { "text", opinionText }, // Sesuai nama field di Firebase
            { "topicID", topicID }
        };

        // 2. Tunjuk ke koleksi "opinions"
        CollectionReference opinionsCol = db.Collection("opinions");

        // 3. Tambahkan data
        opinionsCol.AddAsync(opinionData).ContinueWithOnMainThread(task => {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError($"Gagal menambahkan dokumen: {task.Exception}");
                onError?.Invoke(task.Exception.ToString()); // Kirim error
                return;
            }

            // Sukses!
            DocumentReference docRef = task.Result;
            Debug.Log($"Opini berhasil disubmit dengan ID: {docRef.Id}");
            onSuccess?.Invoke(); // Panggil callback sukses
        });
    }
    
    // --- FUNGSI GET OPINI ANDA (Masih pakai simulasi lokal) ---
    public void GetOpinionsForTopic(string topicID, 
                                      System.Action<List<Opinion>> onOpinionsReceived)
    {
        Debug.Log($"DATABASE (Lokal): Mengambil 10 opini untuk topik: {topicID}");

        // --- KODE SIMULASI (Lokal) ---
        List<Opinion> results = localOpinionDB
            .Where(o => o.topicID == topicID)
            .OrderBy(o => UnityEngine.Random.value) // Acak urutannya
            .Take(10) // Ambil 10 
            .ToList();
        
        // Panggil callback-nya seolah-olah data baru datang dari server
        onOpinionsReceived?.Invoke(results);
    }
}