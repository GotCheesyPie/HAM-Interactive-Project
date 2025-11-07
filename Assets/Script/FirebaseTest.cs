using UnityEngine;
using TMPro; // Anda perlu TextMeshPro untuk status
using System;
using System.Collections.Generic; // Untuk Dictionary

// Import Firebase
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions; // Diperlukan untuk ContinueWithOnMainThread

public class FirebaseTest : MonoBehaviour
{
    // Seret UI TextMeshPro ke sini untuk melihat status
    public TextMeshProUGUI statusText; 
    
    private FirebaseFirestore db;
    private bool isFirebaseInitialized = false;
    
    // Kita simpan ID dokumen yang baru dibuat
    // agar kita tahu apa yang harus dihapus
    private string lastAddedDocumentId;

    void Start()
    {
        statusText.text = "Menginisialisasi Firebase...";
        
        // Cek dan perbaiki dependensi Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Firebase siap
                db = FirebaseFirestore.DefaultInstance;
                isFirebaseInitialized = true;
                statusText.text = "Firebase Siap. Tekan tombol tes.";
                Debug.Log("Firebase berhasil diinisialisasi.");
            }
            else
            {
                // Gagal
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                statusText.text = "Error: Gagal inisialisasi Firebase.";
            }
        });
    }

    /// <summary>
    /// Fungsi ini untuk di-trigger oleh tombol "Add Opinion"
    /// </summary>
    public void TestAddOpinion()
    {
        if (!isFirebaseInitialized)
        {
            statusText.text = "Firebase belum siap.";
            return;
        }

        statusText.text = "Menambahkan opini tes...";

        // 1. Buat data tes (struktur sama seperti di gambar Anda)
        // Kita gunakan Dictionary untuk tes cepat
        var opinionData = new Dictionary<string, object>
        {
            { "authorAge", 99 },
            { "authorCity", "Kota Tes" },
            { "authorName", "User Tes Unity" },
            { "createdAt", FieldValue.ServerTimestamp }, // Gunakan waktu server
            { "isSeedData", false },
            { "text", "Ini adalah opini tes yang dikirim dari Unity." },
            { "topicID", "hak_testing" }
        };

        // 2. Tunjuk ke koleksi "opinions"
        CollectionReference opinionsCol = db.Collection("opinions");

        // 3. Tambahkan data (ini asynchronous)
        opinionsCol.AddAsync(opinionData).ContinueWithOnMainThread(task => {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError($"Gagal menambahkan dokumen: {task.Exception}");
                statusText.text = "Gagal menambahkan opini.";
                return;
            }

            // Sukses!
            DocumentReference docRef = task.Result;
            lastAddedDocumentId = docRef.Id; // <-- Simpan ID ini!
            
            statusText.text = $"Sukses! Opini ditambahkan dengan ID:\n{lastAddedDocumentId}";
            Debug.Log($"Dokumen berhasil ditambahkan dengan ID: {lastAddedDocumentId}");
        });
    }

    /// <summary>
    /// Fungsi ini untuk di-trigger oleh tombol "Delete Last Opinion"
    /// </summary>
    public void TestDeleteLastOpinion()
    {
        if (!isFirebaseInitialized)
        {
            statusText.text = "Firebase belum siap.";
            return;
        }

        if (string.IsNullOrEmpty(lastAddedDocumentId))
        {
            statusText.text = "Belum ada opini untuk dihapus. Tambah dulu.";
            return;
        }

        statusText.text = $"Menghapus opini: {lastAddedDocumentId}...";

        // 1. Tunjuk ke dokumen spesifik yang ingin dihapus
        DocumentReference docRef = db.Collection("opinions").Document(lastAddedDocumentId);

        // 2. Hapus dokumen (ini asynchronous)
        docRef.DeleteAsync().ContinueWithOnMainThread(task => {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError($"Gagal menghapus dokumen: {task.Exception}");
                statusText.text = "Gagal menghapus opini.";
                return;
            }

            // Sukses!
            statusText.text = $"Sukses! Opini {lastAddedDocumentId} telah dihapus.";
            Debug.Log($"Dokumen {lastAddedDocumentId} berhasil dihapus.");
            lastAddedDocumentId = null; // Kosongkan ID-nya
        });
    }
}