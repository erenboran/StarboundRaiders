using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipSelectionManager : MonoBehaviour
{
    public static ShipSelectionManager instance;

    public Ship[] availableShips; // Scriptable Object'teki gemiler
    public Transform shipSlots; // UI'daki butonların bulunduğu yer (GridLayoutGroup)
    public GameObject shipButtonPrefab; // Buton Prefab (Gemi göstermek için)

    public TMP_Text coinText; // Coinleri gösterecek UI
    private int selectedShipIndex = 0;

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
    }

    void Start()
    {
        LoadData();
        UpdateUI();
        PopulateShipButtons();
    }

    void Update()
    {
        UpdateUI();
    }
    void PopulateShipButtons()
    {
        // Önceki butonları temizle
        foreach (Transform child in shipSlots)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < availableShips.Length; i++)
        {
            GameObject buttonObj = Instantiate(shipButtonPrefab, shipSlots);
            Button button = buttonObj.GetComponent<Button>();
            int shipIndex = i; // Lambda içinde doğru index kullanmak için

            // Butona gemiyi yükle
            Image shipImage = buttonObj.transform.GetComponent<Image>(); // İlk child bir Image olmalı
            shipImage.sprite = availableShips[i].shipSprite; // Geminin resmini ayarla
            TMP_Text shipName = buttonObj.transform.GetComponentInChildren<TMP_Text>(); // İlk child bir Text olmalı    
            shipName.text = availableShips[i].shipName; // Geminin adını ayarla
                                                        // Gemi özelliklerini yazdır
            TMP_Text[] texts = buttonObj.GetComponentsInChildren<TMP_Text>();
            foreach (TMP_Text text in texts)
            {
                if (text.name == "Damage/FireRate/Health")
                {
                    text.text = $"Damage: {availableShips[i].shipDamage}\nFire Rate: {availableShips[i].shipFireRate}\nHealth: {availableShips[i].shipHealth}";
                }
                // Gemi kilidi açıksa fiyatı yazma
                if (text.name == "Price")
                {
                    if (availableShips[i].isUnlocked)
                    {
                        text.text = "Unlocked";
                    }
                    else
                    {
                        text.text = availableShips[i].price.ToString();
                    }
                }
            }
            // Seçilen geminin butonunu gri yap
            if (i == selectedShipIndex)
            {
                buttonObj.GetComponent<Image>().color = Color.gray;
            }

            button.onClick.AddListener(() => OnShipButtonClick(shipIndex));
        }
    }

    void OnShipButtonClick(int index)
    {
        Ship shipData = availableShips[index];

        if (!shipData.isUnlocked)
        {
            BuyShip(index);
        }
        else
        {
            SelectShip(index);
        }
    }

    void BuyShip(int index)
    {
        Ship shipData = availableShips[index];

        if (GameManager.instance.coins >= shipData.price)
        {
            GameManager.instance.coins -= shipData.price;
            PlayerPrefs.SetInt("TotalCoins", GameManager.instance.coins);
            shipData.isUnlocked = true;
            PlayerPrefs.SetInt("ShipUnlocked_" + index, 1);
            GameManager.instance.SaveData();
            UpdateUI();
            PopulateShipButtons(); // UI'yı güncelle

        }
    }

    void SelectShip(int index)
    {
        selectedShipIndex = index;
        PlayerPrefs.SetInt("SelectedShip", selectedShipIndex);
        PlayerPrefs.Save();
        PopulateShipButtons();

    }

    void LoadData()
    {
        selectedShipIndex = PlayerPrefs.GetInt("SelectedShip", 0);

        for (int i = 0; i < availableShips.Length; i++)
        {
            if (PlayerPrefs.GetInt("ShipUnlocked_" + i, 0) == 1)
            {
                availableShips[i].isUnlocked = true;
            }
        }
    }

    void UpdateUI()
    {
        coinText.text = "Coins: " + PlayerPrefs.GetInt("TotalCoins", 0);
    }
}