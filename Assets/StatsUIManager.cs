using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatsUIManager : MonoBehaviour
{
    private PlayerShooting playerShooting;

    public TMP_Text damageText;
    public TMP_Text fireRateText;
    public Image fireRateBonusImage; // Fire Rate süresini gösterecek Image

    private float fireRateBonusDuration = 0f; // Süreyi takip etmek için

    void Start()
    {
        // Player objesini bul ve PlayerShooting scriptini al
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerShooting = player.GetComponent<PlayerShooting>();
        }

        // Başlangıçta Fire Rate Bonus barını kapalı yap
        if (fireRateBonusImage != null)
        {
            fireRateBonusImage.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (playerShooting != null)
        {
            // **SADECE SAYILAR GÖRÜNSÜN**
            damageText.text = playerShooting.damage.ToString();            
            fireRateText.text = (playerShooting.fireRate * 4).ToString();
        }

        if (fireRateBonusDuration > 0)
        {
            fireRateBonusDuration -= Time.deltaTime;
            fireRateBonusImage.fillAmount = fireRateBonusDuration / playerShooting.fireRateBonusMaxTime;

            if (fireRateBonusDuration <= 0)
            {
                fireRateBonusImage.gameObject.SetActive(false);
            }
        }
    }

    public void ShowFireRateBonus(float duration)
    {
        fireRateBonusDuration = duration;

        if (fireRateBonusImage != null)
        {
            fireRateBonusImage.gameObject.SetActive(true);
            fireRateBonusImage.fillAmount = 1f;
        }
    }
}
