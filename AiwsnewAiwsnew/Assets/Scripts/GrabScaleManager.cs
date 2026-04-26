using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
/// <summary>
/// ���ڿ�ץȡ�����ϣ�ץȡ��������������ץȡ˲�����ǰ�����ƶ��������������塣
/// ��ǰ�ƶ� �� �Ŵ������ƶ� �� ��С��
/// </summary>
[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class GrabScaleManager : MonoBehaviour
{
    [Header("��������")]
    [Tooltip("���������ȣ�ÿ�ƶ�1�ף����ű��ʱ仯����")]
    [SerializeField] private float scaleSensitivity = 1.0f;
    [Tooltip("��С���ű��ʣ�����ڳ�ʼ��С��")]
    [SerializeField] private float minScaleMultiplier = 0.1f;
    [Tooltip("������ű��ʣ�����ڳ�ʼ��С��")]
    [SerializeField] private float maxScaleMultiplier = 10.0f;
    // ץȡ˲���¼�����״̬
    private Vector3 grabCameraPosition;
    private Vector3 grabCameraForward;
    // ץȡ˲�������λ�ú�����
    private Vector3 grabObjectPosition;
    private Vector3 initialScale;
    // �Ƿ����ڱ�ץȡ
    private bool isGrabbed = false;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    void Awake()
    {

        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    }
    // Update is called once per frame
    void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }
    void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }
    /// <summary>
    /// ץȡ˲�䣺��¼���λ�á������Լ�����ĳ�ʼλ�ú�����
    /// </summary>
    private void OnGrabbed(SelectEnterEventArgs args)
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("GrabScaleManager: �Ҳ�������� (Main Camera)���޷���¼ץȡʱ�����״̬��");
            return;
        }
        // ��¼ץȡ˲������λ�úͳ���
        grabCameraPosition = cam.transform.position;
        grabCameraForward = cam.transform.forward;
        // ��¼ץȡ˲�������λ�ú�����
        grabObjectPosition = transform.position;
        initialScale = transform.localScale;
        isGrabbed = true;
    }
    /// <summary>
    /// �ɿ�ʱֹͣ�����߼�
    /// </summary>
    private void OnReleased(SelectExitEventArgs args)
    {
        isGrabbed = false;
    }
    void Update()
    {

        if (!isGrabbed) return;
        // �������嵱ǰλ�������ץȡʱλ�õ�λ������
        Vector3 displacement = transform.position - grabObjectPosition;
        // ��λ��ͶӰ��ץȡ˲������ǰ��������
        float forwardDistance = Vector3.Dot(displacement, grabCameraForward);
        // ����ǰ�����������ű���
        // forwardDistance > 0 �� ��ǰ�ƶ� �� �Ŵ�
        // forwardDistance < 0 �� �����ƶ� �� ��С
        float scaleMultiplier = 1.0f + forwardDistance * scaleSensitivity;
        // �������ŷ�Χ
        scaleMultiplier = Mathf.Clamp(scaleMultiplier, minScaleMultiplier, maxScaleMultiplier);
        // Ӧ������
        transform.localScale = initialScale * scaleMultiplier;
    }
}
