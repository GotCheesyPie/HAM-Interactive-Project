using System;
using UnityEngine;

// Data untuk 10 Topik HAM 
// Cara terbaik di Unity adalah menggunakan ScriptableObject
[CreateAssetMenu(fileName = "TopicData", menuName = "HAM/Topic Data")]
public class TopicData : ScriptableObject
{
    public string topicID; // "hak_berpendapat", "hak_pendidikan", etc.
    public string topicName; // "Hak Kebebasan Berpendapat" 
    [TextArea(3, 5)]
    public string prompt; // "Apa pendapat kamu tentang..." 
}

// Data untuk satu opini 
[Serializable]
public class Opinion
{
    public string opinionID; // ID unik dari database
    public string topicID; // Link ke TopicData
    public string opinionText; // Max 280 karakter 
    
    // Metadata penulis 
    public string authorName;
    public int authorAge;
    public string authorCity;
}

// Data pemain yang akan disimpan
[Serializable]
public class PlayerData
{
    public string playerName; // "Nama" 
    public int playerAge; // "Umur" 
    public string playerCity; // "Kota Lahir" 
    public int selectedAvatarID; // Index dari 40 preset 
    
    // Opini yang baru saja ditulis pemain
    public string submittedTopicID;
    public string submittedOpinionText;
    public bool didSeePositiveEnding;
}