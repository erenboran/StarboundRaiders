using UnityEngine;

public class PiercingBonus : MonoBehaviour
{
    public float duration = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerShooting playerShooting = other.GetComponent<PlayerShooting>();

            if (playerShooting != null)
            {
                playerShooting.ActivatePiercingBonus(duration);
            }

            Destroy(gameObject);
        }
    }
}
