using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private bool gameEnded = false;

    [Header("🎶 Música de inicio y final")]
    public AudioClip menuMusic;   // misma canción para inicio y final
    [Range(0f, 1f)] public float menuMusicVolume = 0.5f;
    private AudioSource menuMusicSource;

    [Header("🎶 Música de fondo")]
    public AudioClip mainSceneMusic; // Asigna desde el inspector
    [Range(0f, 1f)] public float mainMusicVolume = 0.5f;

    [Header("🌧️ Sonido de lluvia")]
    public AudioClip rainClip; // Asigna desde el inspector
    [Range(0f, 1f)] public float rainVolume = 0.2f; // más bajo que la música
    private AudioSource musicSource;
    private AudioSource rainSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // 🎵 Crear fuentes de audio
        musicSource = gameObject.AddComponent<AudioSource>();
        rainSource = gameObject.AddComponent<AudioSource>();
        menuMusicSource = gameObject.AddComponent<AudioSource>(); // 🔧 CORRECCIÓN: inicializar AudioSource

        // Configurar música principal
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = mainMusicVolume;

        // Configurar sonido de lluvia
        rainSource.loop = true;
        rainSource.playOnAwake = false;
        rainSource.volume = rainVolume;

        // Música de menú (inicio/final)
        menuMusicSource.loop = true;
        menuMusicSource.playOnAwake = false;
        menuMusicSource.volume = menuMusicVolume;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Detener todas las fuentes primero
        StopAllMusic();

        if (scene.name == "MainScene")
        {
            PlayMainMusic();
        }
        else if (scene.name == "inicio" || scene.name == "final")
        {
            PlayMenuMusic();
        }
    }

    private void PlayMainMusic()
    {
        if (mainSceneMusic != null)
        {
            musicSource.clip = mainSceneMusic;
            musicSource.volume = mainMusicVolume;
            musicSource.Play();
        }

        if (rainClip != null)
        {
            rainSource.clip = rainClip;
            rainSource.volume = rainVolume;
            rainSource.Play();
        }

        Debug.Log("🎵 Música y lluvia iniciadas en MainScene");
    }

    private void PlayMenuMusic()
    {
        if (menuMusic != null)
        {
            menuMusicSource.clip = menuMusic;
            menuMusicSource.volume = menuMusicVolume;
            menuMusicSource.Play();
            Debug.Log("🎵 Música de menú iniciada");
        }
    }

    private void StopAllMusic()
    {
        if (musicSource.isPlaying) musicSource.Stop();
        if (rainSource.isPlaying) rainSource.Stop();
        if (menuMusicSource.isPlaying) menuMusicSource.Stop();
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
        SceneManager.LoadScene("inicio");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("MainScene");
    }
}
