using UnityEngine;
using UnityEngine.UI; 
using TMPro;      
using System.Collections; 

public class PlayerCapture : MonoBehaviour
{
    public Image panelFundido;      // Arrastra tu PanelFundido aquí
    public TextMeshProUGUI textoCapturado; // Arrastra tu TextoCapturado aquí
    public AudioSource sonidoJaula;   // Arrastra el objeto que tiene el AudioSource
    public float duracionFundido = 2f; // Segundos que tardará en fundirse

    private bool yaCapturado = false; // Para evitar que se active varias veces

    // Esta función detecta la colisión con un enemigo
    // Asegúrate de que tu enemigo tenga un Collider y el Tag "Enemy"
    private void OnTriggerEnter(Collider other)
    {
        // Comprueba si el objeto con el que chocamos tiene el tag "Enemy"
        if (other.CompareTag("Enemy") && !yaCapturado)
        {
            IniciarCaptura();
        }
    }

    // También puedes usar OnCollisionEnter si tus colisiones no son "Trigger"
    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.CompareTag("Dog") || collision.gameObject.CompareTag("Guard")) && !yaCapturado)
        {
            IniciarCaptura();
        }
    }

    // Función pública por si quieres llamarla desde otro script (ej. el enemigo)
    public void IniciarCaptura()
    {
        if (yaCapturado) return; // Si ya estamos capturados, no hacemos nada

        yaCapturado = true;

        // (Opcional) Desactivar el movimiento del jugador
        // GetComponent<PlayerMovement>().enabled = false; 

        // Inicia la secuencia de captura
        StartCoroutine(SecuenciaDeCaptura());
    }

    // Una Corutina nos permite ejecutar acciones a lo largo del tiempo (como un fundido)
    IEnumerator SecuenciaDeCaptura()
    {
        // 1. Reproducir el sonido
        if (sonidoJaula != null)
        {
            sonidoJaula.Play();
        }

        // 2. Iniciar el fundido a negro
        float tiempoPasado = 0f;
        Color colorPanel = panelFundido.color; // Empezamos desde transparente (Alpha 0)

        while (tiempoPasado < duracionFundido)
        {
            // Aumentamos el alfa gradualmente
            colorPanel.a = Mathf.Lerp(0, 1, tiempoPasado / duracionFundido);
            panelFundido.color = colorPanel;

            tiempoPasado += Time.deltaTime; // Sumamos el tiempo de este frame
            yield return null; // Esperamos al siguiente frame
        }

        // Aseguramos que quede totalmente negro
        colorPanel.a = 1f;
        panelFundido.color = colorPanel;

        // 3. Mostrar el texto
        if (textoCapturado != null)
        {
            textoCapturado.gameObject.SetActive(true);
        }

      yield return new WaitForSeconds(3f); 
      UnityEngine.SceneManagement.SceneManager.LoadScene("Inicio");
    }
}