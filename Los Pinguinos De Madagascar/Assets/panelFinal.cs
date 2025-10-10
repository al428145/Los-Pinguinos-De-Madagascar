using UnityEngine;
using UnityEngine.UI;

public class PanelFadeTest : MonoBehaviour
{
    public Image panel;

    void Start()
    {
        // Inicializa transparente
        Color c = panel.color;
        c.a = 0f;
        panel.color = c;

        // Haz fade-in rápidamente
        StartCoroutine(FadeInPanel(1f)); // 1 segundo
    }

    System.Collections.IEnumerator FadeInPanel(float duration)
    {
        float t = 0f;
        Color c = panel.color;
        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, t / duration);
            panel.color = c;
            yield return null;
        }
        c.a = 1f;
        panel.color = c;
    }
}
