using UnityEngine;
using UnityEngine.UI;

public class RestartButtonManager : MonoBehaviour
{
    public Button restartButton;
    void Start()
    {
        restartButton.onClick.RemoveAllListeners();
        restartButton.onClick.AddListener(OnRestartClicked);
    }
    
    public void OnRestartClicked()
    {
        GameManager.Instance.ResetGame();
    }
}
