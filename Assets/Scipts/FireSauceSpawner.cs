using UnityEngine;

public class FireSauceSpawner : MonoBehaviour
{
    public GameObject fireSaucePrefab;
    public Transform itemParent;
    public Transform borderParent;
    public float spawnInterval = 10f;

    private Camera mainCamera;
    private float nextSpawnTime;

    void Start()
    {
        mainCamera = Camera.main;

        if (borderParent == null)
        {
            GameObject borderObject = GameObject.Find("Borders");
            if (borderObject != null)
                borderParent = borderObject.transform;
        }

        nextSpawnTime = Time.time + spawnInterval;
    }

    void Update()
    {
        if (fireSaucePrefab == null || mainCamera == null || Time.time < nextSpawnTime)
            return;

        if (FindAnyObjectByType<FireSauce>() == null)
            SpawnSauce();

        nextSpawnTime = Time.time + spawnInterval;
    }

    void SpawnSauce()
    {
        float height = mainCamera.orthographicSize * 0.75f;
        float width = height * mainCamera.aspect;

        Vector3 cameraPosition = mainCamera.transform.position;
        Vector3 spawnPosition = new Vector3(
            cameraPosition.x + Random.Range(-width, width),
            cameraPosition.y + Random.Range(-height, height),
            0f
        );

        GameObject sauce = Instantiate(fireSaucePrefab, spawnPosition, Quaternion.identity, itemParent);

        IgnoreBorderCollisions(sauce);
    }

    void IgnoreBorderCollisions(GameObject sauce)
    {
        if (borderParent == null)
            return;

        Collider2D[] sauceColliders = sauce.GetComponentsInChildren<Collider2D>();
        Collider2D[] borderColliders = borderParent.GetComponentsInChildren<Collider2D>();

        foreach (Collider2D sauceCollider in sauceColliders)
        {
            foreach (Collider2D borderCollider in borderColliders)
                Physics2D.IgnoreCollision(sauceCollider, borderCollider);
        }
    }
}