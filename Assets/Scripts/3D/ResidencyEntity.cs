using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResidencyEntity : Entity3D
{
    Residency _residency;

    public void Setup(Residency residency)
    {
        _residency = residency;
        MouseoverName = residency.DisplayName;
    }

    public override IEnumerator Interact(PartyEntity party)
    {
        yield return new WaitForEndOfFrame();
        
        if (_residency.Data.Hours.IsOpen(TimeManagement.Instance.GetCurrentHour()))
            HUD.Instance.EnterResidence(_residency);
        else
        {
            HUD.Instance.SendInfoMessage("This place is open from " + _residency.Data.Hours.GetOpenHours(), 2.0f);
            HUD.Instance.ExpressSelectedMember(GameConstants.EXPRESSION_SAD, GameConstants.EXPRESSION_SAD_DURATION);
        }
    }
}
