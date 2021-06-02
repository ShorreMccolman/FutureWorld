using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractableEffect
{
    RestoreHP,
    RestoreMP,
    StatusEffect,
    PermanentStat,
    Charity
}


public class InteractableEntity : Entity3D
{
    Interactable _interactable;

    public void Setup(Interactable interactable)
    {
        _interactable = interactable;
        MouseoverName = _interactable.Data.Mouseover;
    }

    public override IEnumerator Interact(PartyEntity party)
    {
        bool success = _interactable.TryInteraction(party.Party.ActiveMember);
        if(!success)
        {
            HUD.Instance.SendInfoMessage("Refreshing...", 2.0f);
        }

        yield return new WaitForEndOfFrame();
    }
}
