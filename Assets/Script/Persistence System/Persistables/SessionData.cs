using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SessionData
{

    public List<Opinion> OpinionsToReview = new(); // Query 10 opinion dari database
    public int CurrentOpinionIndex = 0;
    public ChoicesDictionary Choices = new(); // key = index, value = setuju/tidak

    [Serializable]
    public class ChoicesDictionary : Dictionary<int, bool>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<int> keys = new();
        [SerializeField] private List<bool> values = new();
        public void OnAfterDeserialize()
        {
            keys.Clear();
            values.Clear();

            foreach (var item in this)
            {
                keys.Add(item.Key);
                values.Add(item.Value);
            }
        }

        public void OnBeforeSerialize()
        {
            Clear();
            for (int i = 0; i < Math.Min(keys.Count, values.Count); i++)
            {
                Add(keys[i], values[i]);
            }
        }

        public ChoicesDictionary() { }

        public ChoicesDictionary(Dictionary<int, bool> other)
        {
            foreach (var item in other)
            {
                Add(item.Key, item.Value);
            }
        }
    }

    // public Opinion OpinionSubmission;
}