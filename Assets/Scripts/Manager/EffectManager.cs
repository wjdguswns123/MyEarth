using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Def;

public class EffectManager : Singleton<EffectManager>
{
    private List<Effect> _effectList = new List<Effect>();

    /// <summary>
    /// 인게임 이펙트 초기화.
    /// </summary>
    public void InitIngameEffects()
    {
        ResourceManager.Instance.CreateObjectsPool(ResourcePath.EFFECT_PATH, 5);
    }

    /// <summary>
    /// 이펙트 이름, 생성될 위치, 회전값을 받아 이펙트 생성.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="pos"></param>
    /// <param name="rot"></param>
    /// <returns></returns>
    public GameObject LoadEffect(string name, Vector3 pos, Quaternion rot)
    {
        GameObject effectObj = ResourceManager.Instance.LoadResource(ResourcePath.EFFECT_PATH, name, pos, rot);
        var effect = effectObj.GetComponent<Effect>();
        _effectList.Add(effect);
        return effectObj;
    }

    /// <summary>
    /// 이펙트 이름, 생성 시 상위 객체 받아 이펙트 생성.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public GameObject LoadEffect(string name, Transform parent)
    {
        GameObject effectObj = ResourceManager.Instance.LoadResource(ResourcePath.EFFECT_PATH, name, parent);
        var effect = effectObj.GetComponent<Effect>();
        _effectList.Add(effect);
        return effectObj;
    }

    /// <summary>
    /// 현재 재생중인 이펙트 일시정지.
    /// </summary>
    public void Pause()
    {
        int count = _effectList.Count;
        for(int i = 0; i < count; ++i)
        {
            _effectList[i].Pause();
        }
    }

    /// <summary>
    /// 현재 재생중인 이펙트 일시정지 해제.
    /// </summary>
    public void Resume()
    {
        int count = _effectList.Count;
        for(int i = 0; i < count; ++i)
        {
            _effectList[i].Resume();
        }
    }

    /// <summary>
    /// 해당 이펙트 리스트에서 제거.
    /// </summary>
    /// <param name="eff"></param>
    public void RemoveEffect(Effect eff)
    {
        if(eff != null && _effectList.Contains(eff))
        {
            _effectList.Remove(eff);
        }
    }

    /// <summary>
    /// 현재 출력 중인 이펙트 모두 제거.
    /// </summary>
    public void ClearEffectList()
    {
        int count = _effectList.Count;
        for(int i = count - 1; i >= 0; --i)
        {
            _effectList.RemoveAt(i);
        }
    }
}
