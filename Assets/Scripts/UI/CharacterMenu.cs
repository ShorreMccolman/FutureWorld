using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterMenu : Menu 
{
    public abstract void Setup(PartyMember member);

    public static event System.Action<bool> OnCharacterMenuOpen;

    public override void OnOpen()
    {
        OnCharacterMenuOpen?.Invoke(true);
    }

    public override void OnClose()
    {
        OnCharacterMenuOpen?.Invoke(false);
    }
}
