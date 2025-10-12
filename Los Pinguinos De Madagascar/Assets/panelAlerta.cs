using UnityEngine;
using UnityEngine.UI;

public class AlertPanelController : MonoBehaviour
{
    public Image panelImage;      // Arrastra aquí el panel que quieres que parpadee
    public float minAlpha = 0.2f;
    public float maxAlpha = 1f;
    public float pulseSpeed = 2f;

    private bool increasing = true;
    private bool alertActive = false;

    void Update()
    {
        if (!alertActive || panelImage == null) return;

        Color color = panelImage.color;

        if (increasing)
        {
            color.a += pulseSpeed * Time.deltaTime;
            if (color.a >= maxAlpha)
            {
                color.a = maxAlpha;
                increasing = false;
            }
        }
        else
        {
            color.a -= pulseSpeed * Time.deltaTime;
            if (color.a <= minAlpha)
            {
                color.a = minAlpha;
                increasing = true;
            }
        }

        panelImage.color = color;
    }

    public void ShowAlert()
    {
        if (panelImage != null)
            panelImage.gameObject.SetActive(true);
        alertActive = true;
    }

    public void HideAlert()
    {
        if (panelImage != null)
            panelImage.gameObject.SetActive(false);
        alertActive = false;
    }
}
