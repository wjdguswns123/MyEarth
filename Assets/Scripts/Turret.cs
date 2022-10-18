using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour
{
    public GameObject firePosition;
    public Transform  subWeaponPosition;

    InfoWeapon        mainWeaponInfo;

    SubWeapon         subWeapon;

    float             mainFireTimer = 0f;
    int               weaponLevel;

    private void Update()
    {
        if(BattleManager.Instance.GameState == Def.DefEnum.GameState.PLAY)
        {
            FireBullet();  //기본 무기 발사.
        }
    }

    //무기 정보 설정.
    public void Init(InfoWeapon mainInfo, InfoWeapon subInfo)
    {
        mainWeaponInfo = mainInfo;
        weaponLevel = 0;

        ResourceManager.Instance.CreateObjectPool(mainWeaponInfo.bulletPath);

        if (!subInfo.weaponPath.Equals(""))
        {
            subWeapon = ResourceManager.Instance.LoadResource(subInfo.weaponPath, subWeaponPosition).GetComponent<SubWeapon>();
            subWeapon.Init(subInfo);
        }
    }

    //무기 업그레이드 처리.
    public void UpgradeLevel()
    {
        weaponLevel += 1;
        subWeapon.UpgradeLevel(weaponLevel);
    }

    //발사 시간에 맞춰 탄환 발사.
    void FireBullet()
    {
        if(mainFireTimer >= mainWeaponInfo.loadTime)
        {
            Bullet bul = ResourceManager.Instance.LoadResource(mainWeaponInfo.bulletPath, firePosition.transform.position, firePosition.transform.rotation).GetComponent<Bullet>();
            bul.Init(mainWeaponInfo, weaponLevel);
            SoundManager.Instance.PlaySound(mainWeaponInfo.fireSFXPath);
            mainFireTimer = 0f;
        }
        else
        {
            mainFireTimer += Time.deltaTime;
        }
    }

    //전략 무기 발사 처리.
    public void FireSubweapon()
    {
        subWeapon.Fire();
    }
}
