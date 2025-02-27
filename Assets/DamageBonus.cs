using UnityEngine;

public class DamageBonus : MonoBehaviour
{
    public float damageIncrease = 5;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerShooting playerShooting = other.GetComponent<PlayerShooting>();

            if (playerShooting != null)
            {
                playerShooting.ActivatePermanentDamageBonus(damageIncrease);
            }

            Destroy(gameObject);
        }
    }
}