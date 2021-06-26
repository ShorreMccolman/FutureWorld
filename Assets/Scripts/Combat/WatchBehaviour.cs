using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchBehaviour : IBehaviourState
{
    IMoveable _actor;
    Transform _target;

    public WatchBehaviour(IMoveable actor, Transform target)
    {
        _actor = actor;
        _target = target;
    }

    public void Update()
    {
        Vector3 levelPosition = new Vector3(_target.position.x, _actor.Transform.position.y, _target.position.z);
        Vector3 targetVec = (levelPosition - _actor.Transform.position);
        Quaternion targetRotation = Quaternion.LookRotation(targetVec);
        _actor.SetTargetDirection(targetRotation);
    }

    public void OnEnter()
    {
        _actor.MovementSpeed = 0;
    }

    public void OnExit()
    {

    }
}
