using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZigZagBehaviour : IBehaviourState
{
    IMoveable _actor;
    Transform _target;

    bool _movingRight;
    float _changeDirectionTimer;

    public ZigZagBehaviour(IMoveable actor, Transform target)
    {
        _actor = actor;
        _target = target;
    }

    public void Update()
    {
        _changeDirectionTimer -= Time.deltaTime;
        if (_changeDirectionTimer <= 0)
        {
            _changeDirectionTimer = Random.Range(6f, 8f);
            _movingRight = !_movingRight;
        }

        Vector3 levelPosition = new Vector3(_target.position.x, _actor.Transform.position.y, _target.position.z);
        Vector3 targetVec = (levelPosition - _actor.Transform.position);
        float zigAngle = _movingRight ? 30f : -30f;
        Quaternion targetRotation = Quaternion.LookRotation(targetVec) * Quaternion.Euler(0, zigAngle, 0);
        _actor.SetTargetDirection(targetRotation);

        float ang = Vector3.Angle(_actor.Transform.forward, targetVec);
        if (ang < 60f)
        {
            _actor.MovementSpeed += 3 * Time.deltaTime;
        }
        else
        {
            _actor.MovementSpeed -= Time.deltaTime;
        }
    }

    public void OnEnter()
    {
        _changeDirectionTimer = 0f;
        _movingRight = Random.Range(0, 2) == 0;
    }

    public void OnExit()
    {

    }
}
