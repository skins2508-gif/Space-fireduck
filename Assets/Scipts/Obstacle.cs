using UnityEngine;

public class Obstacles : MonoBehaviour
{
    public float minSize = 0.25f;
    public float maxSize = 0.65f;
    public float minSpeed = 5f;
    public float maxSpeed = 7f;
    public float cleanupPadding = 5f;
    public bool moveOnStart = true;
    Rigidbody2D rb;
    private bool hasLaunchDirection;
    private Vector2 launchDirection;
    private Camera mainCamera;
    private float moveSpeed;

    void Start()
    {
        mainCamera = Camera.main;

        float randomSize = Random.Range(minSize, maxSize);
        transform.localScale = new Vector3(randomSize, randomSize, 1);

        Vector2 randomDirection = hasLaunchDirection ? launchDirection : Random.insideUnitCircle.normalized;

        rb = GetComponent<Rigidbody2D>();
        moveSpeed = Random.Range(minSpeed, maxSpeed);
        launchDirection = randomDirection.normalized;
        rb.angularVelocity = 0f;
        rb.linearVelocity = moveOnStart ? launchDirection * moveSpeed : Vector2.zero;
    }

    void FixedUpdate()
    {
        if (rb == null)
            return;

        rb.angularVelocity = 0f;
        rb.linearVelocity = moveOnStart ? launchDirection * moveSpeed : Vector2.zero;
    }

    void Update()
    {
        if (mainCamera == null)
            return;

        float height = mainCamera.orthographicSize + cleanupPadding;
        float width = height * mainCamera.aspect + cleanupPadding;
        Vector3 cameraPosition = mainCamera.transform.position;
        Vector3 offset = transform.position - cameraPosition;

        if (Mathf.Abs(offset.x) > width || Mathf.Abs(offset.y) > height)
            Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsBorderCollision(collision))
            return;

        BounceRandomlyFromBorder(collision);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!IsBorderCollision(collision))
            return;

        BounceRandomlyFromBorder(collision);
    }

    public void LaunchAt(Vector2 direction)
    {
        moveOnStart = true;
        hasLaunchDirection = true;
        launchDirection = direction.normalized;
    }

    public void LaunchRandom()
    {
        moveOnStart = true;
        hasLaunchDirection = true;
        launchDirection = Random.insideUnitCircle.normalized;
    }

    bool IsBorderCollision(Collision2D collision)
    {
        Transform hitTransform = collision.transform;
        return hitTransform.name.StartsWith("Border") ||
               (hitTransform.parent != null && hitTransform.parent.name == "Borders");
    }

    void BounceRandomlyFromBorder(Collision2D collision)
    {
        Vector2 normal = collision.GetContact(0).normal;
        Vector2 reflected = Vector2.Reflect(launchDirection, normal).normalized;
        Vector2 randomSpread = Random.insideUnitCircle * 0.45f;
        Vector2 newDirection = (reflected + randomSpread).normalized;

        if (newDirection.sqrMagnitude < 0.01f)
            newDirection = -launchDirection;

        launchDirection = newDirection;
        moveSpeed = Random.Range(minSpeed, maxSpeed);

        if (rb != null)
            rb.linearVelocity = launchDirection * moveSpeed;
    }
}
