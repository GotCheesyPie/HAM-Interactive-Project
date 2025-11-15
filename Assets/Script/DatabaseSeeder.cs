using UnityEngine;
using Firebase.Firestore;
using System.Collections.Generic;

public class DatabaseSeeder : MonoBehaviour
{
    // Cukup klik kanan skrip ini di Inspector dan pilih "Upload Seed Data"
    [ContextMenu("Upload Seed Data")]
    public void UploadSeedData()
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        if (db == null)
        {
            Debug.LogError("Firebase belum siap. Pastikan sudah diinisialisasi.");
            return;
        }

        List<Dictionary<string, object>> seedOpinions = GetAllSeedOpinions();
        
        WriteBatch batch = db.StartBatch();
        CollectionReference opinionsCol = db.Collection("opinions");

        Debug.Log($"Mempersiapkan {seedOpinions.Count} opini seed...");

        foreach (var opinionData in seedOpinions)
        {
            // Buat dokumen baru di koleksi "opinions"
            DocumentReference docRef = opinionsCol.Document(); 
            batch.Set(docRef, opinionData);
        }

        // Commit batch ke server
        batch.CommitAsync().ContinueWith(task => {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError($"Gagal meng-upload seed data: {task.Exception}");
            }
            else
            {
                Debug.Log($"SUKSES: {seedOpinions.Count} opini seed berhasil di-upload.");
            }
        });
    }

    private List<Dictionary<string, object>> GetAllSeedOpinions()
    {
        List<Dictionary<string, object>> opinions = new List<Dictionary<string, object>>();

        // --- HAK PENDIDIKAN (10) ---
        AddOpinion(opinions, "hak_pendidikan", "Pendidikan harusnya gratis.", "Andi", 20, "Jakarta");
        AddOpinion(opinions, "hak_pendidikan", "Kurikulum sekarang terlalu berat.", "Bunga", 17, "Surabaya");
        AddOpinion(opinions, "hak_pendidikan", "Kualitas guru lebih penting dari gratis.", "Candra", 35, "Medan");
        // ... (tambahkan 7 lagi untuk topik ini)

        // --- HAK BERPENDAPAT (10) ---
        AddOpinion(opinions, "hak_berpendapat", "Bebas berpendapat boleh, tapi harus sopan.", "Dewi", 22, "Bandung");
        AddOpinion(opinions, "hak_berpendapat", "Media sosial harus lebih ketat.", "Eka", 29, "Jogja");
       
       // --- HAK LINGKUNGAN (10) ---
        AddOpinion(opinions, "hak_lingkungan", "Perubahan iklim itu nyata!", "Zul", 40, "Papua");

        return opinions;
    }

    // Fungsi helper untuk membuat data opini
    private void AddOpinion(List<Dictionary<string, object>> list, string topic, string text, string name, int age, string city)
    {
        list.Add(new Dictionary<string, object>
        {
            { "authorAge", age },
            { "authorCity", city },
            { "authorName", name },
            { "createdAt", FieldValue.ServerTimestamp }, // Penting untuk query
            { "isSeedData", true }, // Tandai sebagai data seed
            { "text", text },
            { "topicID", topic }
        });
    }
}