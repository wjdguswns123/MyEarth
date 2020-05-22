using UnityEngine;
using System.Collections;

public class IngameSceneUI : MonoBehaviour
{
    Collider uiCollider;

	// Use this for initialization
	void Start ()
    {
        uiCollider = GetComponentInChildren<Collider>();
        transform.localPosition = Vector3.back * 5f;
    }
	
	// Update is called once per frame
	void Update ()
    {
#if UNITY_EDITOR
        //해당 UI의 콜라이더 클릭하면 다음 이벤트로.
        RaycastHit hit = new RaycastHit();

        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = UIManager.Instance.UICamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider == uiCollider)
                {
                    IngameSceneManager.Instance.CurrentScene.NextEvent();
                }
            }
        }
#elif UNITY_ANDROID
        //모바일은 해당 UI의 콜라이더 '터치'하면 다음 이벤트로.
                RaycastHit hit = new RaycastHit();

                if(Input.touchCount > 0)
                {
                    if(Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        Ray ray = UIManager.Instance.UICamera.ScreenPointToRay(Input.GetTouch(0).position);

                        if (Physics.Raycast(ray, out hit))
                        {
                            if (hit.collider == uiCollider)
                            {
                                IngameSceneManager.Instance.CurrentScene.NextEvent();
                            }
                        }
                    }
                }
#endif
    }
}
