using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PhotoFrameClick : MonoBehaviour
{
    public SimpleBlackout blackoutController;   // 拖入 BlackoutCanvas
    public string displayText = "你感到一阵眩晕，陷入了黑暗……";

    void Start()
    {
        var interactable = GetComponent<XRSimpleInteractable>();
        if (interactable != null)
        {
            interactable.selectEntered.AddListener(_ => OnClick());
        }
    }

    void OnClick()
    {
        Debug.Log("相框被点击，显示黑屏文字");
        if (blackoutController != null)
        {
            blackoutController.ShowBlackout(displayText, () =>
            {
                Debug.Log("黑屏文字结束，可在此处加载场景");
                // 如果需要加载新场景，取消下面注释
                // UnityEngine.SceneManagement.SceneManager.LoadScene("NextSceneName");
            });
        }
    }
}