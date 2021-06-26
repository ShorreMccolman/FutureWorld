using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PursueBehaviour : IBehaviourState
{
    IMoveable _actor;
    Transform _target;

    public PursueBehaviour(IMoveable actor, Transform target)
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

        float ang = Vector3.Angle(_actor.Transform.forward, targetVec);
        if (ang < 45f)
        {
            _actor.MovementSpeed +=  3 * Time.deltaTime;
        } 
        else
        {
            _actor.MovementSpeed -= Time.deltaTime;
        }
    }

    public void OnEnter()
    {
        
    }

    public void OnExit()
    {

    }
}
