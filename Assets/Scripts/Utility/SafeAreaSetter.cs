using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeAreaSetter : MonoBehaviour
{
    // 앵커 노치 처리.
    private void Awake()
    {
        UIAnchor anchor = this.GetComponent<UIAnchor>();

        if(anchor != null)
        {
            switch (anchor.side)
            {
                case UIAnchor.Side.Top:
                case UIAnchor.Side.TopLeft:
                case UIAnchor.Side.TopRight:
                    anchor.pixelOffset.y -= Screen.height - Screen.safeArea.yMax;
                    break;
                case UIAnchor.Side.Bottom:
                case UIAnchor.Side.BottomLeft:
                case UIAnchor.Side.BottomRight:
                    anchor.pixelOffset.y += Screen.safeArea.yMin;
                    break;
            }
        }
    }
}
