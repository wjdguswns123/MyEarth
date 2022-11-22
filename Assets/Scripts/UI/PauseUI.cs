using UnityEngine;
using System.Collections;

public class PauseUI : MonoBehaviour
{
    /// <summary>
    /// 계속하기 버튼 클릭.
    /// </summary>
    public void OnContinueBtnClick()
    {
        UIManager.Instance.ClosePopupUI();
        BattleManager.Instance.Resume();
    }

    /// <summary>
    /// 메인 메뉴 버튼 클릭.
    /// </summary>
    public void OnGoMenuBtnClick()
    {
        UIManager.Instance.ClosePopupUI();
        BattleManager.Instance.ForcedEnd();
    }

    /// <summary>
    /// 종료 버튼 클릭.
    /// </summary>
    public void OnExitBtnClick()
    {
        UIManager.Instance.LoadPopupUI("ExitUI");
    }
}
