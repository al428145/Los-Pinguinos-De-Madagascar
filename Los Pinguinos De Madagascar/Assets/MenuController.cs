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
        Debug.Log("El juego se cerrar�a aqu� (solo funciona en build).");
    }
}
