using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music Settings")]
    private AudioSource musicSource;
    [SerializeField] private float musicVolume = 0.5f;

    [Header("SFX Settings")]
    private AudioSource sfxSource;
    [SerializeField] private float sfxVolume = 0.5f;

    [System.Serializable]
    public class NamedClip
    {
        public string id;
        public AudioClip clip;
    }

    public List<NamedClip> sfxClips;
    private Dictionary<string, AudioClip> sfxDict;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;

            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;

            musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
            musicSource.volume = musicVolume;
            sfxSource.volume = sfxVolume;

            // Init SFX dictionary
            sfxDict = new Dictionary<string, AudioClip>();
            foreach (var clip in sfxClips)
            {
                if (!sfxDict.ContainsKey(clip.id))
                {
                    sfxDict.Add(clip.id, clip.clip);
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ----- Music -----
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource.clip == clip) return;
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic() => musicSource.Stop();

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        musicSource.volume = volume;
    }

    public float GetMusicVolume() => musicVolume;

    // ----- SFX -----
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlaySFX(string id)
    {
        if (sfxDict.TryGetValue(id, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
        else
        {
            Debug.LogWarning($"SFX ID '{id}' not found!");
        }
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        sfxSource.volume = volume;
    }

    public float GetSFXVolume() => sfxVolume;
}
