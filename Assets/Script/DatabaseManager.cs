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

    public void SubmitOpinion(string opinionText, string topicID, PlayerData authorData, 
                              System.Action onSuccess, System.Action<string> onError)
    {
        if (!isFirebaseInitialized)
        {
            onError?.Invoke("Firebase belum siap. Coba lagi sebentar.");
            return;
        }

        var opinionData = new Dictionary<string, object>
        {
            { "authorAge", authorData.playerAge },
            { "authorCity", authorData.playerCity },
            { "authorName", authorData.playerName },
            { "createdAt", FieldValue.ServerTimestamp }, // Penting untuk retrieval
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
            onSuccess?.Invoke();
        });
    }
    
    public void GetOpinionsForTopic(string topicID, 
                                      System.Action<List<Opinion>> onOpinionsReceived,
                                      System.Action<string> onError)
    {
        if (!isFirebaseInitialized)
        {
            onError?.Invoke("Firebase belum siap.");
            return;
        }

        // Query: Ambil 30 opini terbaru untuk topik ini
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

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                // Ubah data dari Firebase (Dictionary) ke kelas C# (Opinion)
                Dictionary<string, object> data = document.ToDictionary();
                Opinion op = new Opinion
                {
                    opinionID = document.Id,
                    topicID = data["topicID"].ToString(),
                    opinionText = data["text"].ToString(),
                    authorName = data["authorName"].ToString(),
                    authorCity = data["authorCity"].ToString(),
                    // Perlu konversi karena data di Firebase adalah Int64
                    authorAge = Convert.ToInt32(data["authorAge"]) 
                };
                fetchedOpinions.Add(op);
            }

            // --- Logika "Random" ---
            List<Opinion> randomOpinions = fetchedOpinions
                .OrderBy(o => UnityEngine.Random.value) // Acak urutannya
                .Take(10) // Ambil 10
                .ToList();

            // Kirim 10 opini acak kembali ke game
            onOpinionsReceived?.Invoke(randomOpinions);
        });
    }
}