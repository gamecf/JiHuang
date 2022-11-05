using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Controller : ObjectBase
{
    public static Player_Controller Instance;
    [SerializeField] CheckCollider checkCollider;
    [SerializeField] Animator animator;
    [SerializeField] CharacterController characterController;
    private Quaternion targetDirQuaternion;
    [SerializeField] float moveSpeed = 1;
    private bool isAttacking = false;
    private bool isHurting = false;
    private bool isDieing = false;
    private float hungry = 100f;
    public bool isDarging = false;  // 当前在拖拽中
    public float Hungry
    {
        get => hungry;
        set
        {
            hungry = value;
            if (hungry <= 0)
            {
                hungry = 0;
                // 衰减HP
                Hp -= Time.deltaTime / 2;
            }
            hungryIamge.fillAmount = hungry / 100;
        }
    }
    [SerializeField] Image hpIamge;
    [SerializeField] Image hungryIamge;
    private void Awake()
    {
        Instance = this;
        // 初始化检测器
        checkCollider.Init(this, 30);
    }
    private void Update()
    {
        UpdateHungry();
        // 即不在攻击中、也不在受伤中，才能移动或发起攻击
        if (!isAttacking && !isHurting && !isDieing)
        {
            Move();
            Attack();
        }
        // 攻击过程中
        else
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetDirQuaternion, Time.deltaTime * 10);
        }
    }

    /// <summary>
    /// 移动
    /// </summary>
    private void Move()
    {
        // 获取玩家的输入
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        // 玩家没有进行移动
        if (h == 0 && v == 0)
        {
            animator.SetBool("Walk", false);
        }
        // 玩家有操作移动
        else
        {
            animator.SetBool("Walk", true);
            // 获取一个最终的目标方向 并过渡过去
            targetDirQuaternion = Quaternion.LookRotation(new Vector3(h, 0, v));
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetDirQuaternion, Time.deltaTime * 10f);
            // 处理实际的移动
            characterController.SimpleMove(new Vector3(h, 0, v).normalized * moveSpeed);
        }
    }

    /// <summary>
    /// 攻击
    /// </summary>
    private void Attack()
    {
        // 检测攻击按键
        if (Input.GetMouseButton(0))
        {
            // 当前在拖拽物品||单前鼠标下在交互UI
            if (isDarging || UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            // 转向到攻击点
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, 100, LayerMask.GetMask("Ground")))
            {
                // 说明碰到地面了
                animator.SetTrigger("Attack");
                // 进入攻击状态
                isAttacking = true;
                // 转向到攻击点
                targetDirQuaternion = Quaternion.LookRotation(hitInfo.point - transform.position);
            }
        }
    }
    /// <summary>
    /// 计算饥饿值
    /// </summary>
    private void UpdateHungry()
    {
        // 衰减饥饿值
        Hungry -= Time.deltaTime * 3;
    }
    public override void Hurt(int damage)
    {
        base.Hurt(damage);
        animator.SetTrigger("Hurt");
        PlayAudio(2);
        isHurting = true;
    }
    protected override void OnHpUpdate()
    {
        // 更新血条
        hpIamge.fillAmount = Hp / 100;
    }
    protected override void Dead()
    {
        if (!isDieing)
        {
            PlayAudio(3);
            animator.SetTrigger("Die");
            isDieing = true;
        }
    }
    public override bool AddItem(ItemType itemType)
    {
        // 检测背包能不能放下
        return UI_BagPanel.Instance.AddItem(itemType);
    }
    /// <summary>
    /// 使用物品
    /// </summary>
    public bool UseItem(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Meat:
                Hp += 40;
                Hungry += 30;
                return true;
            case ItemType.CookedMeat:
                Hp += 60;
                Hungry += 40;
                return true;
            case ItemType.Wood:
                Hp -= 20;
                Hungry += 60;
                return true;
        }
        return false;
    }
    #region 动画事件
    private void StartHit()
    {
        // 播放音效
        PlayAudio(0);
        // 攻击检测
        checkCollider.StartHit();
    }
    private void StopHit()
    {
        // 停止攻击检测
        isAttacking = false;
        checkCollider.StopHit();
    }
    public void HurtOver()
    {
        isHurting = false;
    }
    #endregion
}
