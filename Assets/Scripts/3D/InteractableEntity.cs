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

        switch(_interactable.Data.Type)
        {
            case InteractableType.Fountain:
                MouseoverName = "Fountain";
                break;
            case InteractableType.Barrel:
                if (_interactable.Stat == CharacterStat.None)
                    MouseoverName = "Empty barrel";
                else
                    MouseoverName = "Barrel of " + GameConstants.ColorForStat[_interactable.Stat] + " liquid";
                break;
            case InteractableType.Well:
                MouseoverName = "Well";
                break;
        }
    }

    public override IEnumerator Interact(PartyEntity party)
    {
        bool success = _interactable.TryInteraction(party.Party.ActiveMember);
        switch(_interactable.Data.Type)
        {
            case InteractableType.Fountain:
                if(!success)
                    InfoMessageReceiver.Send("Refreshing...", 2.0f);
                break;
            case InteractableType.Barrel:
                if (success)
                    MouseoverName = "Empty barrel";
                else
                    InfoMessageReceiver.Send("This barrel is empty", 2.0f);
                break;
            case InteractableType.Well:
                if (!success)
                    InfoMessageReceiver.Send("Refreshing...", 2.0f);
                break;
        }

        yield return new WaitForEndOfFrame();
    }
}
