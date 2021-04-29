using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GoldDBObject : ItemDBObject
{
    public Gold[] Items;
    public override Item[] GetItems() { return Items; }
}
