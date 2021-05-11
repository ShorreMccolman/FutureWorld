using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationMenu : Menu
{
    [SerializeField] protected Transform DialogAnchor;
    [SerializeField] protected GameObject DialogOptionPrefab;
    [SerializeField] protected GameObject InputOptionPrefab;

    public virtual void DisplayDialog(string dialog)
    {

    }

    public override void OnOpen()
    {
        base.OnOpen();
        TimeManagement.Instance.SetTimeControl(TimeControl.Manual);
    }

    public override void OnClose()
    {
        base.OnClose();
        TimeManagement.Instance.SetTimeControl(TimeControl.Auto);
    }
}
