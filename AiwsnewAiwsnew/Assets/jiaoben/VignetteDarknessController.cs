using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using DG.Tweening;
using System.Collections;

public class VignetteDarknessController : MonoBehaviour
{
    public Volume volume;
    public float blinkInDuration = 0.1f;   // 闭眼时长
    public float blinkOutDuration = 0.2f;  // 睁眼时长
    public float finalIntensity = 0f;      // 最终强度（默认0）

    private Vignette vignette;

    void Start()
    {
        if (volume == null) volume = GetComponent<Volume>();
        if (volume != null && volume.profile.TryGet<Vignette>(out vignette))
        {
            vignette.intensity.value = 0f;
        }
    }

    /// <summary>
    /// 多次眨眼，最后一次只闭眼不睁眼，最终保持全黑
    /// </summary>
    /// <param name="times">眨眼次数（最后一次不睁眼）</param>
    /// <param name="onComplete">完成回调（最后一次闭眼后调用）</param>
    public void BlinkMultipleKeepDark(int times, System.Action onComplete = null)
    {
        if (vignette == null)
        {
            onComplete?.Invoke();
            return;
        }
        StartCoroutine(BlinkMultipleKeepDarkCoroutine(times, onComplete));
    }

    private IEnumerator BlinkMultipleKeepDarkCoroutine(int times, System.Action onComplete)
    {
        for (int i = 0; i < times; i++)
        {
            // 闭眼
            float t = 0;
            while (t < blinkInDuration)
            {
                t += Time.deltaTime;
                float intensity = Mathf.Lerp(0, 1, t / blinkInDuration);
                vignette.intensity.value = intensity;
                yield return null;
            }
            vignette.intensity.value = 1;

            // 如果不是最后一次，则睁眼
            if (i < times - 1)
            {
                t = 0;
                while (t < blinkOutDuration)
                {
                    t += Time.deltaTime;
                    float intensity = Mathf.Lerp(1, finalIntensity, t / blinkOutDuration);
                    vignette.intensity.value = intensity;
                    yield return null;
                }
                vignette.intensity.value = finalIntensity;
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                // 最后一次：不睁眼，保持全黑，直接完成
                onComplete?.Invoke();
                yield break;
            }
        }
    }

    public void SetFullBlack()
    {
        if (vignette != null) vignette.intensity.value = 1f;
    }

    public void ResetDarkness()
    {
        if (vignette != null) vignette.intensity.value = finalIntensity;
    }
}