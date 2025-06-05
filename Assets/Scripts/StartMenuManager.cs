using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuManager : MonoBehaviour
{
    public GameObject creditsPanel;

    public void ToggleCredits()
    {
        bool isActive = creditsPanel.activeSelf;
        creditsPanel.SetActive(!isActive);
    }
    
    public void LoadMainGame()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }
}
