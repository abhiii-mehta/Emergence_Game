using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Clips")]
    public AudioClip nukeClip, moneyClip, rainClip, angryClip, condomClip, spawnClip;

    [Header("Music")]
    [SerializeField] private AudioClip bgmClip;
    private AudioSource bgmSource;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float bgmVolume = 0.5f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float loopSfxVolume = 1f;

    private AudioSource sfxSource;
    private AudioSource loopSource;

    [SerializeField] private AudioClip angryVsAngryClip;
    [SerializeField] private AudioClip sadVsSadClip;
    [SerializeField] private AudioClip happyVsHappyClip;
    [SerializeField] private AudioClip loveVsLoveClip;
    [SerializeField] private AudioClip sadLoveCryClip;
    [SerializeField] private AudioClip tooManyNPCsClip;


    private Dictionary<string, AudioClip> sfxMap;
    void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        loopSource = gameObject.AddComponent<AudioSource>();
        loopSource.loop = true;
        loopSource.playOnAwake = false;

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.clip = bgmClip;
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;

        bgmSource.volume = bgmVolume;
        sfxSource.volume = sfxVolume;
        loopSource.volume = loopSfxVolume;

        sfxMap = new Dictionary<string, AudioClip>
    {
        { "angry_vs_angry", angryVsAngryClip },
        { "sad_vs_sad", sadVsSadClip },
        { "happy_vs_happy", happyVsHappyClip },
        { "love_vs_love", loveVsLoveClip },
        { "sad_love_cry", sadLoveCryClip },
        { "too_many_npcs", tooManyNPCsClip },
    };

        if (!bgmSource.isPlaying)
            bgmSource.Play();

        DontDestroyOnLoad(gameObject);
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

    public void PlaySFXTemporary(string id, float duration = 2f)
    {
        AudioClip clip = GetClip(id);
        if (clip == null) return;

        GameObject tempGO = new GameObject($"TempAudio_{id}");
        AudioSource tempSource = tempGO.AddComponent<AudioSource>();
        tempSource.playOnAwake = false;
        tempSource.clip = clip;
        tempSource.Play();

        Destroy(tempGO, duration);
    }

    private AudioClip GetClip(string id)
    {
        if (sfxMap.TryGetValue(id, out AudioClip clip)) return clip;

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
