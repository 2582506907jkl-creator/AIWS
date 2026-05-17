using UnityEngine;
using UnityEngine.UI;
using TMPro;   // TMP 命名空间
using System.Collections;

public class SimpleBlackout : MonoBehaviour
{
    public Image blackImage;          // 全屏黑色图片
    public TMP_Text messageText;      // TMP 文本组件
    public float fadeDuration = 0.5f; // 黑屏渐变时间
    public float textDisplayTime = 3f; // 文字显示时间

    private bool isActive = false;

    public void ShowBlackout(string text, System.Action onComplete = null)
    {
        if (isActive) return;
        isActive = true;

        if (messageText != null)
        {
            messageText.text = text;
            messageText.gameObject.SetActive(true);
        }

        StartCoroutine(FadeToBlack(onComplete));
    }

    private IEnumerator FadeToBlack(System.Action onComplete)
    {
        blackImage.gameObject.SetActive(true);
        Color c = blackImage.color;
        c.a = 0;
        blackImage.color = c;

        float elapsed = 0;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Clamp01(elapsed / fadeDuration);
            blackImage.color = c;
            yield return null;
        }
        c.a = 1;
        blackImage.color = c;

        yield return new WaitForSeconds(textDisplayTime);
        onComplete?.Invoke();
    }
}