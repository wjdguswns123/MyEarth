using UnityEngine;
using System.Collections;
using Def;

public class Enemy : MonoBehaviour, InteractiveObject
{
    enum MoveState { NORMAL, NEAR_PLANET, NO_DESTINATION }

    public Transform          firePosition;

    AttackPoint               destAttackPoint;
    Planet                    planet;
    Move[]                    mover = new Move[3];
    DefEnum.EnemyState        state;
    public DefEnum.EnemyState State { get { return state; } }
    MoveState                 moveState = MoveState.NORMAL;
    float                     fireTimer = 0f;
    float                     speed;
    float                     attackSpeed;
    int                       HP  = 1;
    int                       attack;
    int                       level = 0;
    bool                      playedSound = false;
    InfoEnemy                 enemyInfo;

    private GameObject _attackEffect;

    private void Start()
    {
        //일반 이동, 행성 근접했을 때, 목적지 없을 때 이동 클래스 설정.
        mover[(int)MoveState.NORMAL] = new LinearMove(transform, speed);
        mover[(int)MoveState.NEAR_PLANET] = new RotateMove(transform, planet.planetBody, speed);
        mover[(int)MoveState.NO_DESTINATION] = new RotateMove(transform, planet.planetBody, speed);
        state = DefEnum.EnemyState.MOVE;
    }

    // Update is called once per frame
    void Update ()
    {
        if(BattleManager.Instance.GameState == DefEnum.GameState.PLAY)
        {
            switch(state)
            {
                case DefEnum.EnemyState.MOVE:
                    mover[(int)moveState].Moving();
                    switch (moveState)
                    {
                        case MoveState.NORMAL:
                            if (!CheckArriveAttackPoint())
                            {
                                if (Vector3.Distance(transform.position, planet.transform.position) <= planet.notInEnemyAreaRadius)
                                {
                                    moveState = MoveState.NEAR_PLANET;
                                }
                            }
                            break;
                        case MoveState.NEAR_PLANET:
                            CheckArriveAttackPoint();
                            break;
                        case MoveState.NO_DESTINATION:
                            break;
                    }
                    break;
                case DefEnum.EnemyState.ATTACK:
                    //일정 시간 후 삭제.
                    if (fireTimer >= attackSpeed)
                    {
                        planet.Attacked(attack);
                        fireTimer = 0f;
                        if(!playedSound)
                        {
                            SoundManager.Instance.PlaySound("FireSFX_5");
                            playedSound = true;
                        }
                    }
                    else
                    {
                        fireTimer += Time.deltaTime;
                    }
                    break;
            }
        }
    }

    //플레이어 트랜스폼 받아 초기화.
    public void Init(InfoEnemy info)
    {
        BattleManager.Instance.AddEnemy(this);

        this.planet = BattleManager.Instance.planet;
        destAttackPoint = BattleManager.Instance.SearchAttackPoint(this);
        level        = DataManager.Instance.enemyLevelDataList[info.ID].Level;
        HP           = info.HP + (info.HPUpg * (level - 1));
        speed        = info.speed;
        attackSpeed  = info.attackSpeed;
        attack       = info.attack + (info.attackUpg * (level - 1));
        enemyInfo    = info;
        state = DefEnum.EnemyState.MOVE;
        GetComponent<Collider>().enabled = true;
        if (destAttackPoint != null)
        {
            moveState = MoveState.NORMAL;
            SetDirection(destAttackPoint);
        }
        else
        {
            moveState = MoveState.NO_DESTINATION;
        }
    }

    //이동 방향 설정.
    void SetDirection(AttackPoint atkPoint)
    {
        if(atkPoint != null)
        {
            //플레이어 행성을 향하도록.
            transform.rotation = Quaternion.LookRotation((atkPoint.transform.position - transform.position), Vector3.forward);
        }
    }

    //피격 시 처리.
    public void Attacked(int atk)
    {
        if(HP > 0)
        {
            HP -= atk;
            EffectManager.Instance.LoadEffect("Bang_03", transform.position, transform.rotation);

            if (HP <= 0)
            {
                Destroy();
                BattleManager.Instance.CheckFinishWave();
            }
        }
    }

    //자신 삭제. 배틀매니저에 삭제 요청.
    public void Destroy()
    {
        SoundManager.Instance.PlaySound(enemyInfo.destroySFXPath);
        BattleManager.Instance.RemoveEnemy(this);
        DestroyOnly();
    }

    //자신 삭제.
    public void DestroyOnly()
    {
        GetComponent<Collider>().enabled = false;
        BattleManager.Instance.AddScore(enemyInfo.score);
        BattleManager.Instance.AddResources(enemyInfo.resource + (enemyInfo.resourceUpg * (level - 1)));
        EffectManager.Instance.LoadEffect("Bang_05", transform.position, Quaternion.identity);
        if(firePosition.childCount > 0)
        {
            firePosition.DestroyChildren();
        }
        //Destroy(gameObject);
        //ResourceManager.Instance.DestroyObject(enemyInfo.prefabPath, gameObject);
        gameObject.SetActive(false);

        if(_attackEffect != null)
        {
            _attackEffect.SetActive(false);
            _attackEffect = null;
        }
    }

    //목적지에 이미 다른 적이 도착했으면 새로운 목적지 탐색.
    public void FullDestinationAttackPoint(AttackPoint point)
    {
        if(state == DefEnum.EnemyState.MOVE && destAttackPoint == point)
        {
            destAttackPoint = BattleManager.Instance.SearchAttackPoint(this);
            if(destAttackPoint != null)
            {
                SetDirection(destAttackPoint);
            }
            else
            {
                moveState = MoveState.NO_DESTINATION;
            }
        }
    }

    //해당 지점으로 목적지 설정.
    public void ResetDestinationAttackPoint(AttackPoint point)
    {
        destAttackPoint = point;
        SetDirection(destAttackPoint);
        moveState = MoveState.NORMAL;
    }

    //목적지 도달 했는 지 확인.
    bool CheckArriveAttackPoint()
    {
        //목적지에 도달했을 때.
        if (Vector3.Distance(transform.position, destAttackPoint.transform.position) <= 0.05f)
        {
            transform.position = destAttackPoint.transform.position;
            state = DefEnum.EnemyState.ATTACK;
            transform.rotation = Quaternion.LookRotation((planet.planetBody.transform.position - transform.position), Vector3.forward);
            BattleManager.Instance.ArrivalAttackPoint(this, destAttackPoint);
            _attackEffect = EffectManager.Instance.LoadEffect("Attack_01", firePosition.position, firePosition.rotation);

            return true;
        }

        return false;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = HP > 2 ? Color.green : (HP > 1 ? Color.yellow : (HP > 0 ? Color.red : Color.gray));
    //    Gizmos.DrawCube(transform.position, Vector3.one * 0.3f);
    //}
}
