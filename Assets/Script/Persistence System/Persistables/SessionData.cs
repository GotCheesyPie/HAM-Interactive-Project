using System;
using System.Collections.Generic;
using System.Linq;
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
            UpdateListsFromDict();
        }

        public void OnBeforeSerialize()
        {
            Clear();
            UpdateDictFromLists();
        }

        private void UpdateListsFromDict()
        {
            foreach (var item in this)
            {
                keys.Add(item.Key);
                values.Add(item.Value);
            }
        }

        private void UpdateDictFromLists()
        {
            for (int i = 0; i < Math.Min(keys.Count, values.Count); i++)
            {
                Add(keys[i], values[i]);
            }
        }

        public ChoicesDictionary() { }

        public ChoicesDictionary(Dictionary<int, bool> other)
        {
            keys = other.Keys.ToList();
            values = other.Values.ToList();
            UpdateDictFromLists();
        }
    }

    // public Opinion OpinionSubmission;
}