using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] spawnObjects; // Enemy & Meteor prefabları burada olacak
    public GameObject[] rareSpawnObjects; // Nadir düşman prefabları burada olacak
    public GameObject[] bonusObjects; // Bonus prefabları burada olacak
    public GameObject[] rareBonusObjects; // Nadir bonus prefabları burada olacak
    public Transform[] spawnPoints; // Spawn noktaları (X ve Y belirli, Z otomatik ayarlanacak)
    public float spawnInterval = 2f;
    public float rareSpawnInterval = 15f; // Nadir düşmanların spawn olma aralığı
    public float bonusSpawnInterval = 10f;
    public float rareBonusSpawnInterval = 30f; // Nadir bonusların spawn olma aralığı
    public float spawnSpeedIncrease = 0.1f;
    public Transform player; // Oyuncunun referansı
    public float zOffset = 50f; // Oyuncunun önünde kalma mesafesi

    private float nextSpawnTime;
    private float nextRareSpawnTime;
    private float nextBonusSpawnTime;
    private float nextRareBonusSpawnTime;
    private int lastSpawnPointIndex = -1; // Son kullanılan spawn noktası

    private GameManager gameManager;

    void Awake()
    {
        gameManager = GameManager.instance;
    }

    void Start()
    {
                player = GameObject.FindGameObjectWithTag("Player").transform;

        nextRareSpawnTime = Time.time + rareSpawnInterval; // Nadir düşmanların spawnlanma zamanını ayarla
        nextBonusSpawnTime = Time.time + bonusSpawnInterval - 3;
        nextRareBonusSpawnTime = Time.time + rareBonusSpawnInterval - 3;
    }

    void Update()
    {
        // Spawner'ı oyuncunun önüne taşı
        if (player != null)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, player.position.z + zOffset);
        }

        if (Time.time >= nextSpawnTime)
        {
            SpawnObject();
            nextSpawnTime = Time.time + spawnInterval;
            spawnInterval = Mathf.Max(0.5f, spawnInterval - spawnSpeedIncrease);
        }

        if (Time.time >= nextRareSpawnTime)
        {
            SpawnRareObject();
            nextRareSpawnTime = Time.time + rareSpawnInterval;
        }

        if (Time.time >= nextBonusSpawnTime)
        {
            SpawnBonus();
            nextBonusSpawnTime = Time.time + bonusSpawnInterval;
        }

        if (Time.time >= nextRareBonusSpawnTime)
        {
            SpawnBonusRare();
            nextRareBonusSpawnTime = Time.time + rareBonusSpawnInterval;
        }

        AdjustRareSpawnInterval();
    }

    void SpawnObject()
    {
        int randomIndex = Random.Range(0, spawnObjects.Length);
        int randomPoint = GetNextSpawnPoint();

        Vector3 spawnPosition = new Vector3(spawnPoints[randomPoint].position.x, spawnPoints[randomPoint].position.y, transform.position.z);
        GameObject newObject = Instantiate(spawnObjects[randomIndex], spawnPosition, Quaternion.Euler(0, -180, 0));
    }

    void SpawnRareObject()
    {
        int randomIndex = Random.Range(0, rareSpawnObjects.Length);
        int randomPoint = GetNextSpawnPoint();

        Vector3 spawnPosition = new Vector3(spawnPoints[randomPoint].position.x, spawnPoints[randomPoint].position.y, transform.position.z);
        GameObject newObject = Instantiate(rareSpawnObjects[randomIndex], spawnPosition, Quaternion.Euler(0, -180, 0));
    }

    void SpawnBonus()
    {
        int randomIndex = Random.Range(0, bonusObjects.Length);
        int randomPoint = GetNextSpawnPoint();

        Vector3 spawnPosition = new Vector3(spawnPoints[randomPoint].position.x, spawnPoints[randomPoint].position.y, transform.position.z);
        Instantiate(bonusObjects[randomIndex], spawnPosition, Quaternion.identity);
    }

    void SpawnBonusRare()
    {
        int randomIndex = Random.Range(0, rareBonusObjects.Length);
        int randomPoint = GetNextSpawnPoint();

        Vector3 spawnPosition = new Vector3(spawnPoints[randomPoint].position.x, spawnPoints[randomPoint].position.y, transform.position.z);
        Instantiate(rareBonusObjects[randomIndex], spawnPosition, Quaternion.identity);
    }

    int GetNextSpawnPoint()
    {
        int randomPoint;
        do
        {
            randomPoint = Random.Range(0, spawnPoints.Length);
        } while (randomPoint == lastSpawnPointIndex);

        lastSpawnPointIndex = randomPoint;
        return randomPoint;
    }

    void AdjustRareSpawnInterval()
    {
        if (gameManager != null)
        {
            int currentPoints = gameManager.GetPoints();
            if (currentPoints >= 200)
            {
                rareSpawnInterval = 15f; // Puan 1000'i geçtiğinde nadir düşmanların spawn aralığını azalt
            }
            if (currentPoints >= 500)
            {
                rareSpawnInterval = 10f; // Puan 2000'i geçtiğinde nadir düşmanların spawn aralığını daha da azalt
            }
            if (currentPoints >= 700)
            {
                rareSpawnInterval = 5f; // Puan 3000'i geçtiğinde nadir düşmanların spawn aralığını daha da azalt
            }
        }
    }
}