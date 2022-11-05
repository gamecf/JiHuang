using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 转门用来检测的 碰撞物体
/// </summary>
public class CheckCollider : MonoBehaviour
{
    private ObjectBase owner;
    private int damage;
    private bool canHit = false;
    // 敌人的标签
    [SerializeField] List<string> enemyTags = new List<string>();
    // 可以拾取的标签
    [SerializeField] List<string> itemTags = new List<string>();

    public void Init(ObjectBase owner, int damage)
    {
        this.damage = damage;
        this.owner = owner;
    }

    /// <summary>
    /// 开启伤害检测
    /// </summary>
    public void StartHit()
    {
        canHit = true;
    }
    /// <summary>
    /// 关闭伤害检测
    /// </summary>
    public void StopHit()
    {
        canHit = false;
        lastAttackObjectList.Clear();
    }
    private List<GameObject> lastAttackObjectList = new List<GameObject>();
    private void OnTriggerStay(Collider other)
    {
        // 如果当前允许伤害检测
        if (canHit)
        {
            // 此次伤害还没有检测过这个单位 && 敌人的标签包含在敌人列表中
            if (!lastAttackObjectList.Contains(other.gameObject) && enemyTags.Contains(other.tag))
            {
                lastAttackObjectList.Add(other.gameObject);
                // 具体的伤害逻辑
                other.GetComponent<ObjectBase>().Hurt(damage);
            }
            return;
        }
        // 检测拾取
        if (itemTags.Contains(other.tag))
        {
            // 把捡到的东西 tag 转枚举
            ItemType itemType = System.Enum.Parse<ItemType>(other.tag);
            if (owner.AddItem(itemType))
            {
                owner.PlayAudio(1);         // 告诉宿主播放音效
                Destroy(other.gameObject);  // 销毁捡到的物品
            }
        }
    }
}
