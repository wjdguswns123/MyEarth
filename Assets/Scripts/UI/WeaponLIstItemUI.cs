using UnityEngine;
using System.Collections;

public class WeaponLIstItemUI : MonoBehaviour
{
    public UISprite weaponIconImg;
    public UILabel  weaponNameLabel;
    public UILabel  weaponDescLabel;

    InfoWeapon      weaponInfo;

    System.Action   onClickCallback;
    
    //무기 정보와 클릭 콜백 함수 설정.
    public void Init(InfoWeapon info, System.Action cal)
    {
        weaponInfo = info;
        weaponIconImg.spriteName = info.iconPath;
        weaponNameLabel.text = info.name;
        weaponDescLabel.text = info.description;

        onClickCallback = cal;
        UIEventListener.Get(gameObject).onClick = OnItemClick;
    }

    //해당 리스트 아이템 클릭/터치 설정.
    void OnItemClick(GameObject go)
    {
        BattleManager.Instance.SetSubWeapon(weaponInfo);
        if(onClickCallback != null)
        {
            onClickCallback();
        }
    }
}
