using UnityEngine;
using System.Collections;
using Def;

public class Turret : MonoBehaviour
{
    #region Inspector

    public GameObject firePosition;
    public Transform  subWeaponPosition;

    #endregion

    private InfoWeapon _mainWeaponInfo;

    private SubWeapon _subWeapon;

    private float _mainFireTimer;
    private int _weaponLevel;

    /// <summary>
    /// 기본 무기 정보 설정.
    /// </summary>
    /// <param name="mainInfo"></param>
    public void Init(InfoWeapon mainInfo)
    {
        _mainWeaponInfo = mainInfo;
        _weaponLevel = 0;
        _mainFireTimer = 0f;

        ResourceManager.Instance.PreLoadReaource(ResourcePath.BULLET_PATH, _mainWeaponInfo.bulletPath, initCount: 10);    // 기본 무기 총알 풀 생성.
    }

    /// <summary>
    /// 전략 무기 설정.
    /// </summary>
    /// <param name="subInfo"></param>
    public void SetSubWeapon(InfoWeapon subInfo)
    {
        if (!subInfo.weaponPath.Equals(string.Empty))
        {
            _subWeapon = ResourceManager.Instance.LoadResource(ResourcePath.WEAPON_PATH, subInfo.weaponPath, subWeaponPosition).GetComponent<SubWeapon>();
            _subWeapon.Init(subInfo);
        }
    }

    private void Update()
    {
        if(BattleManager.Instance.GameState == Def.DefEnum.GameState.PLAY)
        {
            //기본 무기 발사. 발사 시간에 맞춰 탄환 발사.
            if (_mainFireTimer >= _mainWeaponInfo.loadTime)
            {
                Bullet bul = ResourceManager.Instance.LoadResource(ResourcePath.WEAPON_PATH, _mainWeaponInfo.bulletPath, firePosition.transform.position, firePosition.transform.rotation).GetComponent<Bullet>();
                bul.Init(_mainWeaponInfo, _weaponLevel);
                SoundManager.Instance.PlaySound(_mainWeaponInfo.fireSFXPath);
                _mainFireTimer = 0f;
            }
            else
            {
                _mainFireTimer += Time.deltaTime;
            }
        }
    }

    /// <summary>
    /// 무기 업그레이드 처리.
    /// </summary>
    public void UpgradeLevel()
    {
        _weaponLevel += 1;
        _subWeapon.UpgradeLevel(_weaponLevel);
    }

    /// <summary>
    /// 전략 무기 발사 처리.
    /// </summary>
    public void FireSubweapon()
    {
        _subWeapon.Fire();
    }
}
