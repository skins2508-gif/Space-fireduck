using UnityEngine;

public class ColdMeteorSpawner : MonoBehaviour
{
    public GameObject coldMeteorPrefab;
    public Transform meteorParent;
    public Transform borderParent;
    public float startSpawnInterval = 5f;
    public float minSpawnInterval = 1.5f;
    public float spawnOutsidePadding = 1.5f;
    public int startMaxMeteorCount = 2;
    public int maxMeteorCount = 12;
    public float startMinSpeed = 5f;
    public float startMaxSpeed = 7f;
    public float endMinSpeed = 5f;
    public float endMaxSpeed = 7f;
    public float difficultyRampSeconds = 120f;

    private Camera mainCamera;
    private float nextSpawnTime;
    private float startTime;

    void Start()
    {
        mainCamera = Camera.main;
        startTime = Time.time;

        if (borderParent == null)
        {
            GameObject borderObject = GameObject.Find("Borders");
            if (borderObject != null)
                borderParent = borderObject.transform;
        }
    }

    void Update()
    {
        if (coldMeteorPrefab == null || mainCamera == null)
            return;

        if (Time.time < nextSpawnTime)
            return;

        int currentMaxMeteorCount = GetCurrentMaxMeteorCount();
        if (currentMaxMeteorCount > 0 && CountLiveMeteors() >= currentMaxMeteorCount)
        {
            nextSpawnTime = Time.time + GetCurrentSpawnInterval();
            return;
        }

        SpawnMeteor();
        nextSpawnTime = Time.time + GetCurrentSpawnInterval();
    }

    void SpawnMeteor()
    {
        Vector3 spawnPosition = GetRandomOffscreenPosition();
        Vector3 targetPosition = GetRandomOnscreenPosition();
        GameObject meteor = Instantiate(coldMeteorPrefab, spawnPosition, Quaternion.identity, meteorParent);
        IgnoreBorderCollisions(meteor);

        ColdMeteor coldMeteor = meteor.GetComponent<ColdMeteor>();
        if (coldMeteor != null)
        {
            float difficulty = GetDifficulty();
            coldMeteor.minSpeed = Mathf.Lerp(startMinSpeed, endMinSpeed, difficulty);
            coldMeteor.maxSpeed = Mathf.Lerp(startMaxSpeed, endMaxSpeed, difficulty);
            coldMeteor.Launch((targetPosition - spawnPosition).normalized);
        }
    }

    float GetDifficulty()
    {
        if (difficultyRampSeconds <= 0f)
            return 1f;

        return Mathf.Clamp01((Time.time - startTime) / difficultyRampSeconds);
    }

    float GetCurrentSpawnInterval()
    {
        return Mathf.Lerp(startSpawnInterval, minSpawnInterval, GetDifficulty());
    }

    int GetCurrentMaxMeteorCount()
    {
        return Mathf.RoundToInt(Mathf.Lerp(startMaxMeteorCount, maxMeteorCount, GetDifficulty()));
    }

    Vector3 GetRandomOffscreenPosition()
    {
        float height = mainCamera.orthographicSize;
        float width = height * mainCamera.aspect;
        Vector3 cameraPosition = mainCamera.transform.position;
        int side = Random.Range(0, 4);

        float x = Random.Range(-width, width);
        float y = Random.Range(-height, height);

        if (side == 0)
            y = height + spawnOutsidePadding;
        else if (side == 1)
            y = -height - spawnOutsidePadding;
        else if (side == 2)
            x = -width - spawnOutsidePadding;
        else
            x = width + spawnOutsidePadding;

        return new Vector3(cameraPosition.x + x, cameraPosition.y + y, 0f);
    }

    Vector3 GetRandomOnscreenPosition()
    {
        float height = mainCamera.orthographicSize * 0.85f;
        float width = height * mainCamera.aspect;
        Vector3 cameraPosition = mainCamera.transform.position;

        return new Vector3(
            cameraPosition.x + Random.Range(-width, width),
            cameraPosition.y + Random.Range(-height, height),
            0f);
    }

    void IgnoreBorderCollisions(GameObject meteor)
    {
        if (borderParent == null)
            return;

        Collider2D[] meteorColliders = meteor.GetComponentsInChildren<Collider2D>();
        Collider2D[] borderColliders = borderParent.GetComponentsInChildren<Collider2D>();

        foreach (Collider2D meteorCollider in meteorColliders)
        {
            foreach (Collider2D borderCollider in borderColliders)
                Physics2D.IgnoreCollision(meteorCollider, borderCollider);
        }
    }

    int CountLiveMeteors()
    {
        if (meteorParent == null)
            return FindObjectsByType<ColdMeteor>(FindObjectsInactive.Exclude).Length;

        int count = 0;
        for (int i = 0; i < meteorParent.childCount; i++)
        {
            if (meteorParent.GetChild(i).GetComponent<ColdMeteor>() != null)
                count++;
        }

        return count;
    }
}
