using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PaperClick : MonoBehaviour
{
    [Header("UI to show when clicked")]
    public GameObject infoPanel;   // Õœ»Îƒ„µƒΩÈ…‹UI£®Canvas£©

    private XRSimpleInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
        if (interactable == null)
            interactable = gameObject.AddComponent<XRSimpleInteractable>();
    }

    private void OnEnable()
    {
        interactable.selectEntered.AddListener(OnPaperSelected);
    }

    private void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnPaperSelected);
    }

    private void OnPaperSelected(SelectEnterEventArgs args)
    {
        if (infoPanel != null)
        {
            infoPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("PaperClick: infoPanel is not assigned!");
        }
    }
}