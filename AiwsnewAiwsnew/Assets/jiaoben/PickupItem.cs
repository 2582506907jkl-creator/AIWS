using UnityEngine;

public class PickupItem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 检查是否是玩家碰到了物品（通过Tag判断）
        if (other.CompareTag("Player"))
        {
            // 通知GameManager物品被收集了
            GameManager.Instance.AddItem();

            // 你可以在这里播放收集音效或特效
            // AudioSource.PlayClipAtPoint(collectSound, transform.position);

            // 最后销毁物品
            Destroy(gameObject);
        }
    }
}