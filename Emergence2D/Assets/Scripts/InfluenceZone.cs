using UnityEngine;

public class InfluenceZone : MonoBehaviour
{
    public NPCEmotionController.EmotionType influenceEmotion = NPCEmotionController.EmotionType.Happy;
    [Range(1, 3)] public int influenceLevel = 2;
    public float influenceRadius = 2f;
    public float influenceRate = 1f;

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= influenceRate)
        {
            ApplyInfluence();
            timer = 0f;
        }
    }

    void ApplyInfluence()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, influenceRadius);
        foreach (var hit in hits)
        {
            NPCEmotionController npc = hit.GetComponent<NPCEmotionController>();
            if (npc != null)
            {
                if (npc.currentEmotion == influenceEmotion)
                {
                    if (npc.emotionLevel < influenceLevel)
                    {
                        npc.SetEmotion(influenceEmotion, npc.emotionLevel + 1);
                    }
                }
                else
                {
                    npc.SetEmotion(influenceEmotion, 1);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, influenceRadius);
    }
}
