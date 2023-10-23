using UnityEngine;
using System.Collections;
using Def;

public class Enemy : MonoBehaviour
{
    private const float ATTACK_POINT_GAP = 0.05f;

    public enum MoveState { NORMAL, NEAR_PLANET, NO_DESTINATION }

    #region Inspector

    public Transform firePosition;

    #endregion

    private AttackPoint _destAttackPoint;
    private Planet _planet;
    private Move[] _mover;
    private MoveState _moveState = MoveState.NORMAL;
    private InfoEnemy _enemyInfo;
    private GameObject _attackEffect;

    private float _fireTimer;
    private float _moveSpeed;
    private float _attackSpeed;
    private int _hp;
    private int _attack;
    private int _level = 0;
    private bool _playedSound = false;

    private Collider _collider;

    public DefEnum.EnemyState State { get; private set; }

    private void Start()
    {
        if(_mover == null)
        {
            _mover = new Move[3];
        }

        //일반 이동, 행성 근접했을 때, 목적지 없을 때 이동 클래스 설정.
        _mover[(int)MoveState.NORMAL] = new LinearMove(transform, _moveSpeed);
        _mover[(int)MoveState.NEAR_PLANET] = new RotateMove(transform, _planet.planetBody, _moveSpeed);
        _mover[(int)MoveState.NO_DESTINATION] = new RotateMove(transform, _planet.planetBody, _moveSpeed);
        State = DefEnum.EnemyState.MOVE;
    }

    // Update is called once per frame
    private void Update ()
    {
        if(BattleManager.Instance.GameState == DefEnum.GameState.PLAY)
        {
            switch(State)
            {
                case DefEnum.EnemyState.MOVE:
                    _mover[(int)_moveState].Moving();
                    switch (_moveState)
                    {
                        case MoveState.NORMAL:
                            if (!CheckArriveAttackPoint())
                            {
                                // 적이 행성 공격을 할 수 있는 반경에 도달했으면, 이동 형태 변경.
                                float dist = Vector3.SqrMagnitude(transform.position - _planet.transform.position);
                                if (dist <= _planet.enemyAttackRadius * _planet.enemyAttackRadius)
                                {
                                    _moveState = MoveState.NEAR_PLANET;
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
                    if (_fireTimer >= _attackSpeed)
                    {
                        _planet.Attacked(_attack);
                        _fireTimer = 0f;
                        if(!_playedSound)
                        {
                            SoundManager.Instance.PlaySound("FireSFX_5");
                            _playedSound = true;
                        }
                    }
                    else
                    {
                        _fireTimer += Time.deltaTime;
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// 플레이어 트랜스폼 받아 초기화.
    /// </summary>
    /// <param name="info"></param>
    public void Init(InfoEnemy info)
    {
        BattleManager.Instance.AddEnemy(this);

        this._planet = BattleManager.Instance.planet;
        _destAttackPoint = BattleManager.Instance.SearchAttackPoint(this);
        _level       = DataManager.Instance.enemyLevelDataList[info.ID].level;
        _hp          = info.HP + (info.HPUpg * (_level - 1));
        _moveSpeed   = info.speed;
        _attackSpeed = info.attackSpeed;
        _attack      = info.attack + (info.attackUpg * (_level - 1));
        _enemyInfo   = info;
        _fireTimer   = 0f;
        State = DefEnum.EnemyState.MOVE;

        _collider = GetComponent<Collider>();
        _collider.enabled = true;
        if (_destAttackPoint != null)
        {
            _moveState = MoveState.NORMAL;
            SetDirection(_destAttackPoint);
        }
        else
        {
            _moveState = MoveState.NO_DESTINATION;
        }
    }

    /// <summary>
    /// 이동 방향 설정.
    /// </summary>
    /// <param name="atkPoint"></param>
    private void SetDirection(AttackPoint atkPoint)
    {
        if(atkPoint != null)
        {
            //플레이어 행성을 향하도록.
            transform.rotation = Quaternion.LookRotation((atkPoint.transform.position - transform.position), Vector3.forward);
        }
    }

    /// <summary>
    /// 피격 시 처리.
    /// </summary>
    /// <param name="atk"></param>
    public void Attacked(int atk)
    {
        if(_hp > 0)
        {
            _hp -= atk;
            EffectManager.Instance.LoadEffect("Bang_03", transform.position, transform.rotation);

            if (_hp <= 0)
            {
                Destroy();
                BattleManager.Instance.CheckFinishWave();
            }
        }
    }

    /// <summary>
    /// 자신 삭제. 배틀매니저에 삭제 요청.
    /// </summary>
    public void Destroy()
    {
        SoundManager.Instance.PlaySound(_enemyInfo.destroySFXPath);
        BattleManager.Instance.RemoveEnemy(this);

        _collider.enabled = false;
        BattleManager.Instance.AddScore(_enemyInfo.score);
        BattleManager.Instance.AddResources(_enemyInfo.resource + (_enemyInfo.resourceUpg * (_level - 1)));
        EffectManager.Instance.LoadEffect("Bang_05", transform.position, Quaternion.identity);
        ResourceManager.Instance.ReleaseResource(gameObject);

        if (_attackEffect != null)
        {
            _attackEffect.SetActive(false);
            _attackEffect = null;
        }
    }

    /// <summary>
    /// 목적지에 이미 다른 적이 도착했으면 새로운 목적지 탐색.
    /// </summary>
    /// <param name="point"></param>
    public void FullAttackPoint(AttackPoint point)
    {
        if(State == DefEnum.EnemyState.MOVE && _destAttackPoint == point)
        {
            _destAttackPoint = BattleManager.Instance.SearchAttackPoint(this);
            if(_destAttackPoint != null)
            {
                SetDirection(_destAttackPoint);
            }
            else
            {
                _moveState = MoveState.NO_DESTINATION;
            }
        }
    }

    /// <summary>
    /// 해당 지점으로 목적지 설정.
    /// </summary>
    /// <param name="point"></param>
    public void ResetAttackPoint(AttackPoint point)
    {
        _destAttackPoint = point;
        SetDirection(_destAttackPoint);
        _moveState = MoveState.NORMAL;
    }

    /// <summary>
    /// 목적지 도달 했는 지 확인.
    /// </summary>
    /// <returns></returns>
    private bool CheckArriveAttackPoint()
    {
        //목적지에 도달했을 때.
        float dist = Vector3.SqrMagnitude(transform.position - _destAttackPoint.transform.position);
        if (dist <= ATTACK_POINT_GAP * ATTACK_POINT_GAP)
        {
            State = DefEnum.EnemyState.ATTACK;
            transform.position = _destAttackPoint.transform.position;
            transform.rotation = Quaternion.LookRotation((_planet.planetBody.transform.position - transform.position), Vector3.forward);
            BattleManager.Instance.ArrivalAttackPoint(this, _destAttackPoint);
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
