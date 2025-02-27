using UnityEngine;

public class HyperCasualCamera : MonoBehaviour
{
    public Transform target; // Takip edilecek obje (Uzay Gemisi)
    public Vector3 offset = new Vector3(0, 10, -10); // Yukarıdan bakış açısı
    public float smoothSpeed = 0.2f; // Hafif gecikme efekti (İsteğe bağlı)

    void LateUpdate()
    {
        if (target == null) return;

        // Kamerayı gemiye uzak tutup sabit bir açıda yukarıdan gösteriyoruz
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Kameranın aşağıya bakmasını sağlıyoruz
        transform.rotation = Quaternion.Euler(30f, 0f, 0f); 
    }
}
