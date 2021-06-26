using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoamBehaviour : IBehaviourState
{
    IMoveable _actor;
    Vector3 _origin;

    bool _isIdle;
    float _idleDuration;

    Vector3 _target;

    public RoamBehaviour(IMoveable actor, Vector3 origin)
    {
        _actor = actor;
        _origin = origin;
    }

    public void Update()
    {
        if(_isIdle)
        {
            _idleDuration -= Time.fixedDeltaTime;

            if (_idleDuration <= 0)
                RefreshRoamTarget();
        } 
        else 
        {
            Vector3 levelPosition = new Vector3(_target.x, _actor.Transform.position.y, _target.z);
            float dist = Vector3.Distance(_actor.Transform.position, levelPosition);
            if (dist > 1.0f)
            {
                Vector3 targetVec = (levelPosition - _actor.Transform.position);
                Quaternion targetRotation = Quaternion.LookRotation(targetVec);
                _actor.SetTargetDirection(targetRotation);

                float ang = Vector3.Angle(_actor.Transform.forward, targetVec);
                if (ang < 45f)
                {
                    _actor.MovementSpeed += 3 * Time.deltaTime;
                }
            }
            else
            {
                _actor.MovementSpeed = 0f;
                _idleDuration = Random.Range(7f, 25f);
                _isIdle = true;
            }
        }
    }

    public void RefreshRoamTarget(bool initial = false)
    {
        _isIdle = false;

        if (Vector3.Distance(_target, _origin) > 30f)
        {
            _target = _origin;
            return;
        }

        float angle = Random.Range(0, 360f);
        Vector3 dir = Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward;
        float distance = Random.Range(7f, 15f);

        Ray ray = new Ray(_actor.Transform.position, dir);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            float reduced = hit.distance - 3f;
            if (reduced < 0)
            {
                _idleDuration = 0.5f;
                distance = 0;
            }
            else if (reduced < distance)
                distance = reduced;
        }

        _target = _actor.Transform.position + dir * distance;
    }

    public void OnEnter()
    {
        _target = _actor.Transform.position;
        _isIdle = true;
        _idleDuration = Random.Range(0f, 10f);
    }

    public void OnExit()
    {

    }
}
