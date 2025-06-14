using UnityEngine;

public class NPCEmotionController : MonoBehaviour
{
    public enum EmotionType { Neutral, Happy, Sad, Angry, Love }

    public EmotionType currentEmotion = EmotionType.Neutral;

    private SpriteRenderer rend;
    private Rigidbody2D rb;

    [Header("Movement")]
    public float moveSpeed = 1f;
    private Vector2 moveDirection;
    private float changeDirectionTime = 2f;
    private float directionTimer = 0f;

    private bool isFrozen = false;
    private float freezeTimer = 0f;

    [SerializeField] private GameObject neutralNPCPrefab;
    private float birthCooldown = 5f;
    private float birthTimer = 0f;

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        SetColor();
        PickNewDirection();
    }

    void Update()
    {
        if (isFrozen)
        {
            freezeTimer -= Time.deltaTime;
            if (freezeTimer <= 0f) isFrozen = false;
            return;
        }

        directionTimer += Time.deltaTime;
        if (directionTimer >= changeDirectionTime)
        {
            PickNewDirection();
            directionTimer = 0f;
        }

        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        if (currentEmotion == EmotionType.Love && birthTimer > 0f)
            birthTimer -= Time.deltaTime;
    }

    void PickNewDirection()
    {
        moveDirection = Random.insideUnitCircle.normalized;
    }

    void SetColor()
    {
        Color c = currentEmotion switch
        {
            EmotionType.Happy => Color.yellow,
            EmotionType.Sad => new Color(0.3f, 0.6f, 1f),
            EmotionType.Angry => Color.red,
            EmotionType.Love => Color.magenta,
            EmotionType.Neutral => Color.gray,
            _ => Color.white
        };
        if (rend != null)
            rend.color = c;
    }

    public void SetEmotion(EmotionType newEmotion)
    {
        currentEmotion = newEmotion;
        SetColor();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var otherNPC = other.GetComponent<NPCEmotionController>();
        if (otherNPC == null || otherNPC == this) return;

        EmotionType a = currentEmotion;
        EmotionType b = otherNPC.currentEmotion;

        if (a == EmotionType.Neutral && b != EmotionType.Neutral)
            SetEmotion(b);
        if (b == EmotionType.Neutral && a != EmotionType.Neutral)
            otherNPC.SetEmotion(a);

        if (a == EmotionType.Happy && b == EmotionType.Happy) return;

        if (a == EmotionType.Sad && b == EmotionType.Sad)
        {
            Destroy(gameObject);
            Destroy(otherNPC.gameObject);
            return;
        }

        if (a == EmotionType.Angry && b == EmotionType.Angry)
        {
            if (Random.value > 0.5f) Destroy(gameObject);
            else Destroy(otherNPC.gameObject);
            return;
        }

        if (a == EmotionType.Love && b == EmotionType.Love)
        {
            if (birthTimer <= 0f && otherNPC.birthTimer <= 0f && neutralNPCPrefab != null)
            {
                Vector2 offset = Random.insideUnitCircle.normalized * 0.5f;
                Instantiate(neutralNPCPrefab, transform.position + (Vector3)offset, Quaternion.identity);
                birthTimer = 5f;
                otherNPC.birthTimer = 5f;
            }
            return;
        }

        if ((a == EmotionType.Happy && b == EmotionType.Sad) || (a == EmotionType.Sad && b == EmotionType.Happy))
        {
            SetEmotion(EmotionType.Neutral);
            otherNPC.SetEmotion(EmotionType.Neutral);
            return;
        }

        if ((a == EmotionType.Happy && b == EmotionType.Angry))
        {
            SetEmotion(EmotionType.Sad);
            moveDirection = (transform.position - other.transform.position).normalized;
            return;
        }

        if ((b == EmotionType.Happy && a == EmotionType.Angry))
        {
            otherNPC.SetEmotion(EmotionType.Sad);
            otherNPC.moveDirection = (other.transform.position - transform.position).normalized;
            return;
        }

        if (a == EmotionType.Sad && b == EmotionType.Angry)
        {
            SetEmotion(EmotionType.Angry);
            return;
        }

        if (b == EmotionType.Sad && a == EmotionType.Angry)
        {
            otherNPC.SetEmotion(EmotionType.Angry);
            return;
        }

        if (a == EmotionType.Happy && b == EmotionType.Love)
        {
            SetEmotion(EmotionType.Love);
            return;
        }

        if (b == EmotionType.Happy && a == EmotionType.Love)
        {
            otherNPC.SetEmotion(EmotionType.Love);
            return;
        }

        if (a == EmotionType.Angry && b == EmotionType.Love)
        {
            otherNPC.SetEmotion(EmotionType.Sad);
            return;
        }

        if (b == EmotionType.Angry && a == EmotionType.Love)
        {
            SetEmotion(EmotionType.Sad);
            return;
        }

        if ((a == EmotionType.Sad && b == EmotionType.Love) || (a == EmotionType.Love && b == EmotionType.Sad))
        {
            isFrozen = true;
            otherNPC.isFrozen = true;
            freezeTimer = 2f;
            otherNPC.freezeTimer = 2f;
        }
    }
}
