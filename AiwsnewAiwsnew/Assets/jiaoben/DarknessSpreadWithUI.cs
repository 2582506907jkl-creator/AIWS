using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class DarknessSpreadWithUI : MonoBehaviour
{
    public RectTransform darknessCircle;
    public Camera targetCamera;          // 仅用于获取相框屏幕位置，不用于渲染
    public Transform photoFrame;
    public float spreadDuration = 1.5f;
    public float startSize = 0.1f;
    public float endSize = 5f;           // 可调大确保覆盖全屏

    private Canvas canvas;
    private bool isSpreading = false;
    private System.Action onComplete;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("DarknessSpreadWithUI 必须放在 Canvas 下或 Canvas 的子物体下");
            return;
        }

        // 确保 Canvas 是 Overlay 模式（默认就是）
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        if (darknessCircle == null)
        {
            Debug.LogError("DarknessSpreadWithUI: darknessCircle 未赋值！");
            return;
        }

        darknessCircle.localScale = Vector3.one * startSize;
        darknessCircle.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isSpreading && photoFrame != null && targetCamera != null)
        {
            Vector3 screenPos = targetCamera.WorldToViewportPoint(photoFrame.position);
            Vector2 canvasPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                new Vector2(screenPos.x * Screen.width, screenPos.y * Screen.height),
                null,   // Overlay 模式下为 null
                out canvasPos
            );
            darknessCircle.anchoredPosition = canvasPos;
        }
    }

    public void StartSpread(Transform frame, System.Action onFinished)
    {
        if (isSpreading) return;

        if (darknessCircle == null)
        {
            Debug.LogError("StartSpread 失败：darknessCircle 为空");
            onFinished?.Invoke();
            return;
        }

        photoFrame = frame;
        onComplete = onFinished;
        isSpreading = true;

        darknessCircle.gameObject.SetActive(true);
        darknessCircle.localScale = Vector3.one * startSize;

        // 强制设置初始位置（防止第一帧偏移）
        if (targetCamera != null && photoFrame != null)
        {
            Vector3 screenPos = targetCamera.WorldToViewportPoint(photoFrame.position);
            Vector2 canvasPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                new Vector2(screenPos.x * Screen.width, screenPos.y * Screen.height),
                null,
                out canvasPos
            );
            darknessCircle.anchoredPosition = canvasPos;
        }

        darknessCircle.DOScale(endSize, spreadDuration)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                isSpreading = false;
                onComplete?.Invoke();
            });
    }

    public void ResetDarkness()
    {
        darknessCircle.gameObject.SetActive(false);
        isSpreading = false;
    }
}