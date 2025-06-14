using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float forwardSpeed = 10f; // Sürekli ilerleme hızı (Z ekseni)
    public float laneDistance = 5f; // Koridor mesafesi
    public float moveSpeed = 10f; // Yumuşak geçiş hızı

    private int currentLane = 1; // Başlangıçta orta koridor (0 = Sol, 1 = Orta, 2 = Sağ)
    private Vector3 targetPosition;
    
    private Vector2 touchStartPos;
    private bool isSwiping = false;

    void Start()
    {
        targetPosition = transform.position; // Başlangıç pozisyonu
    }

    void Update()
    {
        // Gemi HER ZAMAN İLERİ GİDER
        Vector3 forwardMove = Vector3.forward * forwardSpeed * Time.deltaTime;
        transform.Translate(forwardMove);

        SwipeCheck();

        // Yumuşak geçiş için pozisyonu güncelle
        Vector3 newPosition = Vector3.Lerp(transform.position, new Vector3(targetPosition.x, transform.position.y, transform.position.z), Time.deltaTime * moveSpeed);
        transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z + forwardMove.z);
    }

    void SwipeCheck()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
                isSwiping = true;
            }
            else if (touch.phase == TouchPhase.Ended && isSwiping)
            {
                float swipeDeltaX = touch.position.x - touchStartPos.x;

                if (swipeDeltaX > 50 && currentLane < 2) // Sağa kay
                {
                    currentLane++;
                    UpdateLane();
                }
                else if (swipeDeltaX < -50 && currentLane > 0) // Sola kay
                {
                    currentLane--;
                    UpdateLane();
                }

                isSwiping = false;
            }
        }
    }

    void UpdateLane()
    {
        // Yeni hedef pozisyonu belirle (X ekseninde kaydır)
        targetPosition = new Vector3((currentLane - 1) * laneDistance, transform.position.y, transform.position.z);
    }
}