using UnityEngine;
using System.Collections;

//회전 이동 클래스.
public class RotateMove : Move
{
    Transform centerObject;

    public RotateMove(Transform mover, Transform center, float spd) : base(mover, spd)
    {
        centerObject = center;
    }

    public override void Moving()
    {
        Vector3 radius = centerObject.transform.position - moveTransform.position;
        Vector3 dir = new Vector3(radius.y, -radius.x, 0).normalized;

        moveTransform.position += dir * speed * Time.deltaTime;
        moveTransform.rotation = Quaternion.LookRotation(dir, Vector3.forward);
    }
}
