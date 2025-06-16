using UnityEngine;

public class AbilityEffect : MonoBehaviour
{
    public AbilityManager.AbilityType abilityType;
    public float radius = 1f;
    public float duration = 1.5f;

    void Start()
    {
        Destroy(gameObject, duration);
        ApplyEffect();
    }

    void ApplyEffect()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D hit in hits)
        {
            var npc = hit.GetComponent<NPCEmotionController>();
            if (npc == null) continue;

            switch (abilityType)
            {
                case AbilityManager.AbilityType.Nuke:
                    Destroy(npc.gameObject);
                    break;
                case AbilityManager.AbilityType.Money:
                    npc.SetEmotion(NPCEmotionController.EmotionType.Happy);
                    break;
                case AbilityManager.AbilityType.Rain:
                    npc.SetEmotion(NPCEmotionController.EmotionType.Sad);
                    break;
                case AbilityManager.AbilityType.Stink:
                    npc.SetEmotion(NPCEmotionController.EmotionType.Angry);
                    break;
                case AbilityManager.AbilityType.Condom:
                    npc.SetEmotion(NPCEmotionController.EmotionType.Love);
                    break;
            }
        }
    }
}
