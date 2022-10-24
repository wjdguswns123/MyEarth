using UnityEngine;
using System.Collections;
using Def;

public class SubWeapon : MonoBehaviour
{
    public Transform[] firePosition = new Transform[1];

    InfoWeapon         subWeaponInfo;
    Bullet             lunchingBullet;

    int                weaponLevel;
    int                loadBulletCount = 0;
    int                maxLoadedCount = 10;
    float              loadTimer = 0f;
    float              coolTimer = 0f;
    float              doubleTouchTimer = 0f;
    bool               isLunching = false;
    bool               isFireReady = false;
    bool               isDoubleTouchEnable = false;

    //무기 정보 설정.
    public void Init(InfoWeapon subInfo)
    {
        subWeaponInfo       = subInfo;
        lunchingBullet      = null;
        isFireReady         = true;
        isDoubleTouchEnable = false;
        loadBulletCount     = 0;
        maxLoadedCount      = System.Convert.ToInt32(InfoManager.Instance.infoGlobalList["MaxSubWeaponLoaded"].value);
        BattleManager.Instance.ingameUI.SetSubWeaponLoadedCnt(loadBulletCount);
    }

    // Update is called once per frame
    void Update ()
    {
        if(BattleManager.Instance.GameState == Def.DefEnum.GameState.PLAY)
        {
            UpdateCoolTime();
            UpdateFireCoolTime();
            UpdateDblTouchTime();

            FireByDblTouch();
        }
	}

    //무기 업그레이드 처리.
    public void UpgradeLevel(int level)
    {
        weaponLevel = level;
    }

    //탄환 발사.
    void FireBullet()
    {
        if(isFireReady && loadBulletCount > 0)
        {
            if(isLunching)
            {
                lunchingBullet.Refresh();
            }
            else
            {
                for (int i = 0; i < subWeaponInfo.count; ++i)
                {
                    //발사 속도가 0이면 발사 지점 아래에 생성, 0보다 크면 부모 객체 없이 생성.
                    Transform firePos = firePosition[(firePosition.Length > i ? i : i % firePosition.Length)];
                    Bullet bul = subWeaponInfo.speed > 0 ?
                        ResourceManager.Instance.LoadResource(ResourcePath.WEAPON_PATH, subWeaponInfo.bulletPath, firePos.position, firePos.rotation).GetComponent<Bullet>() :
                        ResourceManager.Instance.LoadResource(ResourcePath.WEAPON_PATH, subWeaponInfo.bulletPath, firePos).GetComponent<Bullet>();
                    bul.Init(subWeaponInfo, weaponLevel);
                    if (isLunching)
                    {
                        lunchingBullet = bul;
                        bul.DestroyCallback = EndFire;
                    }
                }
            }

            isFireReady = false;
            coolTimer = 0f;
            loadBulletCount--;
            SoundManager.Instance.PlaySound(subWeaponInfo.fireSFXPath);
            BattleManager.Instance.ingameUI.SetSubWeaponLoadedCnt(loadBulletCount);
        }
    }

    //쿨타임 타이머 업데이트.
    void UpdateCoolTime()
    {
        //현재 장전된 탄환 갯수가 최대 장전 갯수 이상이면 실행하지 않는다.
        if(loadBulletCount >= maxLoadedCount)
        {
            return;
        }

        //장전 타이머 갱신.
        float loadTime = GetLoadTime();
        if (loadTimer >= loadTime)
        {
            loadBulletCount++;
            loadTimer = 0f;
            BattleManager.Instance.ingameUI.SetSubWeaponLoadedCnt(loadBulletCount);
        }
        else
        {
            //포대에 붙어서 발사하는 무기는 발사중이 아닐 때 발사 타이머 작동.
            loadTimer += Time.deltaTime;
            BattleManager.Instance.ingameUI.SetSubWeaponLoadCoolTimer(loadTime, loadTimer);
        }
    }

    //발사 지연 타이머 갱신.
    void UpdateFireCoolTime()
    {
        if (!isFireReady && coolTimer >= subWeaponInfo.coolTime)
        {
            isFireReady = true;
        }
        else if (!isFireReady)
        {
            coolTimer += Time.deltaTime;
            BattleManager.Instance.ingameUI.SetSubWeaponFireCoolTimer(subWeaponInfo.coolTime, coolTimer);
        }
    }

    //더블 터치 타이머 갱신.
    void UpdateDblTouchTime()
    {
        if(isDoubleTouchEnable)
        {
            if(doubleTouchTimer >= 0.2f)
            {
                isDoubleTouchEnable = false;
                doubleTouchTimer = 0f;
            }
            else
            {
                doubleTouchTimer += Time.deltaTime;
            }
        }
    }

    //더블 터치로 인한 탄환 발사.
    void FireByDblTouch()
    {
        if(GameManager.Instance.IsEnableDlbTouchFire && Input.GetMouseButtonDown(0))
        {
            if(isDoubleTouchEnable)
            {
                Fire();
                doubleTouchTimer = 0f;
                isDoubleTouchEnable = false;
            }
            else
            {
                isDoubleTouchEnable = true;
            }
        }
    }

    //장전 간격 계산.
    float GetLoadTime()
    {
        float loadTime = subWeaponInfo.loadTime - (subWeaponInfo.loadTimeUpg * weaponLevel);
        return loadTime >= subWeaponInfo.loadTimeMax ? loadTime : subWeaponInfo.loadTimeMax;
    }
    
    //발사 종료 콜백 함수.
    void EndFire()
    {
        isLunching = false;
        lunchingBullet = null;
    }

    ////삭제될 때 리소스 매니저의 리스트에서 해당 무기의 탄환 오브젝트 해제 요청.
    //private void OnDestroy()
    //{
    //    ResourceManager.Instance.DeleteResource(subWeaponInfo.bulletPath);
    //}

    //외부에서 받아오는 발사 처리.
    public void Fire()
    {
        FireBullet();
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawLine(transform.position, transform.position + transform.up * subWeaponInfo.Length);
    //    Gizmos.DrawLine(transform.position, transform.position + transform.right * (subWeaponInfo.Range / 2f));
    //}
}
