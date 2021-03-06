using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestSpawn : Spawn
{
    public ChestDBObject DBObject;

    [SerializeField] private float Range = 3.0f;

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(85f / 256f, 42f / 256f, 0f / 256f);
        Gizmos.DrawSphere(transform.position, Range);
    }

    public override void Populate()
    {
        DropController.Instance.SpawnChest(DBObject.Data, transform);
    }
}
