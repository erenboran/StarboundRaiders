using UnityEngine;

public class SmoothFollowCamera : MonoBehaviour
{
    public Transform target; // Gemi
    public Vector3 offset = new Vector3(0, 10, -10); // Kamera uzaklığı
    public float smoothTime = 0.2f; // Geçiş süresi (daha büyükse daha yumuşak geçiş)

    private Vector3 velocity = Vector3.zero;
    void Start()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }
    void LateUpdate()
    {
        if (target == null) return;

        // Kamera hedefin biraz gerisinden gelsin (hareket hissi verir)
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}