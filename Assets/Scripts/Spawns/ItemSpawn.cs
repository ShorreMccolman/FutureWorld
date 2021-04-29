using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ItemSpawn : Spawn
{
    [SerializeField] protected TreasureLevel ItemLevel;
    [SerializeField] protected float Range = 3.0f;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, Range);
    }

    public override void Populate()
    {
        Item data = ItemDatabase.Instance.GetItemByTreasureLevel(ItemLevel);
        InventoryItem item = new InventoryItem(null, data, ItemLevel);
        DropController.Instance.DropItem(item, transform.position);
    }
}
