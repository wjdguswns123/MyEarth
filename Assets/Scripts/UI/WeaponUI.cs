using UnityEngine;
using System.Collections;
using Def;

public class WeaponUI : MonoBehaviour
{
    public UIGrid listGrid;

    private System.Action _onSelect;

    public void Set(System.Action onSelect)
    {
        _onSelect = onSelect;
        SetWeaponList();
    }

    //무기 리스트 설정.
    void SetWeaponList()
    {
        foreach(InfoWeapon info in InfoManager.Instance.infoWeaponList.Values)
        {
            if(info.difficulty == (int)DataManager.Instance.gameDifficulty && info.isNormal == 0)
            {
                GameObject item = UIManager.Instance.LoadUI("WeaponListItemUI", listGrid.transform);
                item.GetComponent<WeaponLIstItemUI>().Init(info, CloseUI);
            }
        }

        listGrid.Reposition();
    }

    //UI 닫기.
    public void CloseUI()
    {
        //UI 닫을 때 게임 다시 실행.
        _onSelect?.Invoke();
        //BattleManager.Instance.ShowTutorial();
        UIManager.Instance.ClosePopupUI();
    }
}
