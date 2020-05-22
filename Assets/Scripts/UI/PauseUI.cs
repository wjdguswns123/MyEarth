using UnityEngine;
using System.Collections;

public class PauseUI : MonoBehaviour
{
    //계속하기 버튼 클릭.
    public void OnContinueBtnClick()
    {
        UIManager.Instance.ClosePopupUI();
        BattleManager.Instance.Play();
    }
    
    //메인 메뉴 버튼 클릭.
    public void OnGoMenuBtnClick()
    {
        UIManager.Instance.ClosePopupUI();
        BattleManager.Instance.ForcedEnd();
    }

    //종료 버튼 클릭.
    public void OnExitBtnClick()
    {
        UIManager.Instance.LoadPopupUI("ExitUI");
    }
}
