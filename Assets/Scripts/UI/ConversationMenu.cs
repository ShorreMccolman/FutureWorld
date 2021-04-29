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
}
