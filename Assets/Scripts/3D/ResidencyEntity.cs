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
        _residency.TryEnterResidence();
    }
}
