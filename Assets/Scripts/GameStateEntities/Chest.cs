using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

[System.Serializable]
public class SpawnQuantities
{
    public int L1;
    public int L2;
    public int L3;
    public int L4;
    public int L5;
    public int L6;
    public int Artifact;

    public string[] Custom;

    public SpawnQuantities(string[] split)
    {
        L1 = int.Parse(split[0]);
        L2 = int.Parse(split[1]);
        L3 = int.Parse(split[2]);
        L4 = int.Parse(split[3]);
        L5 = int.Parse(split[4]);
        L6 = int.Parse(split[5]);

        Artifact = 0;
        Custom = new string[0];
    }

    public TreasureLevel GetValidTreasureLevel()
    {
        int min = -1;
        int max = -1;
        if (L1 > 0)
            min = 0;
        else if (L2 > 0)
            min = 1;
        else if (L3 > 0)
            min = 2;
        else if (L4 > 0)
            min = 3;
        else if (L5 > 0)
            min = 4;
        else if (L6 > 0)
            min = 5;

        if (L6 > 0)
            max = 5;
        else if (L5 > 0)
            max = 4;
        else if (L4 > 0)
            max = 3;
        else if (L3 > 0)
            max = 2;
        else if (L2 > 0)
            max = 1;
        else if (L1 > 0)
            max = 0;

        return (TreasureLevel)Random.Range(min, max + 1);
    }
}

public class Chest : GameStateEntity
{
    Inventory _inventory;
    public Inventory Inventory { get { return _inventory; } }

    Trap _trap;
    public Trap Trap { get { return _trap; } }

    public ChestData Data { get; protected set; }

    // USE FOR DEBUG ONLY
    public Chest(SpawnQuantities quantities) : base(Party.Instance)
    {
        _inventory = new Inventory(this, quantities);
    }

    public Chest(ChestData data) : base(null)
    {
        Data = data;
        _inventory = new Inventory(this, data.Quantities);
        _trap = new Trap(this);
    }

    public Chest(XmlNode node) : base(null, node)
    {
        Data = ChestDatabase.Instance.GetChestData(node.SelectSingleNode("ID").InnerText);
        _inventory = new Inventory(this, node);
        _trap = new Trap(node.SelectSingleNode("Trap"), this);
    }

    public override XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("Chest");
        element.AppendChild(XmlHelper.Attribute(doc, "ID", Data.ID));
        element.AppendChild(_inventory.ToXml(doc));
        element.AppendChild(_trap.ToXml(doc));
        element.AppendChild(base.ToXml(doc));

        return element;
    }
}
