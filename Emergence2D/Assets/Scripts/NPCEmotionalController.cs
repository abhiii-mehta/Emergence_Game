using UnityEngine;

public class NPCEmotionController : MonoBehaviour
{
    public enum EmotionType { Happy, Sad, Neutral }

    public EmotionType currentEmotion = EmotionType.Happy;
    [Range(1, 3)] public int emotionLevel = 1;

    private SpriteRenderer rend;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 1f;
    private Vector2 moveDirection;
    private float directionChangeInterval = 2f;
    private float directionTimer = 0f;

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        UpdateColor();
        PickNewDirection();
    }

    void Update()
    {
        directionTimer += Time.deltaTime;
        if (directionTimer >= directionChangeInterval)
        {
            PickNewDirection();
            directionTimer = 0f;
        }

        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            RandomizeEmotion();
        }
    }

    void PickNewDirection()
    {
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);
        moveDirection = new Vector2(x, y).normalized;
    }

    void UpdateColor()
    {
        Color baseColor;


        switch (currentEmotion)
        {
            case EmotionType.Happy:
                baseColor = Color.yellow;
                break;
            case EmotionType.Sad:
                baseColor = new Color(0.3f, 0.6f, 1f);
                break;
            case EmotionType.Neutral:
                baseColor = Color.gray;
                break;
            default:
                baseColor = Color.white;
                break;
        }


        float brightness = 0.5f + (emotionLevel * 0.2f);
        baseColor *= brightness;
        baseColor.a = 1f;
        rend.color = baseColor;
    }

    public void SetEmotion(EmotionType newType, int newLevel)
    {
        currentEmotion = newType;
        emotionLevel = currentEmotion == EmotionType.Neutral ? 0 : Mathf.Clamp(newLevel, 1, 3);

        UpdateColor();
    }

    void RandomizeEmotion()
    {
        currentEmotion = (Random.value > 0.5f) ? EmotionType.Happy : EmotionType.Sad;
        emotionLevel = Random.Range(1, 4);
        UpdateColor();
    }

    public override string ToString()
    {
        return $"{name} - {currentEmotion} {emotionLevel}";
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        NPCEmotionController otherNPC = other.GetComponent<NPCEmotionController>();
        if (otherNPC == null || otherNPC == this) return;

        if (currentEmotion == otherNPC.currentEmotion)
        {
            if (emotionLevel < otherNPC.emotionLevel)
            {
                SetEmotion(currentEmotion, otherNPC.emotionLevel);
            }
            else if (emotionLevel > otherNPC.emotionLevel)
            {
                otherNPC.SetEmotion(currentEmotion, emotionLevel);
            }
        }
        else
        {
            int levelDiff = Mathf.Abs(emotionLevel - otherNPC.emotionLevel);

            if (emotionLevel > otherNPC.emotionLevel)
            {
                otherNPC.SetEmotion(currentEmotion, Mathf.Min(3, emotionLevel - levelDiff));
            }
            else if (emotionLevel < otherNPC.emotionLevel)
            {
                SetEmotion(otherNPC.currentEmotion, Mathf.Min(3, otherNPC.emotionLevel - levelDiff));
            }
            else
            {
                SetEmotion(EmotionType.Neutral, 0);
                otherNPC.SetEmotion(EmotionType.Neutral, 0);
                Debug.Log($"{name} and {other.name} neutralized each other.");

            }
        }
    }

}
