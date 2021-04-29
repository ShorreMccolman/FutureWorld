using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ConsumableDBObject : ItemDBObject
{
    public Consumable[] Items;
    public override Item[] GetItems() { return Items; }
}
