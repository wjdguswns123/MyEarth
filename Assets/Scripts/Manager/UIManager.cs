using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIManager : Singleton<UIManager>
{
    public Transform  popupBGImg;    //팝업 UI 뒷 가림막.
    public Camera     UICamera;

    Stack<GameObject> openPopupUIStack = new Stack<GameObject>();

    //해당 이름의 UI 불러오기.
    public GameObject LoadUI(string uiName)
    {
        return ResourceManager.Instance.LoadResource(string.Format("UI/{0}", uiName), transform);
    }

    //해당 이름의 UI 불러와서 parent 자식으로 설정.
    public GameObject LoadUI(string uiName, Transform parent)
    {
        GameObject ui = ResourceManager.Instance.LoadResource(string.Format("UI/{0}", uiName), transform);
        ui.transform.parent = parent;
        return ui;
    }

    //UI 불러오기.
    public GameObject LoadPopupUI(string uiName)
    {
        GameObject ui = LoadUI(uiName);
        if(openPopupUIStack.Count > 0)
        {
            UIPanel[] panels = openPopupUIStack.Peek().GetComponentsInChildren<UIPanel>();
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
        openPopupUIStack.Push(ui);
        popupBGImg.gameObject.SetActive(false);
        popupBGImg.parent = ui.transform;
        popupBGImg.gameObject.SetActive(true);
        
        return ui;
    }

    //UI 닫기.
    public void ClosePopupUI()
    {
        GameObject ui = openPopupUIStack.Pop();
        if(openPopupUIStack.Count > 0)
        {
            popupBGImg.parent = openPopupUIStack.Peek().transform;
        }
        else
        {
            popupBGImg.parent = transform;
            popupBGImg.gameObject.SetActive(false);
        }
        Destroy(ui);
    }
}
