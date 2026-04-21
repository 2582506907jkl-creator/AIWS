using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class ZoneTrigger : MonoBehaviour
{
    public VideoPlayer videoPlayer;     // 从Inspector拖入摄像机的VideoPlayer组件
    public RawImage videoRawImage;      // 从Inspector拖入用于显示视频的RawImage
    public GameObject npcToHide;        // 从Inspector拖入角色1，触发后要隐藏它
    public int requiredPhase = 1;       // 触发这个区域需要的阶段（角色1区域为1）

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            // 检查当前阶段是否匹配
            if (GameManager.Instance.currentPhase == requiredPhase)
            {
                StartVideoSequence();
            }
            else
            {
                Debug.Log("当前阶段不匹配，无法触发");
            }
        }
    }

    void StartVideoSequence()
    {
        hasTriggered = true;
        // 隐藏当前区域对应的NPC
        if (npcToHide != null)
            npcToHide.SetActive(false);

        // 显示RawImage，准备播放视频
        videoRawImage.gameObject.SetActive(true);
        // 开始播放视频
        videoPlayer.Play();

        // 这里可以选择让玩家在视频播放时无法移动
        // 假设你有一个PlayerController，可以调用其SetMovementEnabled(false)方法

        // 监听视频播放结束事件
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        // 视频播放完毕，隐藏RawImage
        videoRawImage.gameObject.SetActive(false);
        // 如果之前禁用了玩家移动，这里需要重新启用
        // 比如: FindObjectOfType<PlayerController>().SetMovementEnabled(true);

        // 注意：角色2的出现由NPCController自己控制，这里不需要额外代码
        Debug.Log("视频播放完毕，阶段已切换，角色2应自动出现。");

        // 可选：视频播放完毕后，销毁这个触发器，防止二次触发
        Destroy(gameObject);
    }
}