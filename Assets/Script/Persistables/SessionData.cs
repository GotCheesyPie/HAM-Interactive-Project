using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SessionData : IPersistable
{
    public FlowStage CurrentFlow;

    public List<Opinion> OpinionsToReview = new(); // Query 10 opinion dari database
    public int CurrentReviewIndex = 0;
    public Dictionary<int, bool> Choices = new(); // Maybe better as Disctionary<int, bool> ? (int index from list)

    public Opinion OpinionSubmission;

    public void Load(GameData data)
    {
        CurrentFlow = data.Session.CurrentFlow;
        OpinionsToReview = data.Session.OpinionsToReview;
        CurrentReviewIndex = data.Session.CurrentReviewIndex;
        Choices = data.Session.Choices;
        OpinionSubmission = data.Session.OpinionSubmission;
    }

    public void Save(ref GameData data)
    {
        data.Session = this;
    }

    // Request kak Kiki: Track topic yang dipilih
    // -> Ambil dari first elem Choices?
    // artinya kalau player belum pilih setuju/tidak ke opini pertamanya = Pilih kategori lagi + Query lagi
}

[Serializable]
public enum FlowStage
{
    Prologue,
    ReviewOpinions,
    MoralChoice
}