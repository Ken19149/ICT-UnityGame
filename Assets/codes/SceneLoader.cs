using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string levelSceneName = "Level1";
    [SerializeField] private string creditsSceneName = "Credits";

    // Called by Play button
    public void LoadLevel()
    {
        SceneManager.LoadScene(levelSceneName);
    }

    // Called by Credits button
    public void LoadCredits()
    {
        SceneManager.LoadScene(creditsSceneName);
    }

    // Called by Back button in Credits / Ending
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // Called by Quit button
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
