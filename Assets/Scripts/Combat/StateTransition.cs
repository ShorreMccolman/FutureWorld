using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StateTransition
{
    IBehaviourState _toState;
    public IBehaviourState ToState => _toState;

    Func<bool> _condition;
    public Func<bool> Condition => _condition;

    int _priority;
    public int Priority => _priority;

    public StateTransition(IBehaviourState state, Func<bool> condition, int priority)
    {
        _toState = state;
        _condition = condition;
        _priority = priority;
    }
}
