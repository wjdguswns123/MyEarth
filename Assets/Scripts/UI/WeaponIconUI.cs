using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WeaponIconUI : MonoBehaviour
{
    public UISprite weaponIcon;

    public void Init(string icon)
    {
        weaponIcon.spriteName = icon;
    }

    //전략무기 UI 열기.
    public void OpenWeaponUI()
    {
        UIManager.Instance.LoadPopupUI("WeaponUI");
    }
}
