using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileInfoMessenger : MonoBehaviour, IPopable
{
    [SerializeField] string Tag;

    public void ShowPopup()
    {
        ProfilePopupInfo info = ProfilePopupInfoDatabase.Instance.GetInfo(Tag);
        string title = info.Title;
        string body = info.Body;
        switch (Tag)
        {
            case "xp":
                body += "\n\n" + Party.Instance.ActiveMember.Profile.GetTrainingDetails();
                break;
            case "attack":
                body += "\n\nRecovery Time: " + Party.Instance.ActiveMember.Vitals.Recovery;
                break;
            case "shoot":
                body += "\n\nRecovery Time: " + Party.Instance.ActiveMember.Vitals.RangedRecovery;
                break;
        }

        HUD.Instance.Popups.ShowText(title, body, 16);
    }
}
