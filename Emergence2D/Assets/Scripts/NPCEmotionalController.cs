using UnityEngine;
using System.Collections;
public class NPCEmotionController : MonoBehaviour
{
    public enum EmotionType { Neutral, Happy, Sad, Angry, Love }
    public EmotionType currentEmotion = EmotionType.Neutral;

    private Rigidbody2D rb;
    private Animator animator;
    [SerializeField] GameObject storkPrefab;
    [SerializeField] public GameObject deathEffectPrefab;
    [SerializeField] private GameObject happyMeetEffect;


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
    private static int totalCryEvents = 0;
    private static int totalBabiesSpawned = 0;


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
        float minX = -6.5f, maxX = 8.5f, minY = -4.5f, maxY = 4.5f;
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
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogWarning($"{name} is missing an Animator when trying to SetEmotion to {newEmotion}!");
                return;
            }
        }

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
        if (newEmotion == EmotionType.Sad)
        {
            totalCryEvents++;
            if (totalCryEvents >= 5)
                AchievementManager.Instance.Unlock("depression", "Depression", "5 crying sessions.");
        }

        CheckForAchievementTriggers();
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
        {
            Debug.Log("Playing happy_vs_happy SFX");
            PlayHappyEffect();
            AudioManager.Instance.PlaySFXTemporary("happy_vs_happy",2f);
            return;
        }

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
            SpawnDeathEffect();
            AudioManager.Instance.PlaySFXTemporary("sad_vs_sad", 2f);
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
            AudioManager.Instance.PlaySFXTemporary("sad_love_cry", 2f);
            isFrozen = otherNPC.isFrozen = true;
            freezeTimer = otherNPC.freezeTimer = 2f;
            return;
        }

        if (a == EmotionType.Angry && b == EmotionType.Angry)
        {
            if (Random.value > 0.5f)
            {
                AudioManager.Instance.PlaySFX("angry_vs_angry");
                SpawnDeathEffect();
                Destroy(gameObject);
            }
            else
            {
                AudioManager.Instance.PlaySFX("angry_vs_angry");
                otherNPC.SpawnDeathEffect();
                Destroy(otherNPC.gameObject);
            }
            return;
        }

        if (a == EmotionType.Angry && b == EmotionType.Love)
            otherNPC.SetEmotion(EmotionType.Sad);
        if (a == EmotionType.Love && b == EmotionType.Angry)
            SetEmotion(EmotionType.Sad);

        if (a == EmotionType.Love && b == EmotionType.Love)
        {
            AudioManager.Instance.PlaySFX("love_vs_love");
            if (birthTimer <= 0f && otherNPC.birthTimer <= 0f && neutralNPCPrefab != null)
            {
                Vector2 offset = Random.insideUnitCircle.normalized * 0.5f;
                Vector3 babyPosition = transform.position + (Vector3)offset;

                SpawnBaby(babyPosition);

                birthTimer = otherNPC.birthTimer = birthCooldown;
            }
        }
        CheckForAchievementTriggers();

    }
    void SpawnBaby(Vector3 babyPosition)
    {
        if (storkPrefab != null)
        {
            Vector3 storkStartPos = new Vector3(
                Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x - 2f,
                babyPosition.y,
                0
            );

            GameObject stork = Instantiate(storkPrefab, storkStartPos, Quaternion.identity);
            StorkController storkCtrl = stork.GetComponent<StorkController>();

            if (storkCtrl != null)
            {
                storkCtrl.babyPrefab = neutralNPCPrefab;
                storkCtrl.babyDropPosition = babyPosition;

                totalBabiesSpawned++;
                if (totalBabiesSpawned == 5)
                    AchievementManager.Instance.Unlock("rabbits", "The Rabbits", "Spawn at least 5 NPCs via childbirth");
            }
        }
        else
        {
            GameObject baby = Instantiate(neutralNPCPrefab, babyPosition, Quaternion.identity);
            var babyCtrl = baby.GetComponent<NPCEmotionController>();
            if (babyCtrl != null)
            {
                babyCtrl.SetEmotion(NPCEmotionController.EmotionType.Neutral);
            }
            else
            {
                Debug.LogWarning("Spawned baby does not have NPCEmotionController!");
            }

            totalBabiesSpawned++;
            if (totalBabiesSpawned == 5)
                AchievementManager.Instance.Unlock("rabbits", "The Rabbits", "Spawn at least 5 NPCs via childbirth");
        }

        CheckForAchievementTriggers();
    }

    private IEnumerator AssignNeutralWithDelay(NPCEmotionController babyCtrl)
    {
        yield return new WaitForSeconds(0.2f);
        babyCtrl.SetEmotion(EmotionType.Neutral);
        Debug.Log($"{babyCtrl.name} initialized as Neutral.");
    }
    void CheckForAchievementTriggers()
    {
        var all = FindObjectsOfType<NPCEmotionController>();

        if (all.Length >= 20)
            AchievementManager.Instance.Unlock("overcrowding", "Overcrowding", "Can of sardines");

        if (all.Length == 1)
        {
            var npc = all[0];
            if (npc.currentEmotion == EmotionType.Happy)
                AchievementManager.Instance.Unlock("power_of_friendship", "The Power of Friendship", "One happy left");
        }

        int loveCount = 0;
        foreach (var npc in all)
        {
            if (npc.currentEmotion == EmotionType.Love) loveCount++;
        }

        if (loveCount >= 10)
            AchievementManager.Instance.Unlock("orgy", "10 Love NPCs", "Love is in the air... a bit too much.");
    }
    public void SpawnDeathEffect()
    {
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }
    }
    void PlayHappyEffect()
    {
        if (happyMeetEffect != null)
        {
            Instantiate(happyMeetEffect, transform.position, Quaternion.identity);
        }
    }


}
