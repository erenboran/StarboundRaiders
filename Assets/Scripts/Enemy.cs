using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float health = 30; // Düşmanın canı
    public Slider healthBar; // Can barı slider'ı
    public GameObject explosionEffect;
    public AudioClip explosionSound;
    public int scoreValue = 10;
    public int coinValue = 5;
    void Start()
    {
        if (healthBar != null)
        {
            healthBar.maxValue = health;
            healthBar.value = health;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        }

    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (healthBar != null)
        {
            healthBar.value = health;
        }

        if (health <= 0)
        {

            GameManager.instance.AddScore(scoreValue); // Puan ekle

            GameManager.instance.AddCoins(coinValue);
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

            Destroy(gameObject); // Can 0 olursa düşmanı yok et
        }
    }
}