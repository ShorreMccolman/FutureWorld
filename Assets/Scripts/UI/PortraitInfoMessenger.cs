using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PortraitInfoMessenger : MonoBehaviour, IInfoMessenger, IPopable, IPointerDownHandler
{
    [SerializeField] CharacterVitalsDisplay Display;

    public PartyMember Member => Display.Member;

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

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            Display.MenuClick();
        }
    }
}
