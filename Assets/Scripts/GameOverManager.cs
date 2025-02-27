using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    // Singleton
    public static GameOverManager instance;
    public GameObject gameOverPanel;
    public GameObject settingsButton;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void GameOver()
    {
        PlayerPrefs.SetInt("IsInGame", 0);
        PlayerPrefs.Save();

        FindObjectOfType<AudioManager>().ChangeMusic(FindObjectOfType<AudioManager>().menuMusic);
        settingsButton.SetActive(false);
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f; // Oyunu durdur
    }

    public void Retry()
    {
        PlayerPrefs.SetInt("IsInGame", 1);
        PlayerPrefs.Save();

        FindObjectOfType<AudioManager>().ChangeMusic(FindObjectOfType<AudioManager>().gameMusic);
        Time.timeScale = 1f; // Oyunu devam ettir
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Aynı sahneyi tekrar yükle
    }

    public void OpenShop()
    {
        gameOverPanel.SetActive(false);
        ShopManager.instance.OpenShop(); // Shop açılıyor
    }
}
