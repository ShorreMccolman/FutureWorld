using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartySpellsMessenger : MonoBehaviour, IPopable
{
    public void ShowPopup()
    {
        Dictionary<string, float> spells = Party.Instance.GetPartySpells();

        string text;
        if (spells.Count == 0)
            text = "None";
        else
        {
            text = "";
            foreach (var key in spells.Keys)
            {
                float mins = spells[key] / 60;
                float seconds = spells[key] % 60;
                text += key + "    " + (int)mins + " mins " + (int)seconds + " seconds\n";
            }
        }

        Popups.ShowText("Active Party Spells", text);
    }
}
