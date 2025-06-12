using UnityEngine;
using System.Collections.Generic;

public class Rewindable : MonoBehaviour
{
    struct State
    {
        public Vector2 pos;
        public float rotZ;
        public Vector2 velocity;
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

    private bool wasRewinding = false;
    private bool hasAppliedFastForward = false;
    public float slowFactor = 0.3f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();
        if (rend != null)
            originalColor = rend.color;
    }

    void Update()
    {
        if (transform.position.y < -20f)
        {
            Debug.LogWarning($"{name} fell through world! Resetting.");
            transform.position = Vector2.zero;
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }

        if (TimeManager.IsRewinding)
        {
            if (rb != null) rb.isKinematic = true;
            Rewind();
            SetColor(Color.cyan);
            wasRewinding = true;
            return;
        }

        if (wasRewinding)
        {
            if (rb != null)
            {
                rb.isKinematic = false;

                if (states.Count > 0)
                {
                    rb.linearVelocity = states[states.Count - 1].velocity;
                    Debug.Log($"{name} resumed with velocity: {rb.linearVelocity}");
                }
            }
            wasRewinding = false;
        }

        Record();

        if (TimeManager.IsFastForwarding)
        {
            if (!hasAppliedFastForward && rb != null)
            {
                rb.linearVelocity *= 2f;
                hasAppliedFastForward = true;
                Debug.Log($"{name} fast forward started: velocity x2");
            }

            SetColor(Color.yellow);
        }
        else if (TimeManager.TimeScale < 1f)
        {
            if (rb != null)
            {
                Vector2 adjustedGravity = Physics2D.gravity * (slowFactor) * rb.gravityScale * Time.deltaTime;
                rb.linearVelocity += adjustedGravity;

                rb.linearVelocity *= Mathf.Lerp(1f, slowFactor, Time.deltaTime * 10f);
            }

            SetColor(Color.Lerp(originalColor, Color.gray, 0.5f));
        }

        else
        {
            hasAppliedFastForward = false;
            SetColor(originalColor);
        }


        HandleGhosts();
    }

    void SetColor(Color targetColor)
    {
        if (rend != null)
            rend.color = Color.Lerp(rend.color, targetColor, 10f * Time.unscaledDeltaTime);
    }

    void Record()
    {
        states.Add(new State
        {
            pos = transform.position,
            rotZ = transform.eulerAngles.z,
            velocity = rb != null ? rb.linearVelocity : Vector2.zero
        });

        if (states.Count > 600)
            states.RemoveAt(0);
    }

    void Rewind()
    {
        rewindTimer += Time.unscaledDeltaTime;
        if (rewindTimer >= rewindSpeed * Time.fixedDeltaTime && states.Count > 0)
        {
            State s = states[states.Count - 1];
            transform.position = s.pos;
            transform.rotation = Quaternion.Euler(0f, 0f, s.rotZ);
            if (rb != null) rb.linearVelocity = s.velocity;
            states.RemoveAt(states.Count - 1);
            rewindTimer = 0f;
        }
    }

    void HandleGhosts()
    {
        if (!TimeManager.IsRewinding && !TimeManager.IsFastForwarding)
            return;

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
