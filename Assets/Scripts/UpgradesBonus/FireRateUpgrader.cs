using UnityEngine;

public class FireRateUpgrader : MonoBehaviour
{
    public float additionalFireRate = 0.1f; // Her bonus alındığında eklenecek FireRate

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerShooting playerShooting = other.GetComponent<PlayerShooting>();
            if (playerShooting != null)
            {
                playerShooting.ActivateFireRateUpgrade(additionalFireRate);
            }
            Destroy(gameObject); // Bonus objesini ySok et
        }
    }
}