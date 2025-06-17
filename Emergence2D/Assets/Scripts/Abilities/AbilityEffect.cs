using UnityEngine;
using static AbilityManager;

public class AbilityEffect : MonoBehaviour
{
    public AbilityManager.AbilityType abilityType;
    public float radius = 1f;
    public float duration = 1.5f;
    private AudioSource sfxSource;
    void Start()
    {
        Destroy(gameObject, duration);
        ApplyEffect();
        switch (abilityType)
        {
            case AbilityManager.AbilityType.Nuke:
                AudioManager.Instance.PlaySFX("nuke");
                break;
            case AbilityManager.AbilityType.Money:
                AudioManager.Instance.PlayLoopingSFX("money");
                break;
            case AbilityManager.AbilityType.Rain:
                AudioManager.Instance.PlayLoopingSFX("rain");
                break;
            case AbilityManager.AbilityType.Stink:
                AudioManager.Instance.PlaySFX("angry");
                break;

            case AbilityManager.AbilityType.Condom:
                AudioManager.Instance.PlaySFX("condom");
                break;
        }
    }
    void OnDestroy()
    {
        if (abilityType == AbilityManager.AbilityType.Money || abilityType == AbilityManager.AbilityType.Rain)
        {
            AudioManager.Instance.StopLoopingSFX();
        }
    }
    void StopEffectSound()
    {
        AudioManager.Instance.StopLoopingSFX();
    }
    void ApplyEffect()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        bool triggeredAchievement = false;
        int affectedNPCs = 0;

        foreach (Collider2D hit in hits)
        {
            var npc = hit.GetComponent<NPCEmotionController>();
            if (npc == null) continue;

            affectedNPCs++;

            switch (abilityType)
            {
                case AbilityManager.AbilityType.Nuke:
                    if (npc != null)
                    {
                        if (npc.CompareTag("Stork")) break;
                        var npcCtrl = npc.GetComponent<NPCEmotionController>();
                        if (npcCtrl != null)
                            npcCtrl.SpawnDeathEffect();

                        Destroy(npc.gameObject);
                    }

                    triggeredAchievement = true;
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

        if (abilityType == AbilityManager.AbilityType.Nuke && triggeredAchievement)
        {
            AchievementManager.Instance.Unlock("nuke", "Hiroshima", "Fire a nuke.");
        }

        if (abilityType == AbilityManager.AbilityType.Rain && affectedNPCs >= 5)
        {
            AchievementManager.Instance.Unlock("depression", "Depression", "Have 5 crying sessions.");
        }
    }

}
