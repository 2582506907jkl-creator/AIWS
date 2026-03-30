using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private CanvasGroup fadeImage; // 全屏黑色Image的Canvas Group
    [SerializeField] private float fadeDuration = 2f; // 渐变持续时间
    [SerializeField] private string targetSceneName = "GameScene"; // 目标场景名称

    private bool isFading = false;

    void Start()
    {
        // 确保初始状态为完全黑屏
        fadeImage.alpha = 1f;
    }

    // 开始过渡到游戏场景
    public void StartTransition()
    {
        if (isFading) return;
        isFading = true;

        // 模拟"睁开眼睛"的效果：从完全黑场(1)到透明(0)
        StartCoroutine(Fade(1f, 0f, () => {
            // 渐变完成后加载目标场景
            SceneManager.LoadScene(targetSceneName);
        }));
    }

    // 协程实现渐变效果
    System.Collections.IEnumerator Fade(float startAlpha, float endAlpha, System.Action onComplete = null)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeImage.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            yield return null;
        }

        fadeImage.alpha = endAlpha;
        onComplete?.Invoke();
        isFading = false;
    }
}
