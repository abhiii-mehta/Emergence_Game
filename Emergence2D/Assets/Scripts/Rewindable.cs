using UnityEngine;
using System.Collections.Generic;

public class Rewindable : MonoBehaviour
{
    struct State
    {
        public Vector2 pos;
        public float rotZ;
    }

    private List<State> states = new List<State>();
    private Rigidbody2D rb;

    public GameObject ghostPrefab;
    private float ghostSpawnInterval = 0.05f;
    private float ghostTimer = 0f;

    private SpriteRenderer rend;
    private Color originalColor;
    private float rewindSpeed = 0.5f;
    private float rewindTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();
        if (rend != null)
            originalColor = rend.color;
    }

    void Update()
    {
        if (TimeManager.IsRewinding)
        {
            if (rb != null) rb.isKinematic = true;
            Rewind();

            if (rend != null)
                rend.color = Color.Lerp(rend.color, Color.cyan, 10f * Time.unscaledDeltaTime);
        }
        else
        {
            if (rb != null) rb.isKinematic = false;
            Record();

            if (TimeManager.IsFastForwarding)
            {
                if (rend != null)
                    rend.color = Color.Lerp(rend.color, Color.yellow, 10f * Time.unscaledDeltaTime);

                ghostTimer += Time.unscaledDeltaTime;
                if (ghostTimer >= ghostSpawnInterval && ghostPrefab != null)
                {
                    GameObject ghost = Instantiate(ghostPrefab, transform.position, transform.rotation);
                    var sr = ghost.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        Color color = TimeManager.IsRewinding ? Color.cyan : Color.yellow;
                        color.a = 0.4f;
                        sr.color = color;
                    }
                    Destroy(ghost, 0.5f);
                    ghostTimer = 0f;
                }

            }
            else
            {
                if (rend != null)
                    rend.color = Color.Lerp(rend.color, originalColor, 10f * Time.deltaTime);
            }
        }
    }

    void Record()
    {
        states.Add(new State
        {
            pos = transform.position,
            rotZ = transform.eulerAngles.z
        });

        if (states.Count > 600)
            states.RemoveAt(0);
    }

    void Rewind()
    {
        if (states.Count == 0) return;

        rewindTimer += Time.unscaledDeltaTime;
        if (rewindTimer >= rewindSpeed * Time.fixedDeltaTime && states.Count > 0)
        {
            State s = states[states.Count - 1];
            transform.position = s.pos;
            transform.rotation = Quaternion.Euler(0f, 0f, s.rotZ);
            states.RemoveAt(states.Count - 1);
            rewindTimer = 0f;
        }

        ghostTimer += Time.unscaledDeltaTime;
        if (ghostTimer >= ghostSpawnInterval && ghostPrefab != null)
        {
            GameObject ghost = Instantiate(ghostPrefab, transform.position, transform.rotation);
            var sr = ghost.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color color = TimeManager.IsRewinding ? Color.cyan : Color.yellow;
                color.a = 0.4f;
                sr.color = color;
            }
            Destroy(ghost, 0.5f);
            ghostTimer = 0f;
        }

    }
}
