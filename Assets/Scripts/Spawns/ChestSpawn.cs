using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestSpawn : Spawn
{
    public SpawnQuantities Quantities;

    [SerializeField] private float Range = 3.0f;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, Range);
    }

    public override void Populate()
    {
        DropController.Instance.SpawnChest(Quantities, transform);
    }
}
