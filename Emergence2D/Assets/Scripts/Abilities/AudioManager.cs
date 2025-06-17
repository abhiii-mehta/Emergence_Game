using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Clips")]
    public AudioClip nukeClip, moneyClip, rainClip, angryClip, condomClip, spawnClip;

    private AudioSource sfxSource;
    private AudioSource loopSource;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        loopSource = gameObject.AddComponent<AudioSource>();
        loopSource.loop = true;
        loopSource.playOnAwake = false;
    }

    public void PlaySFX(string id)
    {
        AudioClip clip = GetClip(id);
        if (clip) sfxSource.PlayOneShot(clip);
    }

    public void PlayLoopingSFX(string id)
    {
        AudioClip clip = GetClip(id);
        if (clip && loopSource.clip != clip)
        {
            loopSource.clip = clip;
            loopSource.Play();
        }
    }

    public void StopLoopingSFX()
    {
        loopSource.Stop();
        loopSource.clip = null;
    }

    private AudioClip GetClip(string id)
    {
        return id switch
        {
            "nuke" => nukeClip,
            "money" => moneyClip,
            "rain" => rainClip,
            "angry" => angryClip,
            "condom" => condomClip,
            "spawn" => spawnClip,
            _ => null
        };
    }
}
