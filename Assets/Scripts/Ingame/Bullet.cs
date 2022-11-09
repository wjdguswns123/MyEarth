using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour, InteractiveObject
{
    private Move _mover;
    private InfoWeapon _weaponInfo;

    private float _liveTimer;
    private float _attackRateTimer;
    private int _weaponLevel;

    /// <summary>
    /// 총알 이동 방식 설정.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public void Init(InfoWeapon info, int level)
    {
        switch(info.moveType)
        {
            case Def.DefEnum.MoveType.LINEAR:
                _mover = new LinearMove(transform, info.speed);
                break;
            case Def.DefEnum.MoveType.GUIDED:
                {
                    var destEnemy = BattleManager.Instance.SearchNearEnemy(transform, info.range);
                    var dest = destEnemy != null ? destEnemy.transform : null;
                    _mover = new GuidedMove(transform, dest, info.speed, info.range);
                }
                break;
        }
        _weaponInfo = info;
        _weaponLevel = level;

        _liveTimer = 0f;
        _attackRateTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (BattleManager.Instance.GameState == Def.DefEnum.GameState.PLAY)
        {
            _mover.Moving();

            //일정 시간 후 삭제.
            if (_liveTimer >= GetLifeTime())
            {
                Destroy();
            }
            else
            {
                _liveTimer += Time.deltaTime;
            }

            //총알이 콜라이더를 가지고 있지 않으면 일정 시간 후 범위 내 적 검사.
            if (!_weaponInfo.haveCollider && (_attackRateTimer >= 0.2f))
            {
                CheckInRange();
                _attackRateTimer = 0;
            }
            else
            {
                _attackRateTimer += Time.deltaTime;
            }
        }
    }

    /// <summary>
    /// 유지 시간 계산.
    /// </summary>
    /// <returns></returns>
    private float GetLifeTime()
    {
        float lifeTime = _weaponInfo.lifeTime + _weaponInfo.lifeTimeUpg * _weaponLevel;
        return lifeTime <= _weaponInfo.lifeTimeMax ? lifeTime : _weaponInfo.lifeTimeMax;
    }

    /// <summary>
    /// 범위 내 적 공격.
    /// </summary>
    private void CheckInRange()
    {
        if (_weaponInfo.attackType == Def.DefEnum.AttackType.ANGLE_RANGE)
        {
            BattleManager.Instance.AngleRangeAttack(transform, _weaponInfo.length, _weaponInfo.range, GetAttackValue());
        }
        else if (_weaponInfo.attackType == Def.DefEnum.AttackType.FRONT_RANGE)
        {
            //범위 공격에 맞은 적이 있으면 사운드 출력.
            if (BattleManager.Instance.FrontRangeAttack(transform, _weaponInfo.range, _weaponInfo.length, GetAttackValue()))
            {
                SoundManager.Instance.PlaySound(_weaponInfo.hitSFXPath);
            }
        }
    }

    /// <summary>
    /// 충돌 처리.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        Destroy();
        if (_weaponInfo.attackType == Def.DefEnum.AttackType.SINGLE)
        {
            InteractiveObject obj = other.GetComponent<InteractiveObject>();
            if (obj != null)
            {
                obj.Attacked(GetAttackValue());
                SoundManager.Instance.PlaySound(_weaponInfo.hitSFXPath);
            }
        }
    }

    /// <summary>
    /// 자신 해제.
    /// </summary>
    public void Destroy()
    {
        if (!_weaponInfo.effectPath.Equals(string.Empty))
        {
            EffectManager.Instance.LoadEffect(_weaponInfo.effectPath, transform.position, transform.rotation);
        }

        if (_weaponInfo.attackType == Def.DefEnum.AttackType.RANGE_EXPLOSION)
        {
            //범위 공격에 맞은 적이 있으면 사운드 출력.
            if (BattleManager.Instance.CircleRangeAttack(transform, _weaponInfo.range, GetAttackValue()))
            {
                SoundManager.Instance.PlaySound(_weaponInfo.hitSFXPath);
            }
        }

        ResourceManager.Instance.ReleaseResource(this.gameObject);
    }

    public void Attacked(int atk)
    {
    }

    /// <summary>
    /// 공격력 계산.
    /// </summary>
    /// <returns></returns>
    private int GetAttackValue()
    {
        //반올림 계산이 5까지 버려서 5는 올리기 위해 0.1 추가.
        return _weaponInfo.attack + Mathf.RoundToInt((_weaponInfo.attackUpg + 0.1f) * _weaponLevel);
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(transform.position, weaponInfo.Range / 2f);
    //}
}
