using UnityEngine;

public class SceneMusicTrigger : MonoBehaviour
{
    [Header("Music Settings")]
    public AudioClip backgroundMusic;

    [Tooltip("Nếu chọn, sẽ tự động phát nhạc này khi scene được load.")]
    public bool playOnStart = true;

    private void Start()
    {
        if (playOnStart && backgroundMusic != null)
        {
            AudioManager.Instance.PlayMusic(backgroundMusic);
        }
    }

    // Gọi thủ công nếu muốn đổi nhạc lúc khác
    public void PlayMusic()
    {
        if (backgroundMusic != null)
        {
            AudioManager.Instance.PlayMusic(backgroundMusic);
        }
    }
}
