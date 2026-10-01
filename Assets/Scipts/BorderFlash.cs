using System.Collections;
using UnityEngine;

public class BorderFlash : MonoBehaviour
{
    private SpriteRenderer sr;

    public Color color1 = Color.blue;
    public Color color2 = Color.red;
    public float interval = 0.5f;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        StartCoroutine(ColorLoop());
    }

    IEnumerator ColorLoop()
    {
        while (true)
        {
            sr.color = color1;
            yield return new WaitForSeconds(interval);

            sr.color = color2;
            yield return new WaitForSeconds(interval);
        }
    }
}