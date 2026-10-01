using UnityEngine;

public class SideFoodSpawner : MonoBehaviour
{
    public GameObject buldakCupPrefab;
    public GameObject chickenRadishPrefab;
    public Transform foodParent;
    public Transform borderParent;
    public float spawnInterval = 4f;
    public float spawnOutsidePadding = 1.5f;

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
        if (mainCamera == null || Time.time < nextSpawnTime)
            return;

        SpawnFood();
        nextSpawnTime = Time.time + spawnInterval;
    }

    void SpawnFood()
    {
        GameObject prefab = Random.value < 0.5f ? buldakCupPrefab : chickenRadishPrefab;
        if (prefab == null)
            return;

        float height = mainCamera.orthographicSize * 0.8f;
        float width = mainCamera.orthographicSize * mainCamera.aspect;
        bool fromLeft = Random.value < 0.5f;

        Vector3 cameraPosition = mainCamera.transform.position;
        Vector3 spawnPosition = new Vector3(
            cameraPosition.x + (fromLeft ? -width - spawnOutsidePadding : width + spawnOutsidePadding),
            cameraPosition.y + Random.Range(-height, height),
            0f
        );

        GameObject food = Instantiate(prefab, spawnPosition, Quaternion.identity, foodParent);

        IgnoreBorderCollisions(food);
        IgnoreObstacleCollisions(food);
        IgnoreOtherFoodCollisions(food);

        SideFood sideFood = food.GetComponent<SideFood>();
        if (sideFood != null)
            sideFood.Launch(fromLeft ? Vector2.right : Vector2.left);
    }

    void IgnoreOtherFoodCollisions(GameObject newFood)
    {
        SideFood[] foods = FindObjectsByType<SideFood>(FindObjectsInactive.Exclude);

        Collider2D[] newFoodColliders = newFood.GetComponentsInChildren<Collider2D>();

        foreach (SideFood otherFood in foods)
        {
            if (otherFood.gameObject == newFood)
                continue;

            Collider2D[] otherColliders = otherFood.GetComponentsInChildren<Collider2D>();

            foreach (Collider2D newCollider in newFoodColliders)
            {
                foreach (Collider2D otherCollider in otherColliders)
                {
                    Physics2D.IgnoreCollision(newCollider, otherCollider);
                }
            }
        }
    }

    void IgnoreBorderCollisions(GameObject food)
    {
        if (borderParent == null)
            return;

        Collider2D[] foodColliders = food.GetComponentsInChildren<Collider2D>();
        Collider2D[] borderColliders = borderParent.GetComponentsInChildren<Collider2D>();

        foreach (Collider2D foodCollider in foodColliders)
        {
            foreach (Collider2D borderCollider in borderColliders)
                Physics2D.IgnoreCollision(foodCollider, borderCollider);
        }
    }

    void IgnoreObstacleCollisions(GameObject food)
    {
        Collider2D[] foodColliders = food.GetComponentsInChildren<Collider2D>();
        Obstacles[] obstacles = FindObjectsByType<Obstacles>(FindObjectsInactive.Exclude);

        foreach (Collider2D foodCollider in foodColliders)
        {
            foreach (Obstacles obstacle in obstacles)
            {
                Collider2D[] obstacleColliders = obstacle.GetComponentsInChildren<Collider2D>();

                foreach (Collider2D obstacleCollider in obstacleColliders)
                    Physics2D.IgnoreCollision(foodCollider, obstacleCollider);
            }
        }
    }
}