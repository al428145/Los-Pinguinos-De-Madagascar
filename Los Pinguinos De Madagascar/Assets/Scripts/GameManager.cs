using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private bool gameEnded = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void WinGame()
    {

        gameEnded = true;
        SceneManager.LoadScene("final");
        
    }

    public void LoseGame()
    {
        if (gameEnded) return;

        gameEnded = true;
        StopGame();
        SceneManager.LoadScene("inicio");

    }

    private void StopGame()
    {
        Time.timeScale = 0f;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        gameEnded = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo.");
        Application.Quit();
    }
    public void inicio()
    {
        SceneManager.LoadScene("inicio"); // Usa el nombre exacto de tu escena principal
    }
    public void StartGame()
    {
        SceneManager.LoadScene("MainScene"); // Usa el nombre exacto de tu escena principal
    }

}