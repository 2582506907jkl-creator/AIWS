using UnityEngine;
using System.Collections;

public class OneTimeMoveBToC : MonoBehaviour
{
    public Transform pointB;
    public Transform pointC;
    public float moveSpeed = 2.0f;

    private Animator animator;
    private bool hasArrived = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        // 设置起点位置
        if (pointB != null) transform.position = pointB.position;
        // 开始移动
        animator.SetBool("isWalking", true);
    }

    void Update()
    {
        if (hasArrived) return;          // 已到达，不再移动
        if (pointC == null) return;

        // 向 C 点移动
        Vector3 direction = (pointC.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
        }

        transform.position = Vector3.MoveTowards(transform.position, pointC.position, moveSpeed * Time.deltaTime);

        // 到达检测
        if (Vector3.Distance(transform.position, pointC.position) < 0.1f)
        {
            hasArrived = true;
            animator.SetTrigger("toEnd");   // 触发结束动画
            // 注意：不要再次设置 isWalking，保持 false（默认 false）
        }
    }
}