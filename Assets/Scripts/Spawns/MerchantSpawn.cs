using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchantSpawn : Spawn
{
    [SerializeField] public float Range = 3.0f;
    [SerializeField] public string MerchantID;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(transform.position, Range);
    }

    public override void Populate()
    {
        DropController.Instance.SpawnMerchants(MerchantID, transform);
    }
}
