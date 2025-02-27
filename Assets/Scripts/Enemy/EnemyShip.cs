using UnityEngine;

public class EnemyShip : MonoBehaviour
{
    public GameObject bulletPrefab; // Düşman mermisi prefabı
    public Transform firePoint; // Merminin çıkış noktası
    public float fireRate = 2f; // Ateş etme sıklığı
    public int damage = 10; // Verilecek hasar
    public float bulletSpeed = 10f; // Mermi hızı
    public AudioClip shootSound; // Ateş etme sesi

    private float nextFireTime;

    void Update()
    {
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + (1f / fireRate);
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = transform.forward * 2 * bulletSpeed; // İleriye doğru ateş et

        AudioSource.PlayClipAtPoint(shootSound, transform.position);
    }
}