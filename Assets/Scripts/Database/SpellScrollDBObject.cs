using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SpellScrollDBObject : ItemDBObject
{
    public MagicScroll[] Items;
    public override Item[] GetItems() { return Items; }
}

