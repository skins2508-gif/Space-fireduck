using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public Transform player;
    public Transform obstacleParent;
    public Transform borderParent;
    public float spawnInterval = 1.5f;
    public float spawnOutsidePadding = 1.5f;
    public float aimRandomness = 2f;
    public float spawnInsidePadding = 1.2f;
    public int minObstacleCount = 3;
    public int maxObstacleCount = 5;
    public int initialSpawnBatch = 1;
    public int maxSpawnBatch = 1;

    private Camera mainCamera;
    private float nextSpawnTime;
    private int currentSpawnBatch;

    void Start()
    {
        mainCamera = Camera.main;
        currentSpawnBatch = Mathf.Max(1, initialSpawnBatch);

        if (player == null)
        {
            GameObject playerObject = GameObject.Find("Player");
            if (playerObject != null)
                player = playerObject.transform;
        }

        if (borderParent == null)
        {
            GameObject borderObject = GameObject.Find("Borders");
            if (borderObject != null)
                borderParent = borderObject.transform;
        }
    }

    void Update()
    {
        if (obstaclePrefab == null || player == null || mainCamera == null)
            return;

        if (Time.time < nextSpawnTime)
            return;

        int liveObstacleCount = CountLiveObstacles();
        if (maxObstacleCount > 0 && liveObstacleCount >= maxObstacleCount)
        {
            nextSpawnTime = Time.time + spawnInterval;
            return;
        }

        int targetCount = Random.Range(minObstacleCount, maxObstacleCount + 1);
        int spawnCount = Mathf.Clamp(targetCount - liveObstacleCount, 0, 1);
        for (int i = 0; i < spawnCount; i++)
            SpawnObstacle();

        nextSpawnTime = Time.time + spawnInterval;
    }

    public void OnFireMeteorEaten()
    {
        currentSpawnBatch = Mathf.Min(currentSpawnBatch, maxSpawnBatch);
    }

    void SpawnObstacle()
    {
        Vector3 spawnPosition = GetRandomOnscreenPosition();
        GameObject obstacle = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity, obstacleParent);

        Obstacles movingObstacle = obstacle.GetComponent<Obstacles>();
        if (movingObstacle != null)
            movingObstacle.LaunchRandom();
    }

    Vector3 GetRandomOnscreenPosition()
    {
        float height = Mathf.Max(0f, mainCamera.orthographicSize - spawnInsidePadding);
        float width = Mathf.Max(0f, mainCamera.orthographicSize * mainCamera.aspect - spawnInsidePadding);
        Vector3 cameraPosition = mainCamera.transform.position;
        float x = Random.Range(-width, width);
        float y = Random.Range(-height, height);

        return new Vector3(cameraPosition.x + x, cameraPosition.y + y, 0f);
    }

    void IgnoreBorderCollisions(GameObject obstacle)
    {
        if (borderParent == null)
            return;

        Collider2D[] obstacleColliders = obstacle.GetComponentsInChildren<Collider2D>();
        Collider2D[] borderColliders = borderParent.GetComponentsInChildren<Collider2D>();

        foreach (Collider2D obstacleCollider in obstacleColliders)
        {
            foreach (Collider2D borderCollider in borderColliders)
                Physics2D.IgnoreCollision(obstacleCollider, borderCollider);
        }
    }

    int CountLiveObstacles()
    {
        if (obstacleParent == null)
            return FindObjectsByType<Obstacles>(FindObjectsInactive.Exclude).Length;

        int count = 0;
        for (int i = 0; i < obstacleParent.childCount; i++)
        {
            if (obstacleParent.GetChild(i).GetComponent<Obstacles>() != null)
                count++;
        }

        return count;
    }
}
