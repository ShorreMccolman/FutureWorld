using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortraitInfoMessenger : MonoBehaviour, IInfoMessenger, IPopable
{
    [SerializeField] CharacterVitalsDisplay Display;

    public void ShowPopup()
    {
        if (HUD.Instance.HeldItemButton != null)
        {
            Scroll scroll = HUD.Instance.HeldItemButton.Item.Data as Scroll;
            if (scroll != null)
            {
                Popups.ShowScroll(scroll);
                Popups.Supress();
                return;
            }

            if(HUD.Instance.HeldItemButton.Item.IsConsumable())
            {
                HUD.Instance.TryConsume(Display.Member);
                Popups.Supress();
                return;
            }
        }

        Popups.ShowVitals(Display);
    }

    public string GetInfoMessage()
    {
        return Display.Member.Profile.FullName + ": " + Display.Member.EffectiveStatusCondition();
    }
}
