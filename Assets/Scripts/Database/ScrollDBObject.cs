using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ScrollDBObject : ItemDBObject
{
    public Scroll[] Items;
    public override Item[] GetItems() { return Items; }
}
