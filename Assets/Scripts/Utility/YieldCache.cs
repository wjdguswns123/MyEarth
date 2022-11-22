using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class YieldCache
{
    private static Dictionary<float, WaitForSeconds> _waitTimes = new Dictionary<float, WaitForSeconds>();
    private static Dictionary<float, WaitForSecondsRealtime> _waitRealTimes = new Dictionary<float, WaitForSecondsRealtime>();

    /// <summary>
    /// 해당 시간의 WaitForSeconds 검색. 없으면 생성 후 캐싱.
    /// </summary>
    /// <param name="second"></param>
    /// <returns></returns>
    public static WaitForSeconds WaitForSeconds(float second)
    {
        WaitForSeconds wfs;
        if(!_waitTimes.TryGetValue(second, out wfs))
        {
            _waitTimes.Add(second, wfs = new UnityEngine.WaitForSeconds(second));
        }
        return wfs;
    }

    /// <summary>
    /// 해당 시간의 WaitForSecondsRealtime 검색. 없으면 생성 후 캐싱.
    /// </summary>
    /// <param name="second"></param>
    /// <returns></returns>
    public static WaitForSecondsRealtime WaitForSecondsRealTime(float second)
    {
        WaitForSecondsRealtime wfs;
        if (!_waitRealTimes.TryGetValue(second, out wfs))
        {
            _waitRealTimes.Add(second, wfs = new UnityEngine.WaitForSecondsRealtime(second));
        }
        return wfs;
    }
}
