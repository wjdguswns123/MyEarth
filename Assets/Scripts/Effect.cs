using UnityEngine;
using System.Collections;

public class Effect : MonoBehaviour
{
    private void OnDestroy()
    {
        //삭제될 때 이펙트 매니저 안의 리스트에서 제거 요청.
        EffectManager.Instance.RemoveEffect(gameObject);
    }
}
