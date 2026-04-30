using UnityEngine;
using UnityEngine.UI;          // ← 必须添加这一行，否则 Image 无法识别
using System.Collections;

public class SceneFadeIn : MonoBehaviour
{
    public Image fadeImage;          // 黑屏Image组件（需要在Inspector中拖拽赋值）
    public float fadeDuration = 1.0f;

    IEnumerator Start()
    {
        // 确保黑屏Image是完全不透明的（Alpha = 1）
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 1f;
            fadeImage.color = c;
        }

        // 等待一帧，确保UI准备就绪
        yield return null;

        // 淡出（逐渐透明）
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            if (fadeImage != null)
            {
                Color c = fadeImage.color;
                c.a = alpha;
                fadeImage.color = c;
            }
            yield return null;
        }

        // 确保最终完全透明
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }
    }
}