using UnityEngine;

public class BackgroundParallax : MonoBehaviour
{
    public Transform player;  // Gemi
    public float parallaxSpeed = 0.1f; // Hareket miktarı
    public float zOffset = 50f; // Oyuncunun önünde kalma mesafesi

    private Vector3 startPosition;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("Player bulunamadı! 'Player' tag'ine sahip bir GameObject olduğundan emin olun.");
        }

        startPosition = transform.position;
    }
    void OnEnable()
    {
       // player = GameObject.FindGameObjectWithTag("Player").transform;

    }
    void Awake()
    {
        //player = GameObject.FindGameObjectWithTag("Player").transform;

        startPosition = transform.position;
    }

    void Update()
    {
        if (player != null)
        {
            // Arka planı oyuncunun önünde tut ve yumuşak bir şekilde hareket ettir
            Vector3 targetPosition = new Vector3(player.position.x * parallaxSpeed, player.position.y * parallaxSpeed, player.position.z + zOffset);
            transform.position = Vector3.Lerp(transform.position, targetPosition, parallaxSpeed * Time.deltaTime);
        }
    }
}