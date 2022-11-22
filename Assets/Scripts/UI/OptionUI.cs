using UnityEngine;
using System.Collections;

public class OptionUI : MonoBehaviour
{
    #region Inspector

    public UIButton BGMButton;
    public UIButton SFXButton;
    public UIButton doubleTouchFireButton;

    public UILabel BGMOnText;
    public UILabel SFXOnText;
    public UILabel doubleTouchFireOnText;

    #endregion

    public void Init()
    {
        SetBGMBtnStatus();
        SetSFXBtnStatus();
        SetDblTouchFireBtnStatus();
    }

    /// <summary>
    /// 닫기 버튼 클릭.
    /// </summary>
    public void OnCloseBtnClick()
    {
        UIManager.Instance.ClosePopupUI();
    }

    /// <summary>
    /// 배경음 버튼 클릭.
    /// </summary>
    public void OnBGMBtnClick()
    {
        SoundManager.Instance.SetBGM();
        SetBGMBtnStatus();
    }

    /// <summary>
    /// 효과음 버튼 클릭.
    /// </summary>
    public void OnSFXBtnClick()
    {
        SoundManager.Instance.SetSFX();
        SetSFXBtnStatus();
    }

    /// <summary>
    /// 더블 터치로 전략 무기 발사 활성화 버튼 클릭.
    /// </summary>
    public void OnDblTouchFireBtnClick()
    {
        GameManager.Instance.SetDblTouchFire();
        SetDblTouchFireBtnStatus();
    }

    /// <summary>
    /// 배경음 켜짐 상태에 따라 배경음 버튼 레이블 설정.
    /// </summary>
    private void SetBGMBtnStatus()
    {
        BGMOnText.text = SoundManager.Instance.OnBGM ? "ON" : "OFF";
    }

    /// <summary>
    /// 효과음 켜짐 상태에 따라 효과음 버튼 레이블 설정.
    /// </summary>
    private void SetSFXBtnStatus()
    {
        SFXOnText.text = SoundManager.Instance.OnSFX ? "ON" : "OFF";
    }

    /// <summary>
    /// 더블 터치로 전략 무기 발사 활성화 상태에 따라 버튼 레이블 설정.
    /// </summary>
    private void SetDblTouchFireBtnStatus()
    {
        doubleTouchFireOnText.text = GameManager.Instance.IsEnableDlbTouchFire ? "ON" : "OFF";
    }
}
