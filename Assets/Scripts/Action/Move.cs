﻿using UnityEngine;

/// <summary>
/// 적 이동 베이스 클래스.
/// </summary>
public abstract class Move
{
    protected Transform moveTransform;
    protected float     speed;

    public Move(Transform mover, float spd) { moveTransform = mover; speed = spd; }
    public virtual void Moving() { }
}
