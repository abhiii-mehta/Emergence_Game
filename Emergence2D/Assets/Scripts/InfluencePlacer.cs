using UnityEngine;

public class InfluencePlacer : MonoBehaviour
{
    public GameObject influenceHappyPrefab;
    public GameObject influenceSadPrefab;
    public Camera mainCamera;

    private NPCEmotionController.EmotionType selectedEmotion = NPCEmotionController.EmotionType.Happy;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) selectedEmotion = NPCEmotionController.EmotionType.Happy;
        if (Input.GetKeyDown(KeyCode.Alpha2)) selectedEmotion = NPCEmotionController.EmotionType.Sad;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            PlaceToken(worldPos);
        }
    }

    void PlaceToken(Vector2 position)
    {
        GameObject prefab = selectedEmotion == NPCEmotionController.EmotionType.Happy
            ? influenceHappyPrefab
            : influenceSadPrefab;

        Instantiate(prefab, position, Quaternion.identity);
        Debug.Log($"Placed {selectedEmotion} token at {position}");
    }
}
