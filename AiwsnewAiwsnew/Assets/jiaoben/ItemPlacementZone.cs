using UnityEngine;

public class ItemPlacementZone : MonoBehaviour
{
    [Header("需要放置的物品类型")]
    public string requiredItemType; // 在Inspector中填写: "Diary", "Medicine", "Photo"

    [Header("物品检测")]
    public string itemTag = "PlaceableItem"; // 所有可放置物品统一标签

    private void OnTriggerEnter(Collider other)
    {
        // 检查进入触发器的物体是否是可放置物品
        if (!other.CompareTag(itemTag)) return;

        // 根据物品的名称判断具体类型（你也可以用自定义组件，这里用名称包含关键字）
        string itemName = other.gameObject.name;
        bool isMatch = false;

        if (requiredItemType == "Diary" && (itemName.Contains("Diary") || itemName.Contains("日记")))
            isMatch = true;
        else if (requiredItemType == "Medicine" && (itemName.Contains("Medicine") || itemName.Contains("药品") || itemName.Contains("药")))
            isMatch = true;
        else if (requiredItemType == "Photo" && (itemName.Contains("Photo") || itemName.Contains("照片")))
            isMatch = true;

        if (!isMatch)
        {
            Debug.Log($"物品 {itemName} 不是所需的 {requiredItemType}");
            return;
        }

        // 通知 GameManager 尝试放置
        GameManager.Instance.TryPlaceItem(requiredItemType);

        // 销毁物品
        Destroy(other.gameObject);

        // 可选：播放放置音效、特效，或者禁用此触发器
        // GetComponent<Collider>().enabled = false;
    }
}