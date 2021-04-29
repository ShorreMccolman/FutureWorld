using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestEntity : Entity3D
{
    Chest _chest;

    public void Setup(Chest chest)
    {
        _chest = chest;
        MouseoverName = "Chest";
    }

    public override IEnumerator Interact(PartyEntity party)
    {
        
        yield return new WaitForEndOfFrame();
        HUD.Instance.InspectChest(_chest);
    }
}
