using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // 阶段管理
    [HideInInspector] public int currentPhase = 0; // 0:初始, 1:家长2出现, 2:家长3出现

    // 第一阶段：拾取物品计数（由 PickupItem 增加）
    public int itemsRequiredForPhase1 = 3;
    private int currentItemCount = 0;

    // 第二阶段：放置物品计数（三种不同物品，各放一次）
    private bool diaryPlaced = false;
    private bool medicinePlaced = false;
    private bool photoPlaced = false;
    private int totalPlaced => (diaryPlaced ? 1 : 0) + (medicinePlaced ? 1 : 0) + (photoPlaced ? 1 : 0);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 拾取物品时调用（由 PickupItem 脚本调用）
    public void AddItem()
    {
        if (currentPhase != 0) return; // 只在第一阶段需要拾取

        currentItemCount++;
        Debug.Log($"拾取物品: {currentItemCount} / {itemsRequiredForPhase1}");

        if (currentItemCount >= itemsRequiredForPhase1)
        {
            // 触发第二阶段：家长2出现
            currentPhase = 1;
            Debug.Log("拾取满3个，家长2出现！");
        }
    }

    // 放置物品时调用（由各个放置触发器调用）
    public void TryPlaceItem(string itemType)
    {
        if (currentPhase != 1)
        {
            Debug.Log("当前不是放置阶段，无法放置");
            return;
        }

        bool alreadyPlaced = false;
        switch (itemType)
        {
            case "Diary":
                if (diaryPlaced) alreadyPlaced = true;
                else diaryPlaced = true;
                break;
            case "Medicine":
                if (medicinePlaced) alreadyPlaced = true;
                else medicinePlaced = true;
                break;
            case "Photo":
                if (photoPlaced) alreadyPlaced = true;
                else photoPlaced = true;
                break;
            default:
                Debug.LogWarning("未知物品类型: " + itemType);
                return;
        }

        if (alreadyPlaced)
        {
            Debug.Log($"已经放置过 {itemType} 了，不能重复放置");
            return;
        }

        Debug.Log($"成功放置 {itemType}，当前总放置数: {totalPlaced} / 3");

        if (totalPlaced == 3)
        {
            currentPhase = 2;
            Debug.Log("所有物品放置完毕，家长3出现！");
        }
    }

    // 可选：供外部查询放置状态
    public bool IsPhaseComplete(int phase)
    {
        if (phase == 1) return currentItemCount >= itemsRequiredForPhase1;
        if (phase == 2) return totalPlaced == 3;
        return false;
    }
}