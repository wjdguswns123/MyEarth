using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIManager : Singleton<UIManager>
{
    private const string UI_PATH = "UI/";

    #region Inspector

    public Transform  popupBGImg;    //팝업 UI 뒷 가림막.
    public Camera     UICamera;

    #endregion

    private Stack<GameObject> _openPopupUIStack;

    public GameObject LoadUI(string uiName)
    {
        return LoadUI(uiName, this.transform);
    }

    /// <summary>
    /// 해당 이름의 UI 불러오기.
    /// </summary>
    /// <param name="uiName"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public GameObject LoadUI(string uiName, Transform parent)
    {
        GameObject ui = Instantiate(Resources.Load<GameObject>(UI_PATH + uiName));
        ui.transform.parent = parent;
        ui.transform.localPosition = Vector3.zero;
        ui.transform.localScale = Vector3.one;
        return ui;
    }

    /// <summary>
    /// 팝업 UI 불러오기.
    /// </summary>
    /// <param name="uiName"></param>
    /// <returns></returns>
    public GameObject LoadPopupUI(string uiName)
    {
        if(_openPopupUIStack == null)
        {
            _openPopupUIStack = new Stack<GameObject>();
        }

        GameObject ui = LoadUI(uiName);
        if(_openPopupUIStack.Count > 0)
        {
            UIPanel[] panels = _openPopupUIStack.Peek().GetComponentsInChildren<UIPanel>();
            int maxDepth = 0;
            for(int i = 0; i < panels.Length; ++i)
            {
                if(maxDepth < panels[i].depth)
                {
                    maxDepth = panels[i].depth;
                }
            }
            ui.GetComponent<UIPanel>().depth = maxDepth + 1;
        }
        _openPopupUIStack.Push(ui);
        popupBGImg.gameObject.SetActive(false);
        popupBGImg.parent = ui.transform;
        popupBGImg.gameObject.SetActive(true);
        
        return ui;
    }

    /// <summary>
    /// 팝업 UI 닫기.
    /// </summary>
    public void ClosePopupUI()
    {
        GameObject ui = _openPopupUIStack.Pop();
        if(_openPopupUIStack.Count > 0)
        {
            popupBGImg.parent = _openPopupUIStack.Peek().transform;
        }
        else
        {
            popupBGImg.parent = transform;
            popupBGImg.gameObject.SetActive(false);
        }
        Destroy(ui);
    }
}
