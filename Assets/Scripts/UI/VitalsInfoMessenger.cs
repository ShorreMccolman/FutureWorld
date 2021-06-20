using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VitalsInfoMessenger : MonoBehaviour, IInfoMessenger, IPopable
{
    [SerializeField] CharacterVitalsDisplay Display;

    public void ShowPopup()
    {
        HUD.Instance.Popups.ShowVitals(Display);
    }

    public string GetInfoMessage()
    {
        string hp = Display.Member.Vitals.CurrentHP + " / " + Display.Member.Vitals.EffectiveTotalHP + " Hit Points";
        string mp = Display.Member.Vitals.CurrentMP + " / " + Display.Member.Vitals.EffectiveTotalMP + " Spell Points";

        return hp + "     " + mp;
    }
}
