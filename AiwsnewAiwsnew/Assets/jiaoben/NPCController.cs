using UnityEngine;

public class NPCController : MonoBehaviour
{
    // 这个角色应该在哪个阶段出现（例如：角色1是阶段1，角色2是阶段2）
    public int phaseToAppear = 1;
    // 这个角色应该在哪个阶段消失（例如：角色1在阶段1后消失，角色2在阶段2后消失）
    public int phaseToDisappear = 1;

    private void Start()
    {
        // 开始时，根据当前阶段决定是否显示自己
        UpdateVisibility();
    }

    private void Update()
    {
        // 每帧检查是否需要改变可见状态
        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        // 如果游戏管理器还不存在，就什么也不做
        if (GameManager.Instance == null) return;

        bool shouldBeActive = false;
        // 当当前阶段等于此角色应出现的阶段时，角色可见
        if (GameManager.Instance.currentPhase == phaseToAppear)
        {
            shouldBeActive = true;
        }
        // 如果当前阶段大于此角色应消失的阶段，角色不可见
        else if (GameManager.Instance.currentPhase > phaseToDisappear)
        {
            shouldBeActive = false;
        }
        // 其他情况，比如未达到出现阶段，也设为不可见
        else
        {
            shouldBeActive = false;
        }

        // 只有当状态发生变化时才执行，减少性能开销
        if (gameObject.activeSelf != shouldBeActive)
        {
            gameObject.SetActive(shouldBeActive);
            Debug.Log($"角色 {gameObject.name} 的可见性变更为: {shouldBeActive}");
        }
    }
}