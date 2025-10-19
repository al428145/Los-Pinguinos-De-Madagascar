using UnityEngine;

public class MenuController : MonoBehaviour
{
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void StartGame()
    {
        GameManager.Instance?.StartGame();
    }

    public void RestartGame()
    {
        GameManager.Instance?.RestartLevel();
    }

    public void GoToMainMenu()
    {
        GameManager.Instance?.inicio();
    }

    public void QuitGame()
    {
        GameManager.Instance?.QuitGame();
    }
}
