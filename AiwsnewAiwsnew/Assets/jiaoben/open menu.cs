using UnityEngine;
using UnityEngine.Video;

public class VideoEndUI : MonoBehaviour
{
    public GameObject buttons;
    public VideoPlayer videoPlayer;

    void Start()
    {
        buttons.SetActive(false);

        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        buttons.SetActive(true);
    }
}