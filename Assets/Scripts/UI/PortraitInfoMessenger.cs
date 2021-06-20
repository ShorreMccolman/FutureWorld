using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortraitInfoMessenger : MonoBehaviour, IInfoMessenger, IPopable
{
    [SerializeField] CharacterVitalsDisplay Display;

    public void ShowPopup()
    {
        HUD.Instance.Popups.ShowVitals(Display);
    }

    public string GetInfoMessage()
    {
        return Display.Member.Profile.FullName + ": " + Display.Member.EffectiveStatusCondition();
    }
}
