using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistenceManager : MonoBehaviour
{
    public GameData GameData;
    private IStorageHandler storageHandler;

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
        SceneManager.sceneUnloaded += PullSceneData;
        SceneManager.activeSceneChanged += RefreshSubscriberList;
    }

    void Start()
    {
        storageHandler = gameObject.AddComponent<JSONStorageHandler>();
    }

    [ContextMenu("Load Game")]
    public void TriggerLoad()
    {
        LoadStarted?.Invoke();

        GameData = storageHandler.Read("save");
        PushToPersistables();

        LoadEnded?.Invoke();
    }

    [ContextMenu("Save Game")]
    public void TriggerSave()
    {
        SaveStarted?.Invoke();

        PullFromPersistables();
        storageHandler.Write(GameData);

        SaveEnded?.Invoke();
    }

    private void PullFromPersistables()
    {
        foreach (var item in Subscribers)
        {
            item.Save(ref GameData);
        }
    }

    private void PushToPersistables()
    {
        foreach (var item in Subscribers)
        {
            item.Load(GameData);
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

        Debug.Log($"[{GetType().Name}] Found {Subscribers.Count} persistables");
    }

    void RefreshSubscriberList(Scene _, Scene __)
    {
        FindAllPersistableScripts();
    }

    void PullSceneData(Scene _)
    {
        PullFromPersistables();
    }
}