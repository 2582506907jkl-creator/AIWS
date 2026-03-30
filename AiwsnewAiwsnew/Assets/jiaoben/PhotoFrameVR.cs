using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PhotoFrameVR : MonoBehaviour
{
    [Header("Vignette 控制")]
    public VignetteDarknessController darknessController;

    [Header("音频")]
    public AudioSource heartbeatAudio;

    [Header("晕眩效果")]
    public Camera playerHeadCamera;
    public float shakeDuration = 2.0f;       // 晃动持续时间（可以与闪烁总时长匹配）
    public float rotateMagnitude = 5f;
    public float positionMagnitude = 0.05f;

    [Header("场景转换")]
    public string nextSceneName = "OversizedBedroom";

    [Header("闪烁设置")]
    public int blinkTimes = 3;  // 闪烁次数（最后一次不睁眼）

    private bool triggered = false;

    void Start()
    {
        var interactable = GetComponent<XRSimpleInteractable>();
        if (interactable != null)
        {
            interactable.selectEntered.AddListener(_ => OnTouch());
        }
        else
        {
            Debug.LogWarning("相框上没有 XRSimpleInteractable 组件，无法响应触碰。");
        }

        if (playerHeadCamera == null) playerHeadCamera = Camera.main;
    }

    void OnTouch()
    {
        if (triggered) return;
        triggered = true;

        Debug.Log("触摸相框，开始效果");

        if (heartbeatAudio != null)
        {
            heartbeatAudio.loop = true;
            heartbeatAudio.Play();
        }

        // 启动晃动协程
        StartCoroutine(ShakeDuringBlink());

        // 启动闪烁（最后一次闭眼后保持全黑，然后加载场景）
        darknessController.BlinkMultipleKeepDark(blinkTimes, () =>
        {
            Debug.Log("闪烁完成，加载场景");
            // 可选：确保全黑（已在最后一次闭眼后全黑）
            // 直接加载场景
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                Debug.LogWarning("未设置场景名");
            }
        });
    }

    IEnumerator ShakeDuringBlink()
    {
        if (playerHeadCamera == null) yield break;

        Vector3 originalPos = playerHeadCamera.transform.localPosition;
        Quaternion originalRot = playerHeadCamera.transform.localRotation;

        float elapsed = 0f;
        // 晃动持续时间可以设定为闪烁总时长 + 一点余量，但闪烁完成后场景加载会打断，所以可以一直晃动直到加载
        while (true)
        {
            float rotX = Random.Range(-rotateMagnitude, rotateMagnitude);
            float rotY = Random.Range(-rotateMagnitude, rotateMagnitude);
            float posX = Random.Range(-positionMagnitude, positionMagnitude);
            float posY = Random.Range(-positionMagnitude, positionMagnitude);

            playerHeadCamera.transform.Rotate(rotX, rotY, 0, Space.Self);
            playerHeadCamera.transform.localPosition += new Vector3(posX, posY, 0);

            yield return null;
            // 无限循环，直到场景加载时被销毁
        }
        // 注意：场景加载时，这个协程会被终止，无需恢复。
    }
}