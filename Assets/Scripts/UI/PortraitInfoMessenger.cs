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
                HUD.Instance.Popups.ShowScroll(scroll);
                HUD.Instance.Popups.Supress();
                return;
            }

            if(HUD.Instance.HeldItemButton.Item.IsConsumable())
            {
                HUD.Instance.TryConsume(Display.Member);
                HUD.Instance.Popups.Supress();
                return;
            }
        }

        HUD.Instance.Popups.ShowVitals(Display);
    }

    public string GetInfoMessage()
    {
        return Display.Member.Profile.FullName + ": " + Display.Member.EffectiveStatusCondition();
    }
}
