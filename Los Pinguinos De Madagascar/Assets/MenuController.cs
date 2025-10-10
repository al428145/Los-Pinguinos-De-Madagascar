using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("MainScene"); // Usa el nombre exacto de tu escena principal
    }

    public void ExitGame()
    {
        Application.Quit(); // (Opcional) para salir del juego
        Debug.Log("El juego se cerraría aquí (solo funciona en build).");
    }
}
