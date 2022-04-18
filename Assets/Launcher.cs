using UnityEngine;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviour
{

    [SerializeField] private string localScene;
    [SerializeField] private string networkedScene;
    
    public void LaunchLocal()
    {
        SceneManager.LoadScene(localScene);
    }

    public void LaunchNetworked()
    {
        SceneManager.LoadScene(networkedScene);
    }
}