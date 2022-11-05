using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 所有单位的基类
/// </summary>
public class ObjectBase : MonoBehaviour
{
    [SerializeField] float hp;
    [SerializeField] AudioSource audioSource;
    [SerializeField] List<AudioClip> audioClips;
    public GameObject lootObject;   // 掉落的物品

    // HP的属性，核心价值是 当hp修改时自动判断死亡/hp更新逻辑
    public float Hp
    {
        get => hp;
        set
        {
            hp = value;
            // 检测死亡
            if (hp <= 0)
            {
                hp = 0;
                Dead();
            }
            // 调用HP更新逻辑
            OnHpUpdate();
        }
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    public void PlayAudio(int index)
    {
        audioSource.PlayOneShot(audioClips[index]);
    }
    /// <summary>
    /// 当HP变化时自动调用
    /// </summary>
    protected virtual void OnHpUpdate() { }
    /// <summary>
    /// 死亡
    /// </summary>
    protected virtual void Dead()
    {
        if (lootObject != null)
        {
            Instantiate(lootObject,
            transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(1f, 1.5f), Random.Range(-0.5f, 0.5f)),
            Quaternion.identity,
            null);
        }
    }
    /// <summary>
    /// 受伤
    /// </summary>
    /// <param name="damage"></param>
    public virtual void Hurt(int damage)
    {
        Hp -= damage;
    }
    /// <summary>
    /// 添加物品
    /// </summary>
    public virtual bool AddItem(ItemType itemType)
    {
        return false;
    }
}
