using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
    private Vector2 startTouchPosition, endTouchPosition;
    private bool isSwiping = false;
    
    public float moveSpeed = 5f; 
    public float forwardSpeed = 10f; // Otomatik ilerleme hızı
    public float swipeThreshold = 50f; // Minimum swipe mesafesi

    public GameObject bulletPrefab; // Mermi Prefab
    public Transform firePoint; // Merminin çıkış noktası
    public float fireRate = 0.5f; // Ateş etme süresi
    private float nextFireTime = 0f;

    void Update()
    {
        HandleTouchInput();
        MoveForward();
        AutoFire();
    }

    void MoveForward()
    {
        transform.position += Vector3.forward * forwardSpeed * Time.deltaTime;
    }

    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startTouchPosition = touch.position;
                    isSwiping = true;
                    break;

                case TouchPhase.Moved:
                    if (isSwiping)
                    {
                        endTouchPosition = touch.position;
                        Vector2 swipeDelta = endTouchPosition - startTouchPosition;

                        if (swipeDelta.magnitude > swipeThreshold)
                        {
                            DetectSwipeDirection(swipeDelta);
                            isSwiping = false; // Bir kere swipe yaptıktan sonra sıfırla
                        }
                    }
                    break;

                case TouchPhase.Ended:
                    isSwiping = false;
                    break;
            }
        }
    }

    void DetectSwipeDirection(Vector2 swipeDelta)
    {
        if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
        {
            if (swipeDelta.x > 0)
                Move(Vector3.right);
            else
                Move(Vector3.left);
        }
        else
        {
            if (swipeDelta.y > 0)
                Move(Vector3.up);
            else
                Move(Vector3.down);
        }
    }

    void Move(Vector3 direction)
    {
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    void AutoFire()
    {
        if (Time.time >= nextFireTime)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            nextFireTime = Time.time + fireRate;
        }
    }
}
