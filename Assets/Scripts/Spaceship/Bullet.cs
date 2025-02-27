using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    public float speed = 20f; // Mermi hızı
    public float lifetime = 5f; // 5 saniye sonra yok ol
    public float damage = 10; // Hasar değeri (Gemiye göre değişir)
    public bool isPiercing = false; // Bu özellik aktifse mermi yok olmaz
    public bool isFancyBullet = false; // Bu özellik aktifse mermi yok olmaz
    private Rigidbody rb;
    public BurnEffect burnEffectPrefab; // Yanma efekti prefabı

    void Awake()
    {
        if (isFancyBullet)
        {

            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.forward * speed; // Z ekseninde ileriye git
        Destroy(gameObject, lifetime); // 5 saniye sonra yok ol
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);

                // Eğer yanma efekti aktifse düşmana uygula
                if (burnEffectPrefab != null)
                {
                    BurnEffect burnEffect = Instantiate(burnEffectPrefab, enemy.transform.position, Quaternion.identity);
                    burnEffect.ApplyBurn(enemy);
                }
            }

            if (!isPiercing) 
            {
                Destroy(gameObject);
            }
        }
    }
}