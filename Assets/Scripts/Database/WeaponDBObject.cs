using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WeaponDBObject : ItemDBObject
{
    public Weapon[] Items;
    public override Item[] GetItems() { return Items; }
}
