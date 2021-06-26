using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBehaviourState
{
    public void Update();
    public void OnEnter();
    public void OnExit();
}
