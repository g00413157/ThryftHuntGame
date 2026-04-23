using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitButton : MonoBehaviour
{
    // Go back to main menu
    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartScene");
    }

    // Quit the game completely (only works in build)
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}