using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestSpawn : Spawn
{
    public SpawnQuantities Quantities;

    [SerializeField] private float Range = 3.0f;

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(173f / 256f, 83f / 256f, 0f / 256f);
        Gizmos.DrawSphere(transform.position, Range);
    }

    public override void Populate()
    {
        DropController.Instance.SpawnChest(Quantities, transform);
    }
}
