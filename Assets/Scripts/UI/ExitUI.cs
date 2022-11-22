using UnityEngine;
using System.Collections;

public class ExitUI : MonoBehaviour
{
    /// <summary>
    /// 게임 종료하기 처리.
    /// </summary>
    public void OnYesBtnClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// 게임 종료 취소 처리.
    /// </summary>
    public void OnNoBtnClick()
    {
        UIManager.Instance.ClosePopupUI();
        if (BattleManager.Instance.GameState == Def.DefEnum.GameState.PAUSE)
        {
            BattleManager.Instance.Resume();
        }
    }
}
