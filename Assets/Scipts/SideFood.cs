using UnityEngine;

public enum SideFoodType
{
    BuldakCup,
    ChickenRadish
}

public class SideFood : MonoBehaviour
{
    public SideFoodType foodType;
    public float minSpeed = 10f;
    public float maxSpeed = 18f;
    public float cleanupPadding = 5f;

    private Rigidbody2D rb;
    private Camera mainCamera;
    private Vector2 launchDirection = Vector2.right;
    private float moveSpeed;

    void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        moveSpeed = Random.Range(minSpeed, maxSpeed);
        rb.angularVelocity = 0f;
        rb.linearVelocity = launchDirection.normalized * moveSpeed;

        IgnoreObstacleCollisions();
    }

    void FixedUpdate()
    {
        if (rb == null)
            return;

        rb.angularVelocity = 0f;
        rb.linearVelocity = launchDirection.normalized * moveSpeed;
    }

    void Update()
    {
        if (mainCamera == null)
            return;

        float height = mainCamera.orthographicSize + cleanupPadding;
        float width = height * mainCamera.aspect + cleanupPadding;
        Vector3 offset = transform.position - mainCamera.transform.position;

        if (Mathf.Abs(offset.x) > width || Mathf.Abs(offset.y) > height)
            Destroy(gameObject);
    }

    public void Launch(Vector2 direction)
    {
        launchDirection = direction.normalized;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponentInParent<Obstacles>() == null && !IsBorderCollision(collision))
            return;

        IgnoreCollisionWith(collision.gameObject);
    }

    bool IsBorderCollision(Collision2D collision)
    {
        Transform hitTransform = collision.transform;
        return hitTransform.name.StartsWith("Border") ||
               (hitTransform.parent != null && hitTransform.parent.name == "Borders");
    }

    void IgnoreObstacleCollisions()
    {
        Obstacles[] obstacles = FindObjectsByType<Obstacles>(FindObjectsInactive.Exclude);
        foreach (Obstacles obstacle in obstacles)
            IgnoreCollisionWith(obstacle.gameObject);
    }

    void IgnoreCollisionWith(GameObject other)
    {
        Collider2D[] ownColliders = GetComponentsInChildren<Collider2D>();
        Collider2D[] otherColliders = other.GetComponentsInChildren<Collider2D>();

        foreach (Collider2D ownCollider in ownColliders)
        {
            foreach (Collider2D otherCollider in otherColliders)
                Physics2D.IgnoreCollision(ownCollider, otherCollider);
        }
    }
}
