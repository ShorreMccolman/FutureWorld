using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBehaviour : IBehaviourState
{
    IMoveable _actor;
    IAttacker _attacker;
    Transform _target;

    public AttackBehaviour(IMoveable actor, IAttacker attacker, Transform target)
    {
        _actor = actor;
        _attacker = attacker;
        _target = target;
    }

    public void Update()
    {
        Vector3 levelPosition = new Vector3(_target.position.x, _actor.Transform.position.y, _target.position.z);
        Vector3 targetVec = (levelPosition - _actor.Transform.position);
        Quaternion targetRotation = Quaternion.LookRotation(targetVec);
        _actor.SetTargetDirection(targetRotation);

        _attacker.TryAttack();
    }

    public void OnEnter()
    {
        _actor.MovementSpeed = 0f;
    }

    public void OnExit()
    {

    }
}
