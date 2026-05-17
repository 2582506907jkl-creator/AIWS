using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PhotoFrameClick : MonoBehaviour
{
    [Header("自动查找，可留空")]
    public VignetteFadeIn vignetteFade;      // 自动查找场景中的 VignetteFadeIn
    public GameObject messageCanvas;         // 自动查找名为 "MessageCanvas" 的对象
    public TMP_Text messageText;             // 自动在 MessageCanvas 下查找 TMP_Text
    public string displayText = "你陷入了一片黑暗...";

    private bool triggered = false;

    void Start()
    {
        // 自动查找 VignetteFadeIn（挂载在 GlobalVolume 上）
        if (vignetteFade == null)
            vignetteFade = FindObjectOfType<VignetteFadeIn>();

        // 自动查找 MessageCanvas（按名称）
        if (messageCanvas == null)
            messageCanvas = GameObject.Find("MessageCanvas");

        // 如果找到了 MessageCanvas，则自动查找其下的 TMP_Text 组件
        if (messageCanvas != null && messageText == null)
            messageText = messageCanvas.GetComponentInChildren<TMP_Text>();

        // 确保文字 Canvas 初始隐藏
        if (messageCanvas != null)
            messageCanvas.SetActive(false);

        // 设置 XR 交互
        var interactable = GetComponent<XRSimpleInteractable>();
        if (interactable != null)
            interactable.selectEntered.AddListener(_ => OnClick());
        else
            Debug.LogWarning("相框缺少 XRSimpleInteractable 组件");
    }

    void OnClick()
    {
        if (triggered) return;
        triggered = true;

        // 显示文字
        if (messageText != null)
            messageText.text = displayText;
        if (messageCanvas != null)
            messageCanvas.SetActive(true);

        // 开始暗角渐变
        if (vignetteFade != null)
            vignetteFade.StartFade();
        else
            Debug.LogError("未找到 VignetteFadeIn 组件，请确保 GlobalVolume 上挂载了该脚本");
    }
}