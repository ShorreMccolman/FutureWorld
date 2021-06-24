using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartySpellsMessenger : MonoBehaviour, IPopable
{
    public void ShowPopup()
    {
        Popups.ShowText("Active Party Spells", "None");
    }
}
