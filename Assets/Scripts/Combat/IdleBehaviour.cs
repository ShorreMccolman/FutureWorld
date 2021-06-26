using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleBehaviour : IBehaviourState
{
    IMoveable _moveable;

    public IdleBehaviour(IMoveable moveable)
    {
        _moveable = moveable;
    }

    public void Update()
    {

    }

    public void OnEnter()
    {
        _moveable.MovementSpeed = 0;
    }

    public void OnExit()
    {

    }
}
