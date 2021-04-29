using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MasterDBObject : ScriptableObject
{
    public List<EnemyDBObject> Enemies;
    public List<ItemDBObject> Items;
}
