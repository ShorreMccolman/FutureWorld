using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ItemSpawn : Spawn
{
    [SerializeField] protected string ItemID;
    [SerializeField] protected TreasureLevel ItemLevel;
    [SerializeField] protected float Range = 3.0f;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, Range);
    }

    public override void Populate()
    {
        Item data;
        if (!string.IsNullOrEmpty(ItemID))
        {
            if(ItemID == "herb")
            {
                int rand = Random.Range(0, 3);
                if (rand == 0)
                    ItemID = "herb_red";
                else if (rand == 1)
                    ItemID = "herb_blue";
                else if (rand == 2)
                    ItemID = "herb_yellow";
            }

            data = ItemDatabase.Instance.GetItem(ItemID);
        }
        else
            data = ItemDatabase.Instance.GetItemByTreasureLevel(ItemLevel);
        InventoryItem item = new InventoryItem(null, data, ItemLevel);
        DropController.Instance.DropItem(item, transform.position);
    }
}
