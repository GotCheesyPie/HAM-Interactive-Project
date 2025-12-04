using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SessionData : IPersistable
{
    public FlowStage CurrentFlow;

    // public List<Opinion> OpinionsToReview = new(); // Query 10 opinion dari database
    public int CurrentReviewIndex = 0;
    public Dictionary<int, bool> Choices = new();

    // public Opinion OpinionSubmission;

    public void Load(GameData data)
    {
        CurrentFlow = data.Session.CurrentFlow;
        CurrentReviewIndex = data.Session.CurrentReviewIndex;
        Choices = data.Session.Choices;
    }

    public void Save(ref GameData data)
    {
        data.Session.CurrentFlow = CurrentFlow;
        data.Session.CurrentReviewIndex = CurrentReviewIndex;
        data.Session.Choices = Choices;
    }

    // Request kak Kiki: Track topic yang dipilih
    // -> Ambil dari first elem Choices?
    // artinya kalau player belum pilih setuju/tidak ke opini pertamanya = Pilih kategori lagi + Query lagi
}

[Serializable]
public enum FlowStage // 3 flow
{
    Prologue, // aka isi data diri + buat opini
    ReviewOpinions, // aka flow swipe kiri/kanan
    MoralChoice // aka buang opini
}