using UnityEngine;
using System.Collections;
public class NPCEmotionController : MonoBehaviour
{
    public enum EmotionType { Neutral, Happy, Sad, Angry, Love }
    public EmotionType currentEmotion = EmotionType.Neutral;

    private Rigidbody2D rb;
    private Animator animator;

    [Header("Animator Controllers")]
    public RuntimeAnimatorController happyController;
    public RuntimeAnimatorController sadController;
    public RuntimeAnimatorController angryController;
    public RuntimeAnimatorController loveController;
    public RuntimeAnimatorController neutralController;

    [Header("Movement")]
    public float moveSpeed = 1f;
    private Vector2 moveDirection;
    private float changeDirectionTime = 2f;
    private float directionTimer = 0f;

    private bool isFrozen = false;
    private float freezeTimer = 0f;

    [Header("Reproduction")]
    [SerializeField] private GameObject neutralNPCPrefab;
    private float birthCooldown = 5f;
    private float birthTimer = 0f;

    private float emotionCooldownTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        PickNewDirection();
        SetEmotion(currentEmotion);
    }

    void Update()
    {
        if (emotionCooldownTimer > 0f)
            emotionCooldownTimer -= Time.deltaTime;

        if (isFrozen)
        {
            freezeTimer -= Time.deltaTime;
            if (freezeTimer <= 0f)
            {
                isFrozen = false;
            }
            return;
        }

        directionTimer += Time.deltaTime;
        if (directionTimer >= changeDirectionTime)
        {
            PickNewDirection();
            directionTimer = 0f;
        }

        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        Vector3 pos = transform.position;
        float minX = -8.5f, maxX = 8.5f, minY = -4.5f, maxY = 4.5f;
        if (pos.x < minX || pos.x > maxX) moveDirection.x *= -1;
        if (pos.y < minY || pos.y > maxY) moveDirection.y *= -1;
        transform.position = new Vector3(Mathf.Clamp(pos.x, minX, maxX), Mathf.Clamp(pos.y, minY, maxY), pos.z);

        if (currentEmotion == EmotionType.Love && birthTimer > 0f)
            birthTimer -= Time.deltaTime;
    }

    void PickNewDirection()
    {
        moveDirection = Random.insideUnitCircle.normalized;
    }

    public void SetEmotion(EmotionType newEmotion)
    {
        if (currentEmotion == newEmotion) return;

        currentEmotion = newEmotion;
        emotionCooldownTimer = 0.3f;
        StartCoroutine(FlashGrey());

        switch (currentEmotion)
        {
            case EmotionType.Happy:
                animator.runtimeAnimatorController = happyController;
                break;
            case EmotionType.Sad:
                animator.runtimeAnimatorController = sadController;
                break;
            case EmotionType.Angry:
                animator.runtimeAnimatorController = angryController;
                break;
            case EmotionType.Love:
                animator.runtimeAnimatorController = loveController;
                break;
            case EmotionType.Neutral:
            default:
                animator.runtimeAnimatorController = neutralController;
                break;
        }
    }
    private IEnumerator FlashGrey()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.grey;
            yield return new WaitForSeconds(0.3f);
            sr.color = Color.white;
        }
        else
        {
            Debug.LogWarning($"{name} has no SpriteRenderer when flashing grey.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var otherNPC = other.GetComponent<NPCEmotionController>();
        if (otherNPC == null || otherNPC == this) return;

        EmotionType a = currentEmotion;
        EmotionType b = otherNPC.currentEmotion;

        if (emotionCooldownTimer > 0f || otherNPC.emotionCooldownTimer > 0f)
            return;

        if (a == EmotionType.Neutral && b != EmotionType.Neutral)
            SetEmotion(b);
        if (b == EmotionType.Neutral && a != EmotionType.Neutral)
            otherNPC.SetEmotion(a);

        if (a == EmotionType.Happy && b == EmotionType.Happy)
            return;

        if ((a == EmotionType.Happy && b == EmotionType.Sad) || (a == EmotionType.Sad && b == EmotionType.Happy))
        {
            SetEmotion(EmotionType.Neutral);
            otherNPC.SetEmotion(EmotionType.Neutral);
            return;
        }

        if (a == EmotionType.Happy && b == EmotionType.Angry)
        {
            SetEmotion(EmotionType.Sad);
            moveDirection = (transform.position - other.transform.position).normalized;
            return;
        }
        if (a == EmotionType.Angry && b == EmotionType.Happy)
        {
            otherNPC.SetEmotion(EmotionType.Sad);
            otherNPC.moveDirection = (other.transform.position - transform.position).normalized;
            return;
        }

        if (a == EmotionType.Happy && b == EmotionType.Love)
            SetEmotion(EmotionType.Love);
        if (a == EmotionType.Love && b == EmotionType.Happy)
            otherNPC.SetEmotion(EmotionType.Love);

        if (a == EmotionType.Sad && b == EmotionType.Sad)
        {
            Destroy(gameObject);
            Destroy(otherNPC.gameObject);
            return;
        }

        if (a == EmotionType.Sad && b == EmotionType.Angry)
            SetEmotion(EmotionType.Angry);
        if (a == EmotionType.Angry && b == EmotionType.Sad)
            otherNPC.SetEmotion(EmotionType.Angry);

        if ((a == EmotionType.Sad && b == EmotionType.Love) || (a == EmotionType.Love && b == EmotionType.Sad))
        {
            isFrozen = otherNPC.isFrozen = true;
            freezeTimer = otherNPC.freezeTimer = 2f;
            return;
        }

        if (a == EmotionType.Angry && b == EmotionType.Angry)
        {
            if (Random.value > 0.5f) Destroy(gameObject);
            else Destroy(otherNPC.gameObject);
            return;
        }

        if (a == EmotionType.Angry && b == EmotionType.Love)
            otherNPC.SetEmotion(EmotionType.Sad);
        if (a == EmotionType.Love && b == EmotionType.Angry)
            SetEmotion(EmotionType.Sad);

        if (a == EmotionType.Love && b == EmotionType.Love)
        {
            if (birthTimer <= 0f && otherNPC.birthTimer <= 0f && neutralNPCPrefab != null)
            {
                Vector2 offset = Random.insideUnitCircle.normalized * 0.5f;
                GameObject baby = Instantiate(neutralNPCPrefab, transform.position + (Vector3)offset, Quaternion.identity);
                var babyCtrl = baby.GetComponent<NPCEmotionController>();
                if (babyCtrl != null)
                {
                    StartCoroutine(AssignNeutralWithDelay(babyCtrl));
                }
                birthTimer = otherNPC.birthTimer = birthCooldown;
            }
        }
    }
    private IEnumerator AssignNeutralWithDelay(NPCEmotionController babyCtrl)
    {
        yield return new WaitForSeconds(0.2f);
        babyCtrl.SetEmotion(EmotionType.Neutral);
        Debug.Log($"{babyCtrl.name} initialized as Neutral.");
    }

}
