using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ArmorDBObject : ItemDBObject
{
    public Armor[] Items;
    public override Item[] GetItems() { return Items; }
}
