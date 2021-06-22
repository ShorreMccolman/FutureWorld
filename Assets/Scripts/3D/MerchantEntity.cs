using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchantEntity : Entity3D
{
    Merchant _merchant;

    public void Setup(Merchant merchant)
    {
        _merchant = merchant;
        MouseoverName = _merchant.Data.StoreName;
    }

    public override IEnumerator Interact(PartyEntity party)
    {
        yield return new WaitForEndOfFrame();

        if(_merchant.Data.Hours.IsOpen(TimeManagement.Instance.GetCurrentHour()))
            HUD.Instance.EnterResidence(_merchant);
        else
        {
            HUD.Instance.SendInfoMessage("This place is open from " + _merchant.Data.Hours.GetOpenHours(), 2.0f);
            Party.Instance.ActiveMember.Vitals.Express(GameConstants.EXPRESSION_SAD, GameConstants.EXPRESSION_SAD_DURATION);
        }
    }
}
