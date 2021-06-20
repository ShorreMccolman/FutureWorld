using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartySpellsMessenger : MonoBehaviour, IPopable
{
    public void ShowPopup()
    {
        HUD.Instance.Popups.ShowText("Active Party Spells", "None");
    }
}
