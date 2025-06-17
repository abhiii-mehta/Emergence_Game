using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class AchievementPopup : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public float duration = 2.5f;
    public float slideDistance = 30f;

    RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void Setup(string title, string description)
    {
        titleText.text = title;
        descriptionText.text = description;
        StartCoroutine(AnimatePopup());
    }

    IEnumerator AnimatePopup()
    {
        Vector2 startPos = rect.anchoredPosition;
        Vector2 endPos = startPos + Vector2.down * slideDistance;

        float t = 0f;
        while (t < 0.3f)
        {
            t += Time.deltaTime;
            rect.anchoredPosition = Vector2.Lerp(startPos, endPos, t / 0.3f);
            yield return null;
        }

        yield return new WaitForSeconds(duration);

        t = 0f;
        while (t < 0.3f)
        {
            t += Time.deltaTime;
            rect.anchoredPosition = Vector2.Lerp(endPos, startPos, t / 0.3f);
            yield return null;
        }

        Destroy(gameObject);
    }
}
