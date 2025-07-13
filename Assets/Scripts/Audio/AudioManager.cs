using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    [SerializeField] private float musicVolume = 0.5f;
    [SerializeField] private float sfxVolume = 0.5f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Tạo nguồn nhạc và hiệu ứng âm thanh riêng
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;

            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;

            // Load volume từ setting đã lưu
            SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 0.5f));
            SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 0.5f));
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Music
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip) return;
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

    // SFX
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        sfxSource.volume = volume;
    }

    public float GetSFXVolume() => sfxVolume;
}
