using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HireButton : MonoBehaviour, IInfoMessenger
{
    [SerializeField] Image Portrait;

    NPC _npc;

    public void Setup(NPC npc)
    {
        _npc = npc;
        gameObject.SetActive(true);

        Portrait.sprite = npc.Portrait;
    }

    public void OnClick()
    {
        _npc.Converse(true);
    }

    public string GetInfoMessage()
    {
        return _npc.DisplayName;
    }
}
