using UnityEngine;
using System.Collections;

public class AutoHideUI : MonoBehaviour
{
    [Tooltip("显示多少秒后自动隐藏")]
    public float displayDuration = 5f;

    private Coroutine autoHideCoroutine;

    private void OnEnable()
    {
        // 每次启用（显示）时，启动或重启倒计时
        if (autoHideCoroutine != null)
            StopCoroutine(autoHideCoroutine);
        autoHideCoroutine = StartCoroutine(AutoHideRoutine());
    }

    private void OnDisable()
    {
        // UI隐藏时，如果有正在运行的倒计时，就停止它
        if (autoHideCoroutine != null)
        {
            StopCoroutine(autoHideCoroutine);
            autoHideCoroutine = null;
        }
    }

    private IEnumerator AutoHideRoutine()
    {
        // 等待指定的秒数
        yield return new WaitForSeconds(displayDuration);

        // 时间到，关闭这个UI物体（假设它自身就是UI的根）
        gameObject.SetActive(false);
        autoHideCoroutine = null;
    }
}