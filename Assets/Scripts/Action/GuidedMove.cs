using UnityEngine;
using System.Collections;

/// <summary>
/// 유도 이동 클래스.
/// </summary>
public class GuidedMove : Move
{
    private Transform _destination;
    private float     _range;

    public GuidedMove(Transform mover, Transform dest, float spd, float range) : base(mover, spd)
    {
        _destination = dest;
        this._range = range;
    }

    public override void Moving()
    {
        //목표가 있으면 목표를 향해서 방향을 잡고, 없으면 목표를 찾기 시도.
        if(_destination == null)
        {
            var destEnemy = BattleManager.Instance.SearchNearEnemy(moveTransform, _range);
            _destination = destEnemy != null ? destEnemy.transform : null;
        }
        else
        {
            moveTransform.rotation = Quaternion.LookRotation((_destination.transform.position - moveTransform.position), Vector3.forward);
        }
        moveTransform.position += moveTransform.forward * speed * Time.deltaTime;
    }
}
