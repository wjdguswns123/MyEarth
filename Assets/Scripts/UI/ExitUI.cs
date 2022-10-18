using UnityEngine;
using System.Collections;

public class ExitUI : MonoBehaviour
{
    //게임 종료하기 처리.
    public void OnYesBtnClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    //게임 종료 취소 처리.
    public void OnNoBtnClick()
    {
        UIManager.Instance.ClosePopupUI();
        if (BattleManager.Instance.GameState == Def.DefEnum.GameState.PAUSE)
        {
            BattleManager.Instance.Resume();
        }
    }
}
