using UnityEngine;

public class ColdMeteor : MonoBehaviour
{
    public float minSize = 0.35f;
    public float maxSize = 0.75f;
    public float minSpeed = 5f;
    public float maxSpeed = 7f;
    public float cleanupPadding = 5f;

    private Rigidbody2D rb;
    private Camera mainCamera;
    private Vector2 launchDirection = Vector2.down;
    private float moveSpeed;

    void Start()
    {
        mainCamera = Camera.main;

        float randomSize = Random.Range(minSize, maxSize);
        transform.localScale = new Vector3(randomSize, randomSize, 1f);

        rb = GetComponent<Rigidbody2D>();
        moveSpeed = Random.Range(minSpeed, maxSpeed);
        rb.angularVelocity = 0f;
        rb.linearVelocity = launchDirection.normalized * moveSpeed;
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
}
