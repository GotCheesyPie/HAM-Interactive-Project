using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SessionData
{

    public List<Opinion> OpinionsToReview = new(); // Query 10 opinion dari database
    public int CurrentOpinionIndex = 0;
    public Dictionary<int, bool> Choices = new();

    // public Opinion OpinionSubmission;
}