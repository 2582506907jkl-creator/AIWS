using UnityEngine;
using System.Collections;

public class LampLightController : MonoBehaviour
{
    public Light lampLight;

    public Color coldColor = new Color(0.6f, 0.8f, 1f);

    public Color warmColor = new Color(1f, 0.8f, 0.5f);

    public float transitionTime = 2f;

    public void SetColdLight()
    {
        StopAllCoroutines();
        StartCoroutine(ChangeLight(coldColor));
    }

    public void SetWarmLight()
    {
        StopAllCoroutines();
        StartCoroutine(ChangeLight(warmColor));
    }

    IEnumerator ChangeLight(Color targetColor)
    {
        Color startColor = lampLight.color;

        float time = 0;

        while (time < transitionTime)
        {
            lampLight.color = Color.Lerp(startColor, targetColor, time / transitionTime);

            time += Time.deltaTime;

            yield return null;
        }

        lampLight.color = targetColor;
    }
}
