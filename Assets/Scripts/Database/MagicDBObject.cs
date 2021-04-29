using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MagicDBObject : ItemDBObject
{
    public Magic[] Items;
    public override Item[] GetItems() { return Items; }
}
