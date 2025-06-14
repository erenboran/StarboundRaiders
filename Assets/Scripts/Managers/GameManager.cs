using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int score = 0;
    public int coins = 0;
    public int highScore = 0;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI highScoreText;

    private GameObject currentShip;// Oyuncu gemisi
    void OnEnable()
    {
         int selectedShipIndex = PlayerPrefs.GetInt("SelectedShip", 0);
        Ship selectedShip = ShipSelectionManager.instance.availableShips[selectedShipIndex];
        SpawnShip(selectedShip);
    }
    private void Awake()
    {        

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        Application.targetFrameRate = 200; // 60 FPS hedefle
        QualitySettings.vSyncCount = 0;  // VSync kapat (Gerekirse)
    }

    void Start()
    {
        // HighScore ve Coin'leri kayıttan al
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        coins = PlayerPrefs.GetInt("TotalCoins", 0); // Önceki coin değerini çek

        score = 0;
        UpdateUI();

        // Seçilen gemiyi yükle

    }
    public void SaveData()
    {
        PlayerPrefs.SetInt("Coins", coins);
        PlayerPrefs.Save();
    }
    public void AddScore(int points)
    {
        score += points;
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
        }
        UpdateUI();
    }

    public int GetPoints()
    {
        return score;
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        PlayerPrefs.SetInt("TotalCoins", coins); // Yeni coin değerini kaydet
        UpdateUI();
    }

    void UpdateUI()
    {
        scoreText.text = "Score: " + score;
        coinText.text = "Coin: " + coins;
        highScoreText.text = "HighScore: " + highScore;
    }

    public void SpawnShip(Ship ship)
    {
        if (currentShip != null)
        {
            Destroy(currentShip);
        }

        currentShip = Instantiate(ship.shipPrefab, Vector3.zero, Quaternion.identity);
        currentShip.tag = "Player";
    }
}