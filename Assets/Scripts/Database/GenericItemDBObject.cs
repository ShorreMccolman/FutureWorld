using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GenericItemDBObject : ItemDBObject
{
    public Item[] Items;
    public override Item[] GetItems() { return Items; }
}
