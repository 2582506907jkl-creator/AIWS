using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 使用单例模式，让其他脚本可以方便地访问这个管理器
    public static GameManager Instance;

    // 当前玩家已经收集的物品数量
    [HideInInspector] public int currentItemCount = 0;

    // 你设定好的触发条件：第一阶段需要3个物品，第二阶段再需要3个
    public int itemsRequiredForPhase1 = 3;
    public int itemsRequiredForPhase2 = 6; // 累计数量

    // 当前游戏处于哪个阶段（0:未开始, 1:已触发角色1, 2:已触发角色2）
    [HideInInspector] public int currentPhase = 0;

    private void Awake()
    {
        // 单例模式初始化，确保场景中只有一个GameManager
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject); // 切换场景时不销毁
    }

    // 当玩家捡起一个物品时，调用这个方法来增加计数并检查进度
    public void AddItem()
    {
        currentItemCount++;
        Debug.Log($"物品已收集: {currentItemCount}");

        // 触发角色2消失和角色3出现的条件
        if (currentItemCount == itemsRequiredForPhase2)
        {
            // 注意：角色2的消失和角色3的出现，将在角色1的区域触发逻辑里处理。
            // 这里只记录阶段，不直接操作角色。
            currentPhase = 2;
            Debug.Log("收集满6个物品，已解锁第三阶段。");
        }
        // 触发角色1消失和角色2出现的条件
        else if (currentItemCount == itemsRequiredForPhase1 && currentPhase == 0)
        {
            currentPhase = 1;
            Debug.Log("收集满3个物品，已解锁第二阶段。");
        }
    }
}