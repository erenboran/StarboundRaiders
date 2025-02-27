using UnityEngine;

public class FireRateBonus : MonoBehaviour
{
    public float fireRateMultiplier = 1.2f; // FireRate %20 artırılacak
    public float duration = 10f; // 10 saniye sürecek

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerShooting playerShooting = other.GetComponent<PlayerShooting>();
            if (playerShooting != null)
            {
                playerShooting.ActivateFireRateBoost(fireRateMultiplier, duration);
            }
            Destroy(gameObject); // Bonus objesini yok et
        }
    }
}