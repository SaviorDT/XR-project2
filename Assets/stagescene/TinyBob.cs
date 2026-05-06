using UnityEngine;

public class TinyBob : MonoBehaviour
{
    public float height = 0.008f;
    public float speed = 1.1f;

    private Vector3 startPos;
    private float offset;

    void Start()
    {
        startPos = transform.position;
        offset = Random.Range(0f, 10f);
    }

    void Update()
    {
        float y = Mathf.Sin((Time.time + offset) * speed) * height;
        transform.position = startPos + new Vector3(0f, y, 0f);
    }
}