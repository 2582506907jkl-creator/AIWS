using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelChanger : MonoBehaviour
{
    [Header("场景跳转设置")]
    public string targetSceneName;
    public bool loadAsync = true;
    public GameObject loadingScreen;

    private void OnTriggerEnter(Collider other)
    {
        // 只通过 Tag 来判断是否为玩家，不再需要 XROrigin 组件，也无需添加命名空间
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[LevelChanger] ✅ 检测到玩家（通过Tag识别）！开始跳转到场景：{targetSceneName}");

            if (loadAsync)
                StartCoroutine(LoadSceneAsyncCoroutine());
            else
                SceneManager.LoadScene(targetSceneName);
        }
    }

    private IEnumerator LoadSceneAsyncCoroutine()
    {
        if (loadingScreen != null)
            loadingScreen.SetActive(true);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetSceneName);
        if (asyncLoad == null)
        {
            Debug.LogError($"[LevelChanger] ❌ 无法加载场景：{targetSceneName}，请检查 Build Settings 中是否添加了该场景。");
            yield break;
        }

        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            Debug.Log($"[LevelChanger] 加载进度：{asyncLoad.progress * 100:F1}%");
            yield return null;
        }

        Debug.Log("[LevelChanger] 场景资源已就绪，即将切换...");
        yield return new WaitForSeconds(0.5f); // 一个短暂延时，确保加载界面稳定显示
        asyncLoad.allowSceneActivation = true;
    }
}