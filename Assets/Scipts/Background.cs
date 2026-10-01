using UnityEngine;

public class Background : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float resetX = 0f;
    public float endX = 20f;

    void Update()
    {
        // 오른쪽으로 이동
        transform.position += Vector3.right * moveSpeed * Time.deltaTime;

        // 일정 위치에 도달하면 원점으로
        if (transform.position.x >= endX)
        {
            transform.position = new Vector3(
                resetX,
                transform.position.y,
                transform.position.z);
        }
    }
}