using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// 敌人AI状态
/// </summary>
public enum EnemyState
{
    Idle,       // 待机
    Move,       // 移动
    Pursue,     // 追击
    Attack,     // 攻击
    Hurt,       // 受伤
    Die         // 死亡
}

/// <summary>
/// 野猪AI
/// </summary>
public class Pig_Controller : ObjectBase
{
    [SerializeField] Animator animator;
    [SerializeField] NavMeshAgent navMeshAgent;
    [SerializeField] CheckCollider checkCollider;

    // 行动范围
    public float maxX = 4.74f;
    public float minX = -5.62f;
    public float maxZ = 5.92f;
    public float minZ = -6.33f;

    private EnemyState enemyState;

    private Vector3 targetPos;

    // 当状态修改时，进入新状态要做得拾取
    // 相当于OnEnter
    public EnemyState EnemyState
    {
        get => enemyState;
        set
        {
            enemyState = value;
            switch (enemyState)
            {
                case EnemyState.Idle:
                    // 播放动画
                    // 关闭导航
                    // 休息一段时间之后去巡逻
                    animator.CrossFadeInFixedTime("Idle", 0.25f);
                    navMeshAgent.enabled = false;
                    Invoke(nameof(GoMove), Random.Range(3f, 10f));
                    break;
                case EnemyState.Move:
                    // 播放动画
                    // 开启导航
                    // 获取巡逻点
                    // 移动到指定目标位置
                    animator.CrossFadeInFixedTime("Move", 0.25f);
                    navMeshAgent.enabled = true;
                    targetPos = GetTargetPos();
                    navMeshAgent.SetDestination(targetPos);
                    break;
                case EnemyState.Pursue:
                    animator.CrossFadeInFixedTime("Move", 0.25f);
                    navMeshAgent.enabled = true;
                    break;
                case EnemyState.Attack:
                    animator.CrossFadeInFixedTime("Attack", 0.25f);
                    transform.LookAt(Player_Controller.Instance.transform.position);
                    navMeshAgent.enabled = false;
                    break;
                case EnemyState.Hurt:
                    animator.CrossFadeInFixedTime("Hurt", 0.25f);
                    PlayAudio(0);
                    navMeshAgent.enabled = false;
                    break;
                case EnemyState.Die:
                    PlayAudio(0);
                    navMeshAgent.enabled = false;
                    animator.CrossFadeInFixedTime("Die", 0.25f);
                    break;
            }
        }
    }

    private void Start()
    {
        checkCollider.Init(this, 10);
        EnemyState = EnemyState.Idle;
    }
    private void Update()
    {
        StateOnUpdate();
    }
    private void StateOnUpdate()
    {
        switch (enemyState)
        {
            case EnemyState.Move:
                if (Vector3.Distance(transform.position, targetPos) < 1.5f)
                {
                    EnemyState = EnemyState.Idle;
                }
                break;
            case EnemyState.Pursue:
                // 追击到足够近，切换到攻击状态
                if (Vector3.Distance(transform.position, Player_Controller.Instance.transform.position) < 1)
                {
                    EnemyState = EnemyState.Attack;
                }
                else
                {
                    // 距离遥远，继续追
                    navMeshAgent.SetDestination(Player_Controller.Instance.transform.position);
                }
                break;
            case EnemyState.Attack:
                if (Player_Controller.Instance.Hp <= 0) EnemyState = EnemyState.Idle;
                break;
        }
    }

    private void GoMove()
    {
        EnemyState = EnemyState.Move;
    }
    /// <summary>
    /// 获取一个范围内的随机点
    /// </summary>
    /// <returns></returns>
    private Vector3 GetTargetPos()
    {
        return new Vector3(Random.Range(minX, maxX), 0, Random.Range(minZ, maxZ));
    }
    public override void Hurt(int damage)
    {
        if (EnemyState == EnemyState.Die) return;
        CancelInvoke(nameof(GoMove));   //取消切换到移动状态的延时调用
        base.Hurt(damage);
        if (Hp > 0)    // 没有死亡，切换到受伤
        {
            EnemyState = EnemyState.Hurt;
        }
    }
    protected override void Dead()
    {
        base.Dead();
        EnemyState = EnemyState.Die;
    }
    #region 动画事件
    private void StartHit()
    {
        checkCollider.StartHit();
    }
    private void StopHit()
    {
        checkCollider.StopHit();
        if (EnemyState != EnemyState.Die) EnemyState = EnemyState.Pursue;
    }
    private void StopAttack()
    {
        if (EnemyState != EnemyState.Die) EnemyState = EnemyState.Pursue;
    }

    private void HurtOver()
    {
        if (EnemyState != EnemyState.Die) EnemyState = EnemyState.Pursue;
    }
    private void Die()
    {
        Destroy(gameObject);
    }
    #endregion
}

