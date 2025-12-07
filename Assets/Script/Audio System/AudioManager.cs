using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixer mixer;

    public AudioSource SceneLoaderSource;
    // if needed, add more

    public AudioMixerGroup SFX;
    public AudioMixerGroup Music;


    public AudioManager Instance { get; private set; }

    private void Awake()
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
        Music = mixer.FindMatchingGroups("Music")[0];
        SFX = mixer.FindMatchingGroups("Sound Effects")[0];

        TryGetSceneLoaderAudioSource(out SceneLoaderSource);
    }

    void TryGetSceneLoaderAudioSource(out AudioSource source)
    {
        if (SceneLoader.Instance != null)
        {
            source = SceneLoader.Instance.gameObject.GetComponent<AudioSource>();
            return;
        }
        source = null;
    }
}