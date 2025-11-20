using UnityEngine;
using Firebase.Firestore;
using System.Collections.Generic;
using Firebase.Extensions;
using System;

// Kelas pembungkus agar array JSON bisa dibaca oleh JsonUtility
[Serializable]
public class OpinionList
{
    public List<OpinionData> opinions;
}

// Kelas data sederhana yang cocok dengan struktur JSON
[Serializable]
public class OpinionData
{
    public string topicID;
    public string text;
    public string authorName;
    public int authorAge;
    public string authorCity;
    // Kita tidak set isSeedData di JSON karena kita set otomatis di script
}

public class JsonSeeder : MonoBehaviour
{
    [Header("JSON File")]
    // Tarik file opinions.json dari Project window ke slot ini
    public TextAsset jsonFile;

    private FirebaseFirestore db;

    void Start()
    {
        // Pastikan Firebase siap (gunakan logic inisialisasi Anda sebelumnya jika perlu)
        db = FirebaseFirestore.DefaultInstance;
    }

    // Panggil fungsi ini lewat Tombol di Inspector atau UI
    [ContextMenu("Upload JSON to Firebase")]
    public void UploadJsonToFirebase()
    {
        if (jsonFile == null)
        {
            Debug.LogError("File JSON belum dimasukkan ke Inspector!");
            return;
        }

        if (db == null)
        {
            db = FirebaseFirestore.DefaultInstance;
        }

        // 1. Parsing JSON
        OpinionList dataList = JsonUtility.FromJson<OpinionList>(jsonFile.text);

        if (dataList == null || dataList.opinions == null)
        {
            Debug.LogError("Gagal membaca JSON. Pastikan formatnya benar.");
            return;
        }

        Debug.Log($"Membaca {dataList.opinions.Count} opini dari file...");

        // 2. Siapkan Batch Write (Agar lebih cepat dan hemat koneksi)
        WriteBatch batch = db.StartBatch();
        CollectionReference opinionsCol = db.Collection("opinions");

        int count = 0;

        foreach (var op in dataList.opinions)
        {
            // Buat dokumen baru dengan ID otomatis
            DocumentReference docRef = opinionsCol.Document();

            // Konversi ke Dictionary untuk Firebase
            // Kita tambahkan ServerTimestamp dan isSeedData=true di sini
            var opinionDict = new Dictionary<string, object>
            {
                { "topicID", op.topicID },
                { "text", op.text },
                { "authorName", op.authorName },
                { "authorAge", op.authorAge },
                { "authorCity", op.authorCity },
                { "isSeedData", true }, // Tandai sebagai data awal
                { "createdAt", FieldValue.ServerTimestamp } // Waktu server
            };

            batch.Set(docRef, opinionDict);
            count++;
        }

        // 3. Commit (Kirim) ke Firebase
        Debug.Log("Mengirim data ke Firebase...");
        batch.CommitAsync().ContinueWithOnMainThread(task => {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError($"Gagal upload: {task.Exception}");
            }
            else
            {
                Debug.Log($"SUKSES! {count} opini telah berhasil di-upload ke database.");
            }
        });
    }
}