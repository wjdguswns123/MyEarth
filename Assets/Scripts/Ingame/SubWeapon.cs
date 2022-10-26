using UnityEngine;
using System.Collections;
using Def;

public class SubWeapon : MonoBehaviour
{
    #region Inspector

    public Transform[] firePosition;

    #endregion

    private InfoWeapon _subWeaponInfo;

    private int _weaponLevel;
    private int _loadBulletCount;
    private int _maxLoadedCount;

    private float _loadTimer;
    private float _coolTimer;
    private float _doubleTouchTimer;
    private bool _isFireReady;
    private bool _isDoubleTouchEnable;

    /// <summary>
    /// 무기 정보 설정.
    /// </summary>
    /// <param name="subInfo"></param>
    public void Init(InfoWeapon subInfo)
    {
        _subWeaponInfo = subInfo;

        _isFireReady = true;
        _isDoubleTouchEnable = false;
        _loadBulletCount = 0;
        _loadTimer = 0f;
        _coolTimer = 0f;
        _doubleTouchTimer = 0f;

        _maxLoadedCount = System.Convert.ToInt32(InfoManager.Instance.infoGlobalList["MaxSubWeaponLoaded"].value);
        SetSubWeaponLoadedCountUI();
    }

    /// <summary>
    /// 전략 무기 장전 UI 설정.
    /// </summary>
    private void SetSubWeaponLoadedCountUI()
    {
        BattleManager.Instance.ingameUI.SetSubWeaponLoadedCnt(_loadBulletCount);
    }

    // Update is called once per frame
    void Update ()
    {
        if(BattleManager.Instance.GameState == Def.DefEnum.GameState.PLAY)
        {
            UpdateLoadCoolTime();
            UpdateFireCoolTime();
            UpdateDblTouchTime();

            FireByDblTouch();
        }
	}

    /// <summary>
    /// 전략 무기 장전 쿨타임 타이머 업데이트.
    /// </summary>
    private void UpdateLoadCoolTime()
    {
        //현재 장전된 탄환 갯수가 최대 장전 갯수 이상이면 실행하지 않는다.
        if (_loadBulletCount >= _maxLoadedCount)
        {
            return;
        }

        //장전 타이머 갱신.
        float loadTime = GetLoadTime();
        if (_loadTimer >= loadTime)
        {
            _loadBulletCount++;
            _loadTimer = 0f;
            SetSubWeaponLoadedCountUI();
        }
        else
        {
            //포대에 붙어서 발사하는 무기는 발사중이 아닐 때 발사 타이머 작동.
            _loadTimer += Time.deltaTime;
            BattleManager.Instance.ingameUI.SetSubWeaponLoadCoolTimer(loadTime, _loadTimer);
        }
    }

    /// <summary>
    /// 장전 간격 계산.
    /// </summary>
    /// <returns></returns>
    private float GetLoadTime()
    {
        float loadTime = _subWeaponInfo.loadTime - (_subWeaponInfo.loadTimeUpg * _weaponLevel);
        return loadTime >= _subWeaponInfo.loadTimeMax ? loadTime : _subWeaponInfo.loadTimeMax;
    }

    /// <summary>
    /// 발사 지연 타이머 갱신.
    /// </summary>
    private void UpdateFireCoolTime()
    {
        if (!_isFireReady)
        {
            if(_coolTimer >= _subWeaponInfo.coolTime)
            {
                _isFireReady = true;
            }
            else
            {
                _coolTimer += Time.deltaTime;
                BattleManager.Instance.ingameUI.SetSubWeaponFireCoolTimer(_subWeaponInfo.coolTime, _coolTimer);
            }
        }
    }

    /// <summary>
    /// 더블 터치 타이머.
    /// 터치 후 일정 시간이 지나면 더블 터치가 되지 않게 하는 타이머.
    /// </summary>
    private void UpdateDblTouchTime()
    {
        if (_isDoubleTouchEnable)
        {
            if (_doubleTouchTimer >= 0.2f)
            {
                _isDoubleTouchEnable = false;
                _doubleTouchTimer = 0f;
            }
            else
            {
                _doubleTouchTimer += Time.deltaTime;
            }
        }
    }

    /// <summary>
    /// 더블 터치로 인한 탄환 발사.
    /// </summary>
    private void FireByDblTouch()
    {
        if (GameManager.Instance.IsEnableDlbTouchFire && Input.GetMouseButtonDown(0))
        {
            if (_isDoubleTouchEnable)
            {
                Fire();
                _doubleTouchTimer = 0f;
                _isDoubleTouchEnable = false;
            }
            else
            {
                _isDoubleTouchEnable = true;
            }
        }
    }

    /// <summary>
    /// 탄환 발사.
    /// </summary>
    public void Fire()
    {
        if (_isFireReady && _loadBulletCount > 0)
        {
            for (int i = 0; i < _subWeaponInfo.count; ++i)
            {
                //발사 속도가 0이면 발사 지점 아래에 생성, 0보다 크면 부모 객체 없이 생성.
                Transform firePos = firePosition[(firePosition.Length > i ? i : i % firePosition.Length)];
                Bullet bullet = _subWeaponInfo.speed > 0 ?
                    ResourceManager.Instance.LoadResource(ResourcePath.WEAPON_PATH, _subWeaponInfo.bulletPath, firePos.position, firePos.rotation).GetComponent<Bullet>() :
                    ResourceManager.Instance.LoadResource(ResourcePath.WEAPON_PATH, _subWeaponInfo.bulletPath, firePos).GetComponent<Bullet>();
                bullet.Init(_subWeaponInfo, _weaponLevel);
            }

            _isFireReady = false;
            _coolTimer = 0f;
            _loadBulletCount--;
            SoundManager.Instance.PlaySound(_subWeaponInfo.fireSFXPath);
            SetSubWeaponLoadedCountUI();
        }
    }

    /// <summary>
    /// 전략 무기 레벨 설정.
    /// </summary>
    /// <param name="level"></param>
    public void SetLevel(int level)
    {
        _weaponLevel = level;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawLine(transform.position, transform.position + transform.up * subWeaponInfo.Length);
    //    Gizmos.DrawLine(transform.position, transform.position + transform.right * (subWeaponInfo.Range / 2f));
    //}
}
