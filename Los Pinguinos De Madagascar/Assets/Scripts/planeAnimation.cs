﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // Para cambiar de escena

public class planeAnimation : MonoBehaviour
{
    [Header("Movimiento del avión")]
    public float firstTargetX = -260f;
    public float finalTargetX = -400f;
    public float moveSpeed = 10f;
    public float rotationSpeed = 2f;
    public float climbAngle = -25f;
    public float climbSpeed = 5f;

    [Header("Referencias UI (Panel y Texto)")]
    public Image panelImage;
    public Text uiText;
    public TMP_Text tmpText;
    public TMP_Text tmpText2;

    [Header("Botones final")]
    public Button replayButton;      // Botón de "Play Again"
    public Button mainMenuButton;    // Botón de "Main Menu"

    [Header("Tiempos de fade")]
    public float panelFadeDelay = 0.5f;
    public float panelFadeDuration = 0.5f;
    public float textFadeDelay = 1f;
    public float textFadeDuration = 0.5f;

    private bool isClimbing = false;
    private bool finished = false;

    void Start()
    {
        // Inicializar opacidad de panel y textos
        SetAlpha(panelImage, 0f);
        SetAlpha(uiText, 0f);
        SetAlpha(tmpText, 0f);
        SetAlpha(tmpText2, 0f);

        // Ocultar botones al inicio
        if (replayButton != null) replayButton.gameObject.SetActive(false);
        if (mainMenuButton != null) mainMenuButton.gameObject.SetActive(false);


    }

    void Update()
    {
        if (finished) return;
        Vector3 pos = transform.position;

        if (!isClimbing && pos.x > firstTargetX)
        {
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;
        }
        else if (!isClimbing)
        {
            StartCoroutine(RotateAndClimbSimultaneously());
            StartCoroutine(FadeSequence());
            isClimbing = true;
        }
    }

    IEnumerator RotateAndClimbSimultaneously()
    {
        Quaternion targetRot = Quaternion.Euler(climbAngle, transform.eulerAngles.y, transform.eulerAngles.z);

        while (transform.position.x > finalTargetX)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;
            transform.position += Vector3.up * climbSpeed * Time.deltaTime;
            yield return null;
        }

        finished = true;

        // Mostrar botones al final
        if (replayButton != null) replayButton.gameObject.SetActive(true);
        if (mainMenuButton != null) mainMenuButton.gameObject.SetActive(true);
    }

    IEnumerator FadeSequence()
    {
        yield return new WaitForSeconds(panelFadeDelay);
        if (panelImage != null)
            yield return StartCoroutine(FadeImage(panelImage, panelFadeDuration, 1f));

        yield return new WaitForSeconds(textFadeDelay - panelFadeDelay);
        if (uiText != null) yield return StartCoroutine(FadeText(uiText, textFadeDuration, 1f));
        if (tmpText != null) yield return StartCoroutine(FadeTMP(tmpText, textFadeDuration, 1f));
        if (tmpText2 != null) yield return StartCoroutine(FadeTMP(tmpText2, textFadeDuration, 1f));
    }

    // Métodos de fade
    void SetAlpha(Graphic g, float alpha)
    {
        if (g == null) return;
        Color c = g.color;
        c.a = alpha;
        g.color = c;
    }

    IEnumerator FadeImage(Image img, float duration, float targetAlpha)
    {
        yield return FadeGraphic(img, duration, targetAlpha);
    }

    IEnumerator FadeText(Text txt, float duration, float targetAlpha)
    {
        yield return FadeGraphic(txt, duration, targetAlpha);
    }

    IEnumerator FadeTMP(TMP_Text txt, float duration, float targetAlpha)
    {
        yield return FadeGraphic(txt, duration, targetAlpha);
    }

    IEnumerator FadeGraphic(Graphic g, float duration, float targetAlpha)
    {
        float startAlpha = g.color.a;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            Color c = g.color;
            c.a = Mathf.Lerp(startAlpha, targetAlpha, t / duration);
            g.color = c;
            yield return null;
        }
        Color finalC = g.color;
        finalC.a = targetAlpha;
        g.color = finalC;
    }


}