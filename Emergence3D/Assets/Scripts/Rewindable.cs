using UnityEngine;
using System.Collections.Generic;

public class Rewindable : MonoBehaviour
{
    struct State
    {
        public Vector3 pos;
        public Quaternion rot;
    }

    private List<State> states = new List<State>();
    private Rigidbody rb;

    public GameObject ghostPrefab;
    private float ghostSpawnInterval = 0.05f;
    private float ghostTimer = 0f;

    private Renderer rend;
    private Color originalColor;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
        if (rend != null)
            originalColor = rend.material.color;
    }

    void Update()
    {
        if (TimeManager.IsRewinding)
        {
            if (rb != null) rb.isKinematic = true;
            Rewind();

            if (rend != null)
                rend.material.color = Color.Lerp(rend.material.color, Color.cyan, 10f * Time.unscaledDeltaTime);
        }
        else
        {
            if (rb != null) rb.isKinematic = false;
            Record();

            if (TimeManager.IsFastForwarding)
            {
                if (rend != null)
                    rend.material.color = Color.Lerp(rend.material.color, Color.yellow, 10f * Time.unscaledDeltaTime);

                ghostTimer += Time.unscaledDeltaTime;
                if (ghostTimer >= ghostSpawnInterval && ghostPrefab != null)
                {
                    GameObject ghost = Instantiate(ghostPrefab, transform.position, transform.rotation);
                    Destroy(ghost, 0.5f);
                    ghostTimer = 0f;
                }
            }
            else
            {
                if (rend != null)
                    rend.material.color = Color.Lerp(rend.material.color, originalColor, 10f * Time.deltaTime);
            }
        }
    }

    void Record()
    {
        states.Add(new State
        {
            pos = transform.position,
            rot = transform.rotation
        });

        if (states.Count > 600)
            states.RemoveAt(0);
    }

    void Rewind()
    {
        if (states.Count == 0) return;

        State s = states[states.Count - 1];
        transform.position = s.pos;
        transform.rotation = s.rot;
        states.RemoveAt(states.Count - 1);

        ghostTimer += Time.unscaledDeltaTime;
        if (ghostTimer >= ghostSpawnInterval && ghostPrefab != null)
        {
            GameObject ghost = Instantiate(ghostPrefab, transform.position, transform.rotation);
            Destroy(ghost, 0.5f);
            ghostTimer = 0f;
        }
    }
}
