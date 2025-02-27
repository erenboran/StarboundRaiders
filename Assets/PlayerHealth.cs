using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public Image healthBar; // UI'deki can barı

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        healthBar.fillAmount = currentHealth / maxHealth; // Canı güncelle
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount); // Max HP'yi geçme
        UpdateHealthBar();
    }
    void Die()
    {
        Debug.Log("Player Öldü!");
        GameOverManager.instance.GameOver(); // Oyuncuyu yok et (İstersen Game Over ekranı aç)
        gameObject.SetActive(false); // Oyuncuyu yok et (İstersen Game Over ekranı aç)
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Meteor"))
        {
            TakeDamage(20f); // Meteor'a çarpınca 20 hasar al
        }
        else if (other.CompareTag("Enemy"))
        {
            TakeDamage(20f); // Düşmana çarpınca direkt öl
        }
    }
}
