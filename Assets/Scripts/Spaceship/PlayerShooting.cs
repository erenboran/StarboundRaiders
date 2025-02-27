using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab; // Mermi prefabı
    public Transform firePoint; // Merminin çıkış noktası
    public Transform secondFirePoint; // İkinci merminin çıkış noktası
    public float fireRate = 1f; // Atış hızı (saniyede kaç atış)
    public float damage = 10; // Verilecek hasar
    public AudioClip shootSound; // Mermi sesi
    private AudioSource audioSource; // AudioSource bileşeni
    private float baseFireRate;
    private bool isFireRateBoosted = false;
    private bool isBurnShot = false; // Yanma bonusu aktif mi?
    private float nextFireTime = 0f;
    public bool isPiercing = false;
    public BurnEffect burnEffectPrefab; // Yanma efekti prefabı
    public float fireRateBonusMaxTime = 10f; // Maksimum süre
    public bool isTwoFirePoint = false; // İki fire pointten ateş etme

    void Start()
    {
        baseFireRate = fireRate;
        audioSource = gameObject.AddComponent<AudioSource>(); // AudioSource bileşenini ekle
    }

    void Update()
    {
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + (1f / fireRate); // Ateş hızına göre bekleme süresi
        }
    }

    void Shoot()
    {
        FireBullet(firePoint);

        if (isTwoFirePoint && secondFirePoint != null)
        {
            FireBullet(secondFirePoint);
        }

        // Mermi sesini çal ve pitch değerini rastgele ayarla
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.PlayOneShot(shootSound);
    }

    void FireBullet(Transform firePoint)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0, 90, 90));
        Bullet bulletComponent = bullet.GetComponent<Bullet>();
        bulletComponent.damage = damage; // Merminin hasarını ayarla
        bulletComponent.isPiercing = isPiercing; // Merminin isPiercing özelliğini ayarla

        // Yanma bonusu aktifse BurnEffect prefabını mermiye aktar
        if (isBurnShot)
        {
            bulletComponent.burnEffectPrefab = burnEffectPrefab;
        }
    }

    public void ActivateBurnBonus(float duration)
    {
        isBurnShot = true;
        Invoke(nameof(ResetBurnBonus), duration);
    }

    void ResetBurnBonus()
    {
        isBurnShot = false;
    }

    public void ActivatePiercingBonus(float duration)
    {
        isPiercing = true;
        Invoke(nameof(ResetPiercing), duration);
    }

    void ResetPiercing()
    {
        isPiercing = false;
    }

    public void ActivateFireRateBoost(float multiplier, float duration)
    {
        if (isFireRateBoosted) return; // Zaten aktifse bir daha çalıştırma

        isFireRateBoosted = true;
        fireRate *= multiplier;

        // UI'ya bildir
        FindObjectOfType<StatsUIManager>()?.ShowFireRateBonus(duration);

        Invoke(nameof(ResetFireRate), duration);
    }

    public void ActivateFireRateUpgrade(float additionalFireRate)
    {
        baseFireRate += additionalFireRate; // Base FireRate artır
        if (!isFireRateBoosted)
        {
            fireRate = baseFireRate; // Eğer boost aktif değilse fireRate'i güncelle
        }
    }

    public void ActivatePermanentDamageBonus(float additionalDamage)
    {
        damage += additionalDamage; // Süresiz hasar artır
    }

    private void ResetFireRate()
    {
        fireRate = baseFireRate;
        isFireRateBoosted = false;
    }
}