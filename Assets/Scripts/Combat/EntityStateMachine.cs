using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EntityStateMachine
{
    IBehaviourState _activeState;
    Dictionary<Type, List<StateTransition>> _transitionDict = new Dictionary<Type, List<StateTransition>>();
    List<StateTransition> _stateTransitions = new List<StateTransition>();
    List<StateTransition> _anyStateTransitions = new List<StateTransition>();

    private static List<StateTransition> EmptyTransition = new List<StateTransition>();

    public void Update()
    {
        StateTransition transition = GetTransition();
        if(transition != null)
        {
            SetState(transition.ToState);
        }

        _activeState?.Update();
    }

    public void SetState(IBehaviourState state)
    {
        //if (_activeState == state)
        //    return;

        _activeState?.OnExit();

        _activeState = state;

        _transitionDict.TryGetValue(_activeState.GetType(), out _stateTransitions);
        if (_stateTransitions == null)
            _stateTransitions = EmptyTransition;

        _activeState?.OnEnter();
    }

    public void AddTransition(IBehaviourState from, IBehaviourState to, Func<bool> condition, int priority = 1)
    {
        if (_transitionDict.ContainsKey(from.GetType()))
        {
            _transitionDict[from.GetType()].Add(new StateTransition(to, condition, priority));
        } 
        else
        {
            _transitionDict.Add(from.GetType(), new List<StateTransition>() { new StateTransition(to, condition, priority) });
        }
    }

    public void AddTransition(IBehaviourState to, Func<bool> condition, int priority = 1)
    {
        _anyStateTransitions.Add(new StateTransition(to, condition, priority));
    }

    StateTransition GetTransition()
    {
        int currentPriority = 0;
        StateTransition result = null;

        foreach (var transition in _anyStateTransitions)
        {
            if (transition.Condition() && transition.Priority > currentPriority)
            {
                currentPriority = transition.Priority;
                result = transition;
            }
        }

        if (result != null)
            return result;

        foreach (var transition in _stateTransitions)
        {
            if (transition.Condition() && transition.Priority > currentPriority)
            {
                currentPriority = transition.Priority;
                result = transition;
            }
        }

        return result;
    }
}
