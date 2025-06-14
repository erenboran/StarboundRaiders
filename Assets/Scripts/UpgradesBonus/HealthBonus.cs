using UnityEngine;

public class HealthBonus : MonoBehaviour
{
    public int healthAmount = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.Heal(healthAmount);
            }

            Destroy(gameObject); // Bonus kaybolur
        }
    }
}
