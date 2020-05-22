using UnityEngine;
using System.Collections;

//직선 이동 클래스.
public class LinearMove : Move
{
    public LinearMove(Transform mover, float spd) : base(mover, spd)
    {
    }

    public override void Moving()
    {
        moveTransform.position += moveTransform.forward * speed * Time.deltaTime;
    }
}
