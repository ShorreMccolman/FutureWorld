using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class InventoryItem : GameStateEntity
{
    public string ID { get; private set; }
    public int Slot { get; private set; }
    public List<int> OccupyingSlots { get; private set; }
    public Item Data { get; private set; }
    public TreasureLevel Level { get; private set; }
    public Enchantment Enchantment { get; private set; }

    public bool IsBroken { get; private set; }
    public bool IsIdentified { get; private set; }

    public bool IsConsumable()
    {
        if (Data is Consumable || Data is Magic)
        {
            return true;
        }

        return false;
    }

    int _effectiveValue;
    public int EffectiveValue { get { return IsBroken ? 1 : _effectiveValue; } }

    public string EffectiveName
    {
        get
        {
            if(!IsIdentified)
            {
                return Data.GetTypeDescription();
            }

            bool comesBefore = false;
            string enchantName = "";
            if (Enchantment != null)
            {
                comesBefore = !Enchantment.Data.DisplayName.Contains("of");
                enchantName = Enchantment.Data.DisplayName;
            }

            string name = "";
            if (comesBefore)
                name = enchantName + " ";
            name += Data.DisplayName;
            if (!comesBefore && Enchantment != null)
                name += " " + enchantName;

            return name;
        }
    }

    public string EffectiveDescription
    {
        get
        {
            if (!IsIdentified)
            {
                return "Not Identified";
            }
            else if (IsBroken)
            {
                return "Broken";
            }
            else
            {
                string description = "";
                if (Enchantment != null)
                {
                    description = "Special: " + Enchantment.SpecialText + "\n\n";
                }
                description += Data.GetItemDescription();
                return description;
            }
        }
    }

    public InventoryItem(GameStateEntity parent, Item item, TreasureLevel level) : base(parent)
    {
        ID = item.ID;
        Level = level;
        Slot = -1;
        OccupyingSlots = new List<int>();
        Data = item;
        IsBroken = false;
        IsIdentified = item.Level == 0;

        if(ItemDatabase.Instance.EnchantDB.ShouldEnchant(item))
        {
            Enchant();
        }

        if(item is Gold)
        {
            Gold gold = item as Gold;
            _effectiveValue = Random.Range(0, GameConstants.GoldDropRangeForItemLevel[level] + 1) + GameConstants.GoldDropBaseAmountForItemLevel[level];
        }
        else if (Enchantment != null)
        {
            _effectiveValue = Enchantment.GetModifiedValue(Data.Value);
        }
        else 
            _effectiveValue = Data.Value;
    }

    public InventoryItem(GameStateEntity parent, XmlNode node) : base(parent, node)
    {
        ID = node.SelectSingleNode("ID").InnerText;
        Level = (TreasureLevel)int.Parse(node.SelectSingleNode("Level").InnerText);
        Slot = int.Parse(node.SelectSingleNode("Slot").InnerText);

        IsIdentified = bool.Parse(node.SelectSingleNode("IsIdentified").InnerText);
        IsBroken = bool.Parse(node.SelectSingleNode("IsBroken").InnerText);
        _effectiveValue = int.Parse(node.SelectSingleNode("Value").InnerText);

        OccupyingSlots = new List<int>();
        XmlNode slotsNode = node.SelectSingleNode("OccupyingSlots");
        XmlNodeList slotNodes = node.SelectNodes("Slot");
        for (int i = 0; i < slotNodes.Count; i++)
        {
            OccupyingSlots.Add(int.Parse(slotNodes.Item(i).InnerText));
        }
        Data = ItemDatabase.Instance.GetItem(ID);
        XmlNode enchant = node.SelectSingleNode("Enchantment");
        if(enchant != null)
            Enchantment = new Enchantment(this, node.SelectSingleNode("Enchantment"));
    }

    public override XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("Item");
        element.AppendChild(XmlHelper.Attribute(doc, "ID", ID));
        element.AppendChild(XmlHelper.Attribute(doc, "Level", (int)Level));
        element.AppendChild(XmlHelper.Attribute(doc, "Slot", Slot));
        element.AppendChild(XmlHelper.Attribute(doc, "IsIdentified", IsIdentified));
        element.AppendChild(XmlHelper.Attribute(doc, "IsBroken", IsBroken));
        element.AppendChild(XmlHelper.Attribute(doc, "Value", _effectiveValue));
        XmlNode slots = doc.CreateElement("OccupyingSlots");
        foreach (var slot in OccupyingSlots)
        {
            slots.AppendChild(XmlHelper.Attribute(doc, "Slot", slot));
        }
        element.AppendChild(slots);
        if(Enchantment !=null)
            element.AppendChild(Enchantment.ToXml(doc));
        element.AppendChild(base.ToXml(doc));
        return element;
    }

    public void AddAtSlot(int slot, int boardWidth, int boardHeight)
    {
        Slot = slot;

        OccupyingSlots = new List<int>();
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                OccupyingSlots.Add(slot + i + j * boardWidth);
            }
        }
    }

    public void RemoveFromSlot()
    {
        Slot = -1;
        OccupyingSlots = new List<int>();
    }

    public void Enchant()
    {
        if(Data is Weapon)
        {
            Enchantment = ItemDatabase.Instance.EnchantDB.GetRandomEnchantment(this, Data as Weapon, Level);
        }
        else if (Data is Armor)
        {
            Enchantment = ItemDatabase.Instance.EnchantDB.GetRandomEnchantment(this, Data as Armor, Level);
        }
    }

    public bool TryIdentify(int skill)
    {
        if (IsIdentified)
            return false;

        bool success = false;
        if(skill >= Data.Level)
        {
            IsIdentified = true;
            success = true;
        }
        return success;
    }

    public bool TryRepair(int skill)
    {
        if (!IsBroken)
            return false;

        bool success = false;
        if (skill >= Data.Level)
        {
            IsBroken = false;
            success = true;
        }
        return success;
    }
}
