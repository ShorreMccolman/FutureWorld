using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VitalsInfoMessenger : MonoBehaviour, IInfoMessenger, IPopable
{
    [SerializeField] CharacterVitalsDisplay Display;

    public void ShowPopup()
    {
        Popups.ShowVitals(Display);
    }

    public string GetInfoMessage()
    {
        string hp = Display.Member.Vitals.CurrentHP + " / " + Display.Member.Vitals.Stats.EffectiveTotalHP + " Hit Points";
        string mp = Display.Member.Vitals.CurrentSP + " / " + Display.Member.Vitals.Stats.EffectiveTotalSP + " Spell Points";

        return hp + "     " + mp;
    }
}
