using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour, InteractiveObject
{
    Move         mover;
    InfoWeapon   weaponInfo;

    float        liveTimer = 0f;
    float        attackRateTimer = 0f;
    int          weaponLevel;

    System.Action destroyCallback = null;
    public System.Action DestroyCallback { set { destroyCallback = value; } }

    //총알 이동 방식 설정.
    public bool Init(InfoWeapon info, int level)
    {
        switch(info.moveType)
        {
            case Def.DefEnum.MoveType.LINEAR:
                mover = new LinearMove(transform, info.speed);
                break;
            case Def.DefEnum.MoveType.GUIDED:
                {
                    var destEnemy = BattleManager.Instance.SearchNearEnemy(transform, info.range);
                    var dest = destEnemy != null ? destEnemy.transform : null;
                    mover = new GuidedMove(transform, dest, info.speed, info.range);
                }
                break;
        }
        weaponInfo = info;
        weaponLevel = level;
        Refresh();

        return weaponInfo.speed == 0 && weaponInfo.lifeTime > 0f;
    }

    public void Refresh()
    {
        liveTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (BattleManager.Instance.GameState == Def.DefEnum.GameState.PLAY)
        {
            mover.Moving();

            //일정 시간 후 삭제.
            if (liveTimer >= GetLifeTime())
            {
                Destroy();
            }
            else
            {
                liveTimer += Time.deltaTime;
            }

            //총알이 콜라이더를 가지고 있지 않으면 일정 시간 후 범위 내 적 검사.
            if (!weaponInfo.haveCollider && (attackRateTimer >= 0.2f))
            {
                CheckInRange();
                attackRateTimer = 0;
            }
            else
            {
                attackRateTimer += Time.deltaTime;
            }
        }
        else if (BattleManager.Instance.GameState == Def.DefEnum.GameState.END)
        {
            //게임 종료되면 탄환 삭제.
            //Destroy(gameObject);
            //ResourceManager.Instance.DestroyObject(weaponInfo.bulletPath, gameObject);
            gameObject.SetActive(false);
        }
    }

    //충돌 처리.
    private void OnTriggerEnter(Collider other)
    {
        Destroy();
        if (weaponInfo.attackType == Def.DefEnum.AttackType.SINGLE)
        {
            InteractiveObject obj = other.GetComponent<InteractiveObject>();
            if (obj != null)
            {
                obj.Attacked(GetAttackValue());
                SoundManager.Instance.PlaySound(weaponInfo.hitSFXPath);
            }
        }
    }

    //범위 내 적 공격.
    void CheckInRange()
    {
        if(weaponInfo.attackType == Def.DefEnum.AttackType.ANGLE_RANGE)
        {
            BattleManager.Instance.AngleRangeAttack(transform, weaponInfo.length, weaponInfo.range, GetAttackValue());
        }
        else if(weaponInfo.attackType == Def.DefEnum.AttackType.FRONT_RANGE)
        {
            //범위 공격에 맞은 적이 있으면 사운드 출력.
            if(BattleManager.Instance.FrontRangeAttack(transform, weaponInfo.range, weaponInfo.length, GetAttackValue()))
            {
                SoundManager.Instance.PlaySound(weaponInfo.hitSFXPath);
            }
        }
    }

    public void Attacked(int atk)
    {
    }

    //공격력 계산.
    int GetAttackValue()
    {
        //반올림 계산이 5까지 버려서 5는 올리기 위해 0.1 추가.
        return weaponInfo.attack + Mathf.RoundToInt((weaponInfo.attackUpg + 0.1f) * weaponLevel);
    }

    //유지 시간 계산.
    float GetLifeTime()
    {
        float lifeTime = weaponInfo.lifeTime + weaponInfo.lifeTimeUpg * weaponLevel;
        return lifeTime <= weaponInfo.lifeTimeMax ? lifeTime : weaponInfo.lifeTimeMax;
    }

    //자신 삭제.
    public void Destroy()
    {
        if(destroyCallback != null)
        {
            destroyCallback();
        }

        if(weaponInfo.effectPath != "")
        {
            EffectManager.Instance.LoadEffect(weaponInfo.effectPath, transform.position, transform.rotation);
        }

        if(weaponInfo.attackType == Def.DefEnum.AttackType.RANGE_EXPLOSION)
        {
            //범위 공격에 맞은 적이 있으면 사운드 출력.
            if(BattleManager.Instance.CircleRangeAttack(transform, weaponInfo.range, GetAttackValue()))
            {
                SoundManager.Instance.PlaySound(weaponInfo.hitSFXPath);
            }
        }

        //Destroy(gameObject);
        //ResourceManager.Instance.DestroyObject(weaponInfo.bulletPath, gameObject);
        //gameObject.SetActive(false);
        ResourceManager.Instance.ReleaseResource(this.gameObject);
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(transform.position, weaponInfo.Range / 2f);
    //}
}
