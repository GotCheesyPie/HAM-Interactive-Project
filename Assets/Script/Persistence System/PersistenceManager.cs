using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PersistenceManager : MonoBehaviour
{
    public GameData GameData;

    List<IPersistable> Subscribers = new();

    public event Action SaveStarted;
    public event Action LoadStarted;
    public event Action SaveEnded;
    public event Action LoadEnded;

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

    void Start()
    {
        FindAllPersistableScripts();
    }

    public void TriggerLoad()
    {
        LoadStarted?.Invoke();

        foreach (var item in Subscribers)
        {
            item.Load(GameData);
        }

        LoadEnded?.Invoke();
    }
    public void TriggerSave()
    {
        SaveStarted?.Invoke();

        foreach (var item in Subscribers)
        {
            item.Save(ref GameData);
        }

        SaveEnded?.Invoke();
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