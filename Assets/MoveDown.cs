using UnityEngine;

public class MoveDown : MonoBehaviour
{
    public float moveSpeed = 5f;
    public bool isEnemy = false;
    private GameManager gameManager;
    private int lastCheckedPoints = 0;

    void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager bulunamadı!");
        }
    }

    void Update()
    {
        if (gameManager != null)
        {
            int currentPoints = GameManager.instance.GetPoints();
            if (currentPoints >= lastCheckedPoints + 200)
            {
                moveSpeed += 1f; // Hızı artır
                lastCheckedPoints = currentPoints;
            }
        }

        if (isEnemy)
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime); // Z ekseninde geriye git
        }
    }
}