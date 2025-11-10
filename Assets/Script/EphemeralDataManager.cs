using UnityEngine;

public class EphemeralDataManager : MonoBehaviour
{
    public PlayerIdData playerIdData;
    public static EphemeralDataManager Instance { get; private set; }
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
}