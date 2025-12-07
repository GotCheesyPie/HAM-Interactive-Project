using UnityEngine;
using UnityEngine.UI;

public class HomeScreenManager : MonoBehaviour
{
    public Button startButton;
    public Button exitButton;
    public Button creditButton;
    public Button creditExitButton;
    public GameObject creditPanel;
    void Start()
    {
        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(GameManager.Instance.InitializePlayer);
        
        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(OnExit);

        creditButton.onClick.RemoveAllListeners();
        creditButton.onClick.AddListener(OnCreditClicked);

        creditExitButton.onClick.RemoveAllListeners();
        creditExitButton.onClick.AddListener(OnCreditExit);
    }

    public void OnExit()
    {
        Application.Quit();
    }

    public void OnCreditClicked()
    {
        creditPanel.SetActive(true);
        SceneLoader.Instance.PlayCreditMusic();
    }

    public void OnCreditExit()
    {
        creditPanel.SetActive(false);
        SceneLoader.Instance.PlayHomeMusic();
    }
}
