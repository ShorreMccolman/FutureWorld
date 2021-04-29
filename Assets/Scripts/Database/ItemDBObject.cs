using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDBObject : ScriptableObject
{
    public virtual Item[] GetItems() { return new Item[0]; }
    public GameObject defaultModel;
}
