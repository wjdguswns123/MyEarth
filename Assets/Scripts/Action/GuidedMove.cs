using UnityEngine;
using System.Collections;

//유도 이동 클래스.
public class GuidedMove : Move
{
    Transform destination;
    float     range;

    public GuidedMove(Transform mover, Transform dest, float spd, float range) : base(mover, spd)
    {
        destination = dest;
        this.range  = range;
    }

    public override void Moving()
    {
        //목표가 있으면 목표를 향해서 방향을 잡고, 없으면 목표를 찾기 시도.
        if(destination == null)
        {
            destination = BattleManager.Instance.SearchNearEnemy(moveTransform, range);
        }
        else
        {
            moveTransform.rotation = Quaternion.LookRotation((destination.transform.position - moveTransform.position), Vector3.forward);
        }
        moveTransform.position += moveTransform.forward * speed * Time.deltaTime;
    }
}
