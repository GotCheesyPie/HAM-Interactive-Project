using UnityEngine;
using UnityEngine.SceneManagement;
public class InMemoryStorageScene_SceneManager : MonoBehaviour
{
    public void MoveToNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}