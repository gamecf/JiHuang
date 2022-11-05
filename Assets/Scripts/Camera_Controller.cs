using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 摄像机控制
/// </summary>
public class Camera_Controller : MonoBehaviour
{
    [SerializeField] Transform target;  // 要跟随的目标
    [SerializeField] Vector3 offset;    // 目标偏移量
    [SerializeField] float transitionSpeed = 2;  // 过渡的速度
    // 因为玩家的移动在Update中执行，那理论上最好的情况是玩家先移动，相机再去追
    private void LateUpdate()
    {
        if (target != null)
        {
            Vector3 targetPos = target.position + offset;
            // 从当前坐标 平滑 过渡到 目标点
            transform.position = Vector3.Lerp(transform.position, targetPos, transitionSpeed * Time.deltaTime);
        }
    }
}
