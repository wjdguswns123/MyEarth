using UnityEngine;
using System.Collections;
using Def;

public class WeaponUI : MonoBehaviour
{
    #region Inspector

    public UIGrid listGrid;

    #endregion

    private System.Action _onSelect;

    /// <summary>
    /// UI 설정.
    /// </summary>
    /// <param name="onSelect"></param>
    public void Set(System.Action onSelect)
    {
        _onSelect = onSelect;

        foreach (InfoWeapon info in InfoManager.Instance.infoWeaponList.Values)
        {
            if (info.difficulty == (int)BattleManager.Instance.GameDifficulty && info.isNormal == 0)
            {
                GameObject item = UIManager.Instance.LoadUI("WeaponListItemUI", listGrid.transform);
                item.GetComponent<WeaponLIstItemUI>().Init(info, CloseUI);
            }
        }

        listGrid.Reposition();
    }

    /// <summary>
    /// UI 닫기.
    /// </summary>
    private void CloseUI()
    {
        //UI 닫을 때 게임 다시 실행.
        _onSelect?.Invoke();
        //BattleManager.Instance.ShowTutorial();
        UIManager.Instance.ClosePopupUI();
    }
}
