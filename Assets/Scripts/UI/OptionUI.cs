using UnityEngine;
using System.Collections;

public class OptionUI : MonoBehaviour
{
    public UIButton BGMButton;
    public UIButton SFXButton;
    public UIButton doubleTouchFireButton;

    public void Init()
    {
        SetBGMBtnStatus();
        SetSFXBtnStatus();
        SetDblTouchFireBtnStatus();
    }

    //닫기 버튼 클릭.
    public void OnCloseBtnClick()
    {
        UIManager.Instance.ClosePopupUI();
    }

    //배경음 버튼 클릭.
    public void OnBGMBtnClick()
    {
        SoundManager.Instance.SetBGM();
        SetBGMBtnStatus();
    }

    //효과음 버튼 클릭.
    public void OnSFXBtnClick()
    {
        SoundManager.Instance.SetSFX();
        SetSFXBtnStatus();
    }

    //더블 터치로 전략 무기 발사 활성화 버튼 클릭.
    public void OnDblTouchFireBtnClick()
    {
        GameManager.Instance.SetDblTouchFire();
        SetDblTouchFireBtnStatus();
    }

    //배경음 켜짐 상태에 따라 배경음 버튼 레이블 설정.
    void SetBGMBtnStatus()
    {
        BGMButton.transform.Find("StatusLabel").GetComponent<UILabel>().text = SoundManager.Instance.OnBGM ? "ON" : "OFF";
    }

    //효과음 켜짐 상태에 따라 효과음 버튼 레이블 설정.
    void SetSFXBtnStatus()
    {
        SFXButton.transform.Find("StatusLabel").GetComponent<UILabel>().text = SoundManager.Instance.OnSFX ? "ON" : "OFF";
    }

    //더블 터치로 전략 무기 발사 활성화 상태에 따라 버튼 레이블 설정.
    void SetDblTouchFireBtnStatus()
    {
        doubleTouchFireButton.transform.Find("StatusLabel").GetComponent<UILabel>().text = GameManager.Instance.IsEnableDlbTouchFire ? "ON" : "OFF";
    }
}
