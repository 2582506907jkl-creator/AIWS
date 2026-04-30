using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class PickupSwitchTo3D : MonoBehaviour
{
    public GameObject target3D; // 在Inspector中拖入场景中的3D物体

    private void Start()
    {
        XRGrabInteractable grab = GetComponent<XRGrabInteractable>();
        grab.selectEntered.AddListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // 隐藏自己（2D物体）
        gameObject.SetActive(false);

        // 显示3D物体
        if (target3D != null)
            target3D.SetActive(true);
    }
}