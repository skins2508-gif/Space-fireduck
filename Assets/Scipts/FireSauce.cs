using UnityEngine;

public class FireSauce : MonoBehaviour
{
    public float cleanupSeconds = 12f;
    public float spinSpeed = 60f;

    void Start()
    {
        Destroy(gameObject, cleanupSeconds);
    }

    void Update()
    {
        transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
    }
}
