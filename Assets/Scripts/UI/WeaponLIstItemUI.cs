using UnityEngine;
using System.Collections;

public class WeaponLIstItemUI : MonoBehaviour
{
    #region Inspector

    public UISprite weaponIconImg;
    public UILabel  weaponNameLabel;
    public UILabel  weaponDescLabel;

    #endregion

    private InfoWeapon _weaponInfo;

    private System.Action _onClickCallback;

    /// <summary>
    /// 무기 정보와 클릭 콜백 함수 설정.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="cal"></param>
    public void Init(InfoWeapon info, System.Action cal)
    {
        _weaponInfo = info;
        weaponIconImg.spriteName = info.iconPath;
        weaponNameLabel.text = info.name;
        weaponDescLabel.text = info.description;

        _onClickCallback = cal;
        UIEventListener.Get(gameObject).onClick = OnItemClick;
    }

    /// <summary>
    /// 해당 리스트 아이템 클릭/터치 설정.
    /// </summary>
    /// <param name="go"></param>
    private void OnItemClick(GameObject go)
    {
        BattleManager.Instance.SetSubWeapon(_weaponInfo);
        _onClickCallback?.Invoke();
    }
}
