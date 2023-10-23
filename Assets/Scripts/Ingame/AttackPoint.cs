using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPoint : MonoBehaviour
{
    /// <summary>
    /// 해당 포인트에 도착한 적.
    /// </summary>
    public Enemy ArrivalEnemy { get; private set; }

    /// <summary>
    /// 도착한 적이 있는지 확인.
    /// </summary>
    /// <returns></returns>
    public bool IsArriveEnemy()
    {
        return ArrivalEnemy != null;
    }

    /// <summary>
    /// 해당 적이 현재 도착한 적인지 확인.
    /// </summary>
    /// <param name="enemy"></param>
    /// <returns></returns>
    public bool IsArriveThisEnemy(Enemy enemy)
    {
        return ArrivalEnemy == enemy;
    }

    /// <summary>
    /// 도착한 적 등록.
    /// </summary>
    /// <param name="enemy"></param>
    public void ArriveEnemy(Enemy enemy)
    {
        ArrivalEnemy = enemy;
    }

    /// <summary>
    /// 적 등록 해제.
    /// </summary>
    public void Clear()
    {
        ArrivalEnemy = null;
    }
}
