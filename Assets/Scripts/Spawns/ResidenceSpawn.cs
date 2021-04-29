using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResidenceSpawn : Spawn
{
    [SerializeField] public float Range = 3.0f;
    [SerializeField] public ResidencyDBObject DataObject;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, Range);
    }

    public override void Populate()
    {
        DropController.Instance.SpawnResidence(DataObject, transform);
    }
}
