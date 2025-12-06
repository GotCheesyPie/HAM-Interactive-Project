using System.Collections.Generic;
using UnityEngine;
using System.Linq; // <-- Penting untuk Shuffle dan Take
using System; 
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions; // <-- Penting untuk ContinueWithOnMainThread

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }
    private FirebaseFirestore db;
    private bool isFirebaseInitialized = false;

    // (Kita tidak perlu localOpinionDB lagi, tapi bisa disimpan untuk testing)
    // private List<Opinion> localOpinionDB; 

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeFirebase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
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

   // Perhatikan perubahan pada 'System.Action<string> onSuccess'
    public void SubmitOpinion(string opinionText, string topicID, PlayerData authorData, 
                              System.Action<string> onSuccess, System.Action<string> onError)
    {
        if (!isFirebaseInitialized)
        {
            onError?.Invoke("Firebase belum siap.");
            return;
        }

        var opinionData = new Dictionary<string, object>
        {
            // ... (isi data sama seperti sebelumnya) ...
            { "authorAge", authorData.playerAge },
            { "authorCity", authorData.playerCity },
            { "authorName", authorData.playerName },
            { "createdAt", FieldValue.ServerTimestamp },
            { "isSeedData", false },
            { "text", opinionText },
            { "topicID", topicID }
        };

        db.Collection("opinions").AddAsync(opinionData).ContinueWithOnMainThread(task => {
            if (task.IsCanceled || task.IsFaulted)
            {
                onError?.Invoke(task.Exception.ToString());
                return;
            }

            // Sukses! Ambil ID Dokumen yang baru dibuat
            DocumentReference docRef = task.Result;
            string newID = docRef.Id;
            
            Debug.Log($"Opini berhasil disubmit dengan ID: {newID}");
            
            // Kirim ID ini kembali ke pemanggil
            onSuccess?.Invoke(newID); 
        });
    }
    
    // -----------------------------------------------------------------
    // --- SISTEM 3: OPINION RETRIEVAL (Fetch & Filter Self) ---
    // -----------------------------------------------------------------
    public void GetOpinionsForTopic(string topicID, 
                                      System.Action<List<Opinion>> onOpinionsReceived,
                                      System.Action<string> onError)
    {
        if (!isFirebaseInitialized)
        {
            onError?.Invoke("Firebase belum siap.");
            return;
        }

        // 1. Query: Ambil 30 opini terbaru (jumlah lebih banyak untuk cadangan filter)
        Query query = db.Collection("opinions")
                        .WhereEqualTo("topicID", topicID)
                        .OrderByDescending("createdAt")
                        .Limit(30);

        query.GetSnapshotAsync().ContinueWithOnMainThread(task => {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError($"Gagal mengambil opini: {task.Exception}");
                onError?.Invoke(task.Exception.ToString());
                return;
            }

            QuerySnapshot snapshot = task.Result;
            List<Opinion> fetchedOpinions = new List<Opinion>();

            // 2. AMBIL ID OPINI SAYA DARI GAMEMANAGER
            string myOpinionID = "";
            if (GameManager.Instance != null && GameManager.Instance.currentPlayer != null)
            {
                myOpinionID = GameManager.Instance.currentPlayer.submittedOpinionID;
            }
            
            Debug.Log($"ID Opini Saya (untuk di-skip): {myOpinionID}");

            // 3. LOOP DAN FILTER
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                // Jika ID dokumen sama dengan ID opini saya, LEWATI.
                if (document.Id == myOpinionID)
                {
                    Debug.Log("Menemukan opini sendiri, dilewati (skip).");
                    continue; 
                }
                // -------------------------------

                Dictionary<string, object> data = document.ToDictionary();
                
                // Cek safety jika ada data field yang hilang
                if (!data.ContainsKey("text") || !data.ContainsKey("authorName")) continue;

                Opinion op = new Opinion
                {
                    opinionID = document.Id,
                    topicID = data.ContainsKey("topicID") ? data["topicID"].ToString() : topicID,
                    opinionText = data["text"].ToString(),
                    authorName = data["authorName"].ToString(),
                    authorCity = data.ContainsKey("authorCity") ? data["authorCity"].ToString() : "",
                    authorAge = data.ContainsKey("authorAge") ? System.Convert.ToInt32(data["authorAge"]) : 0
                };
                
                fetchedOpinions.Add(op);
            }

            // 4. SHUFFLE (ACAK) DAN AMBIL 10
            List<Opinion> randomOpinions = fetchedOpinions
                .OrderBy(o => UnityEngine.Random.value) // Acak urutan
                .Take(10) // Ambil maksimal 10
                .ToList();

            if (randomOpinions.Count == 0)
            {
                Debug.LogWarning("Tidak ada opini lain ditemukan (mungkin database masih sepi).");
            }

            // Kirim hasil
            onOpinionsReceived?.Invoke(randomOpinions);
        });
    }
}