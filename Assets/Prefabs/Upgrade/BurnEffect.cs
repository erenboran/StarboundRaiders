using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BurnEffect : MonoBehaviour
{
    public float burnDuration = 5f;  // Yanma süresi
    public int burnDamage = 2;  // Her saniye kaç hasar verecek
    public GameObject fireIconPrefab; // UI Ateş simgesi prefabı

    private Enemy enemy;
    private GameObject fireIconInstance;
    private Renderer enemyRenderer;
    private List<Color> originalColors = new List<Color>(); // Tüm materyallerin renklerini sakla

    public void ApplyBurn(Enemy target)
    {
        enemy = target;
        enemyRenderer = enemy.GetComponent<Renderer>();

        if (enemyRenderer != null)
        {
            // Enemy'nin tüm materyallerini kontrol et
            foreach (var mat in enemyRenderer.materials)
            {
                originalColors.Add(mat.color); // Orijinal rengi sakla
                mat.color = Color.red; // Kırmızıya boya
            }
        }

        // UI Ateş simgesini oluştur
        if (fireIconPrefab != null)
        {
            fireIconInstance = Instantiate(fireIconPrefab, enemy.transform.position + Vector3.up * 2f, Quaternion.identity, enemy.transform);
        }

        StartCoroutine(BurnCoroutine());
    }

    IEnumerator BurnCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < burnDuration)
        {
            if (enemy != null)
            {
                enemy.TakeDamage(burnDamage); // Her saniye hasar ver
            }

            elapsed += 1f;
            yield return new WaitForSeconds(1f);
        }

        // Yanma efekti bittiğinde rengi geri döndür
        if (enemyRenderer != null)
        {
            for (int i = 0; i < enemyRenderer.materials.Length; i++)
            {
                enemyRenderer.materials[i].color = originalColors[i]; 
            }
        }

        // UI ikonu sil
        if (fireIconInstance != null)
        {
            Destroy(fireIconInstance);
        }

        Destroy(gameObject);
    }
}