using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;

public class ItemDrop : GameStateEntity {

    public InventoryItem Item { get; private set; }

    public ItemDrop(InventoryItem item) : base(null)
    {
        Item = item;
    }

    public ItemDrop(XmlNode node) : base(null, node)
    {
        Item = new InventoryItem(this, node.SelectSingleNode("Item"));
    }

    public override XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("ItemDrop");
        element.AppendChild(Item.ToXml(doc));
        element.AppendChild(base.ToXml(doc));
        return element;
    }
}
