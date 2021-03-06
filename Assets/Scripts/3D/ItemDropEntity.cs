using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropEntity : Entity3D
{
    [SerializeField] private Transform anchor;

    ItemDrop _drop;

    public void Setup(ItemDrop drop)
    {
        _drop = drop;
        GameObject obj = Instantiate(drop.Item.Data.model, anchor);
        MouseoverName = "Get " + drop.Item.Data.GetTypeDescription();
    }

    public override IEnumerator Interact(PartyEntity party)
    {
        bool success = party.Party.PickupDrop(_drop);
        yield return new WaitForEndOfFrame();
        if (success)
        {
            Kill();
        }
    }
}
