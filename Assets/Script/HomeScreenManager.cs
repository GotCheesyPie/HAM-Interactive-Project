using UnityEngine;
using UnityEngine.UI;

public class HomeScreenManager : MonoBehaviour
{
    public Button startButton;
    public Button exitButton;
    void Start()
    {
        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(GameManager.Instance.InitializePlayer);
        
        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(OnExit);
    }

    public void OnExit()
    {
        Application.Quit();
    }
}
