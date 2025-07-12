using UnityEngine;
using System.Collections.Generic;

public class SFXController : MonoBehaviour
{
    public static SFXController Instance;

    [System.Serializable]
    public class NamedClip
    {
        public string id;
        public AudioClip clip;
    }

    public List<NamedClip> clips;
    private Dictionary<string, AudioClip> clipDict;
    private AudioSource sfxSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;

            clipDict = new Dictionary<string, AudioClip>();
            foreach (var c in clips)
            {
                if (!clipDict.ContainsKey(c.id))
                    clipDict.Add(c.id, c.clip);
            }

            // Load volume from PlayerPrefs
            float savedVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
            sfxSource.volume = savedVolume;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Play(string id)
    {
        if (clipDict.ContainsKey(id))
        {
            sfxSource.PlayOneShot(clipDict[id]);
        }
    }

    public void SetVolume(float value)
    {
        sfxSource.volume = value;
    }
}
