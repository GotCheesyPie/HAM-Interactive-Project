using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PersistenceManager : MonoBehaviour
{
    public GameData GameData;
    List<IPersistable> Subscribers = new();
    public static PersistenceManager Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void TriggerLoad()
    {
        foreach (var item in Subscribers)
        {
            item.Load(GameData);
        }
    }
    public void TriggerSave()
    {
        foreach (var item in Subscribers)
        {
            item.Save(ref GameData);
        }
    }

    public void AddSubcriber(IPersistable persistable)
    {
        Subscribers.Add(persistable);
    }

    void FindAllPersistableScripts()
    {
        // Finds every MonoBehaviour script that also implements IPersistable
        IEnumerable<IPersistable> persistables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IPersistable>();
        Subscribers = new List<IPersistable>(persistables);
    }
}