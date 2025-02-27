using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;
    public GameObject shopPanel;
    public GameObject gameOverPanel;

    private void Awake()
    {
        instance = this;
        shopPanel.SetActive(false); // Başlangıçta kapalı
    }

    public void OpenShop()
    {
        shopPanel.SetActive(true);
        gameOverPanel.SetActive(false);
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        gameOverPanel.SetActive(true);
    }
}
