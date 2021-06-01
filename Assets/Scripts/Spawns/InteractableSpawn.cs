using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableSpawn : Spawn
{
    public InteractableDBObject DataObject;

    [SerializeField] private float Range = 3.0f;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.position, Range);
    }

    public override void Populate()
    {
        DropController.Instance.SpawnInteractable(DataObject.Data, transform);
    }
}
