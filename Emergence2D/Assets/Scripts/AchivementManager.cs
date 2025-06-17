using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;

    [SerializeField] private GameObject popupPrefab;
    [SerializeField] private Transform popupParent;

    private HashSet<string> unlocked = new HashSet<string>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    public void Unlock(string id, string title, string description)
    {
        if (unlocked.Contains(id)) return;

        unlocked.Add(id);

        if (popupPrefab != null && popupParent != null)
        {
            GameObject popup = Instantiate(popupPrefab, popupParent);
            var popupScript = popup.GetComponent<AchievementPopup>();
            if (popupScript != null)
            {
                popupScript.Setup(title, description);
            }
        }
        else
        {
            Debug.LogWarning("Popup prefab or parent not set in AchievementManager.");
        }
    }

}
