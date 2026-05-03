using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class PickupSwitchTo3D : MonoBehaviour
{
    public GameObject target3D;   // 拖入场景中已经摆放好的3D物体（初始未激活）

    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
            grabInteractable.selectEntered.AddListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        StartCoroutine(DelayedSwitch());
    }

    private IEnumerator DelayedSwitch()
    {
        yield return null; // 等待一帧

        if (target3D != null)
        {
            target3D.SetActive(true);   // 显示3D物体
            Debug.Log("3D物体已显示: " + target3D.name);
        }
        else
        {
            Debug.LogError("target3D 未在Inspector中指定！");
        }

        gameObject.SetActive(false);    // 隐藏2D图片
    }
}