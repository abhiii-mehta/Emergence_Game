using UnityEngine;

public class StorkController : MonoBehaviour
{
    public GameObject babyPrefab;
    public float speed = 5f;
    private bool hasDropped = false;

    public Vector3 babyDropPosition;

    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;

        if (!hasDropped && transform.position.x >= babyDropPosition.x)
        {
            hasDropped = true;

            GameObject baby = Instantiate(babyPrefab, babyDropPosition, Quaternion.identity);
            var babyCtrl = baby.GetComponent<NPCEmotionController>();
            if (babyCtrl != null)
            {
                babyCtrl.SetEmotion(NPCEmotionController.EmotionType.Neutral);
            }
        }

        float screenRightEdge = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x + 2f;
        if (transform.position.x > screenRightEdge)
        {
            Destroy(gameObject);
        }
    }
}
