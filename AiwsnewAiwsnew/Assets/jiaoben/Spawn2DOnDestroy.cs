using UnityEngine;

public class Show2DOnDestroy : MonoBehaviour
{
    public GameObject target2D; // 在Inspector中拖入场景中的2D物体

    private void OnDestroy()
    {
        if (target2D != null)
            target2D.SetActive(true);
    }
}