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
        _merchant.TryEnterResidence();
    }
}
