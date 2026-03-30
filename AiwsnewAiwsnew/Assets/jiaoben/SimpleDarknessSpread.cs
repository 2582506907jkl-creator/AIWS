using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SimpleDarknessSpread : MonoBehaviour
{
    public Image darknessPanel;           // 黑色面板
    public float spreadDuration = 1.5f;   // 扩散时长

    private bool isSpreading = false;
    private System.Action onComplete;

    void Start()
    {
        if (darknessPanel != null)
        {
            Color c = darknessPanel.color;
            c.a = 0f;
            darknessPanel.color = c;
        }
    }

    public void StartSpread(System.Action onFinished)
    {
        if (isSpreading) return;
        isSpreading = true;
        onComplete = onFinished;
        StartCoroutine(SpreadCoroutine());
    }

    IEnumerator SpreadCoroutine()
    {
        float elapsed = 0;
        while (elapsed < spreadDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / spreadDuration);
            Color c = darknessPanel.color;
            c.a = alpha;
            darknessPanel.color = c;
            yield return null;
        }

        Color finalColor = darknessPanel.color;
        finalColor.a = 1f;
        darknessPanel.color = finalColor;

        isSpreading = false;
        onComplete?.Invoke();
    }

    public void ResetDarkness()
    {
        Color c = darknessPanel.color;
        c.a = 0f;
        darknessPanel.color = c;
    }
}
