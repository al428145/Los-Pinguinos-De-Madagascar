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
    }

    public void WinGame()
    {
        if (gameEnded) return;

        gameEnded = true;
        Debug.Log("Pantalla de victoria");
        StopGame();
    }

    public void LoseGame()
    {
        if (gameEnded) return;

        gameEnded = true;
        Debug.Log("Pantalla de derrota");
        StopGame();
    }

    private void StopGame()
    {
        Time.timeScale = 0f;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo.");
        Application.Quit();
    }
}