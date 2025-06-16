using UnityEngine;

public class StorkFlyby : MonoBehaviour
{
    public float speed = 5f;
    public float destroyDelay = 10f;

    void Start()
    {
        Destroy(gameObject, destroyDelay);
    }

    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;
    }
}

