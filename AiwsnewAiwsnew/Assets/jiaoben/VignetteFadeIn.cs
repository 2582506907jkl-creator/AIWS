using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using DG.Tweening;

public class VignetteFadeIn : MonoBehaviour
{
    public Volume volume;
    public float targetIntensity = 0.8f;   // 最终暗角强度
    public float fadeDuration = 1.5f;      // 渐变时间

    private Vignette vignette;

    void Start()
    {
        if (volume == null) volume = GetComponent<Volume>();
        if (volume != null && volume.profile.TryGet<Vignette>(out vignette))
        {
            vignette.intensity.value = 0f;
        }
    }

    public void StartFade()
    {
        if (vignette == null) return;
        DOTween.To(() => vignette.intensity.value, x => vignette.intensity.value = x, targetIntensity, fadeDuration)
            .SetEase(Ease.OutCubic);
    }
}