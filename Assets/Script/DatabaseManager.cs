using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Untuk mengambil data

// Singleton untuk koneksi database
public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }

    // --- SIMULASI DATABASE LOKAL ---
    // Kita gunakan ini untuk testing sebelum ada backend
    private List<Opinion> localOpinionDB;

    void Awake()
    {
        // Setup Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSeedData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Mengisi database palsu dengan 100 opini awal 
    void InitializeSeedData()
    {
        localOpinionDB = new List<Opinion>
        {
            // Contoh untuk 1 topik
            new Opinion { opinionID = "seed001", topicID = "hak_pendidikan", 
                          opinionText = "Pendidikan harusnya gratis untuk semua.", 
                          authorName = "Andi", authorAge = 20, authorCity = "Jakarta" },
            new Opinion { opinionID = "seed002", topicID = "hak_pendidikan", 
                          opinionText = "Kurikulum sekarang terlalu berat.", 
                          authorName = "Bunga", authorAge = 17, authorCity = "Surabaya" },
            new Opinion { opinionID = "seed003", topicID = "hak_pendidikan", 
                          opinionText = "Yang penting bukan gratis, tapi kualitas guru.", 
                          authorName = "Candra", authorAge = 35, authorCity = "Medan" },
            
            // ... (tambahkan 97 opini lainnya untuk 10 topik)
                          
            new Opinion { opinionID = "seed004", topicID = "hak_berpendapat", 
                          opinionText = "Bebas berpendapat boleh, tapi harus sopan.", 
                          authorName = "Dewi", authorAge = 22, authorCity = "Bandung" }
        };
    }

    // --- METODE DATABASE (Placeholder) ---

    // 1. Mengirim opini pemain ke database
    public void SubmitOpinion(Opinion opinion)
    {
        Debug.Log($"DATABASE: Menyimpan opini: '{opinion.opinionText}'");
        
        // --- KODE FIREBASE SEBENARNYA (Contoh) ---
        // FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        // CollectionReference colRef = db.Collection("opinions");
        // colRef.AddAsync(opinion); // Ini adalah async operation
        // --- Akhir Kode Firebase ---

        // Untuk simulasi, kita tambahkan ke DB lokal
        opinion.opinionID = "player_" + Random.Range(1000, 9999);
        localOpinionDB.Add(opinion);
    }

    // 2. Mengambil 10 opini dari database untuk Flow 2 
    // Ini adalah fungsi ASYNCHRONOUS, jadi kita pakai 'Callback'
    public void GetOpinionsForTopic(string topicID, 
                                     System.Action<List<Opinion>> onOpinionsReceived)
    {
        Debug.Log($"DATABASE: Mengambil 10 opini untuk topik: {topicID}");

        // --- KODE FIREBASE SEBENARNYA (Contoh) ---
        // FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        // Query query = db.Collection("opinions")
        //                 .WhereEqualTo("topicID", topicID)
        //                 .Limit(10);
        //
        // query.GetSnapshotAsync().ContinueWithOnMainThread(task => {
        //     List<Opinion> fetchedOpinions = new List<Opinion>();
        //     QuerySnapshot snapshot = task.Result;
        //     foreach (DocumentSnapshot document in snapshot.Documents) {
        //         fetchedOpinions.Add(document.ConvertTo<Opinion>());
        //     }
        //     onOpinionsReceived?.Invoke(fetchedOpinions); // Kirim data via callback
        // });
        // --- Akhir Kode Firebase ---


        // --- KODE SIMULASI (Lokal) ---
        // Ambil 10 opini acak dari DB lokal yang sesuai topik
        List<Opinion> results = localOpinionDB
            .Where(o => o.topicID == topicID)
            .OrderBy(o => Random.value) // Acak urutannya
            .Take(10) // Ambil 10 
            .ToList();
        
        // Panggil callback-nya seolah-olah data baru datang dari server
        onOpinionsReceived?.Invoke(results);
    }
}