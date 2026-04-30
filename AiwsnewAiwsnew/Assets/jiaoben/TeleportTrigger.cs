using UnityEngine;
using UnityEngine.UI;          // ← 必须添加，否则 Image 类型无法识别
using UnityEngine.SceneManagement;
using System.Collections;

public class TeleportTrigger : MonoBehaviour
{
    [Header("目标场景")]
    public string targetSceneName = "Level3";

    [Header("黑屏过渡设置")]
    public float fadeDuration = 1.0f;

    [Header("引用（可选，不填则自动查找）")]
    public Image fadeImage;

    private bool isTriggered = false;

    private void Start()
    {
        if (fadeImage == null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
                fadeImage = canvas.GetComponentInChildren<Image>();
        }

        if (fadeImage == null)
            Debug.LogWarning("没有找到黑屏Image组件，请手动指定或确保Canvas下有Image");

        // 确保初始完全透明
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;
        if (!other.CompareTag("Player")) return;

        isTriggered = true;
        StartCoroutine(TeleportSequence());
    }

    private IEnumerator TeleportSequence()
    {
        // 淡入黑屏
        yield return StartCoroutine(Fade(fadeDuration, 0f, 1f));

        yield return new WaitForSeconds(0.2f);

        // 加载场景
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetSceneName);
            asyncLoad.allowSceneActivation = true;
            while (!asyncLoad.isDone)
                yield return null;
        }
        else
        {
            Debug.LogError("目标场景名称为空，无法传送！");
        }
    }

    private IEnumerator Fade(float time, float startAlpha, float targetAlpha)
    {
        if (fadeImage == null) yield break;

        float elapsed = 0f;
        Color color = fadeImage.color;
        color.a = startAlpha;
        fadeImage.color = color;

        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / time;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            color.a = newAlpha;
            fadeImage.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        fadeImage.color = color;
    }
}