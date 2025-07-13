using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class IntroVideoManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public RawImage videoDisplay;
    public GameObject videoPanel;
    public TextMeshProUGUI skipText;

    public float holdToSkipDuration = 3f;
    private float skipTimer = 0f;
    private bool isSkipping = false;
    private bool hasPlayed = false;

    // Add these fields to store the previous music clip and volume
    private AudioClip previousMusicClip;
    private float previousMusicVolume;

    private void Start()
    {
        videoPanel.SetActive(false);
        skipText.gameObject.SetActive(false);
    }

    public void PlayIntro()
    {
        if (hasPlayed) return; // Chỉ chạy 1 lần
        hasPlayed = true;
        videoPanel.SetActive(true);

        // Save current music state and stop music
        previousMusicClip = AudioManager.Instance?.GetComponent<AudioSource>()?.clip;
        previousMusicVolume = AudioManager.Instance?.GetMusicVolume() ?? 0.5f;
        AudioManager.Instance?.StopMusic();

        string path = System.IO.Path.Combine(Application.streamingAssetsPath, "intro.mp4");
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = path;
        videoPlayer.Prepare();

        videoPlayer.prepareCompleted += OnPrepared;
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void OnPrepared(VideoPlayer vp)
    {
        videoPanel.SetActive(true);
        skipText.gameObject.SetActive(true);

        // Assign the video texture to the RawImage
        videoDisplay.texture = videoPlayer.texture;

        videoPlayer.Play();
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        EndIntro();
    }

    void EndIntro()
    {
        videoPlayer.Stop();
        videoPanel.SetActive(false);
        skipText.gameObject.SetActive(false);

        // Restore previous music
        if (previousMusicClip != null)
        {
            AudioManager.Instance?.PlayMusic(previousMusicClip);
            AudioManager.Instance?.SetMusicVolume(previousMusicVolume);
        }

        // Chuyển sang panel chọn nhân vật, hoặc load scene tùy anh
        FindObjectOfType<MainMenuManager>()?.OpenCharacterSelection();
    }

    private void Update()
    {
        if (!videoPlayer.isPlaying) return;

        if (Input.anyKey)
        {
            skipTimer += Time.deltaTime;
            skipText.text = $"Hold any key in ({(holdToSkipDuration - skipTimer):F1}s) to skip ";
            if (skipTimer >= holdToSkipDuration && !isSkipping)
            {
                isSkipping = true;
                EndIntro();
            }
        }
        else
        {
            skipTimer = 0f;
            skipText.text = "Hold any key for 3s to skip";
        }
    }
}
