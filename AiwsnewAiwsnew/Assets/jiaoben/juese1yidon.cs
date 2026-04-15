using UnityEngine;

public class PatrolAB : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 2.0f;

    private Transform currentTarget;
    private Animator animator;
    private bool hasTriggeredEnd = false; // 防止在同一个到达点重复触发

    void Start()
    {
        animator = GetComponent<Animator>();
        if (pointA != null)
            currentTarget = pointA;
        else
            Debug.LogError("pointA 未设置");
    }

    void Update()
    {
        if (currentTarget == null) return;

        // 1. 转向目标点
        Vector3 dir = (currentTarget.position - transform.position).normalized;
        if (dir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
        }

        // 2. 移动
        transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, moveSpeed * Time.deltaTime);

        // 3. 到达检测
        if (!hasTriggeredEnd && Vector3.Distance(transform.position, currentTarget.position) < 0.1f)
        {
            // 触发结束动画
            animator.SetTrigger("toEnd");
            hasTriggeredEnd = true;

            // 切换目标点
            currentTarget = (currentTarget == pointA) ? pointB : pointA;

            // 注意：这里不立即重置 hasTriggeredEnd，因为要等 End 动画播放完才能重新开始移动？
            // 但我们的需求是不停顿，所以应该允许角色在 End 动画播放期间就开始转向移动？
            // 实际上，我们希望角色到达后立即转身走向下一个点，同时播放 End 动画。
            // 因此，我们不需要等待，直接切换目标并允许移动。
            // 但是，如果不加延迟，移动会立即开始，而 End 动画还在播放，看起来可能有点怪。
            // 为了效果更好，可以等待一小段时间（例如 0.2 秒）再重置标志，让角色在原地完成 End 动画的末尾部分。
            // 根据你的描述，之前的问题是动画缺失，所以这里我们简单处理：不等待，直接允许移动。
            // 如果你希望角色在转身时有停顿，可以取消注释下面的协程。
            // 先试试简单版本，不行再加延迟。

            // 为了确保 toEnd 不会因为同一位置再次触发，我们延迟一帧再重置标志
            Invoke(nameof(ResetTriggerFlag), 0.2f); // 0.2秒后重置，避免连续触发
        }
    }

    private void ResetTriggerFlag()
    {
        hasTriggeredEnd = false;
    }
}