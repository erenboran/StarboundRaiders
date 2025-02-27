using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel; // Pause menüsü paneli
    public GameObject gameOverPanel; // Game Over Paneli
    private GameObject playerCanvas; // Player içindeki Canvas
    [SerializeField] private GameObject settingsButton; // Player içindeki Canvas
    private GameObject player; // Player referansı
    private bool isPaused = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerCanvas = player.GetComponentInChildren<Canvas>().gameObject;
        }

        pausePanel.SetActive(false); 
       gameOverPanel.SetActive(false);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
            if (playerCanvas) playerCanvas.SetActive(false);
        }
        else
        {
            Time.timeScale = 1f;
            pausePanel.SetActive(false);
            if (playerCanvas) playerCanvas.SetActive(true);
        }
    }

    public void ExitGame()
    {
            PlayerPrefs.SetInt("IsInGame", 0);
          PlayerPrefs.Save();
    
    FindObjectOfType<AudioManager>().ChangeMusic(FindObjectOfType<AudioManager>().menuMusic);
        Time.timeScale = 0f; // Oyunu durdur
        if (player) Destroy(player); // Player'ı yok et
        pausePanel.SetActive(false); // Pause menüsünü kapat
        gameOverPanel.SetActive(true); // Game Over panelini aç
        settingsButton.SetActive(false); // Ayarlar butonunu kapat
    }
}
