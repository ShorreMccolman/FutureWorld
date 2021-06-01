using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;

public class Merchant : GameStateEntity
{
    public MerchantData Data { get; protected set; }
    public string DisplayName { get { return Data.MerchantName + " the " + GameConstants.MerchantTitleForStoreType[Data.StoreType]; } }
    public InventoryItem[] BuyItems { get; protected set; }
    public InventoryItem[] SpecialItems { get; protected set; }

    public Merchant(MerchantData data) : base(null)
    {
        Data = data;

        Restock();
    }

    public void TryUpdateStock(float currentTime)
    {
        float update;
        bool refresh = TimeManagement.Instance.ShouldRefresh(GameConstants.RefreshForStoreType[Data.StoreType], LastUpdate, out update);
        LastUpdate = update;
        if (refresh)
        {
            Restock();
        }
    }

    public Merchant(XmlNode node) : base(null, node)
    {
        Data = MerchantDatabase.Instance.GetMerchant(node.SelectSingleNode("ID").InnerText);

        XmlNodeList buys = node.SelectNodes("BuySlot");
        BuyItems = new InventoryItem[buys.Count];
        for(int i=0;i<buys.Count;i++)
        {
            XmlNode info = buys.Item(i).SelectSingleNode("Item");
            if(info != null)
            {
                BuyItems[i] = new InventoryItem(this, info);
            }
        }

        XmlNodeList specials = node.SelectNodes("SpecialSlot");
        SpecialItems = new InventoryItem[specials.Count];
        for (int i = 0; i < specials.Count; i++)
        {
            XmlNode info = specials.Item(i).SelectSingleNode("Item");
            if (info != null)
            {
                SpecialItems[i] = new InventoryItem(this, info);
            }
        }
    }

    public override XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("Merchant");
        element.AppendChild(XmlHelper.Attribute(doc, "ID", Data.ID));

        for (int i = 0; i < BuyItems.Length; i++)
        {
            XmlNode slot = doc.CreateElement("BuySlot");
            if (BuyItems[i] != null)
                slot.AppendChild(BuyItems[i].ToXml(doc));
            element.AppendChild(slot);
        }

        for (int i = 0; i < SpecialItems.Length; i++)
        {
            XmlNode slot = doc.CreateElement("SpecialSlot");
            if (BuyItems[i] != null)
                slot.AppendChild(BuyItems[i].ToXml(doc));
            element.AppendChild(slot);
        }

        element.AppendChild(base.ToXml(doc));
        return element;
    }

    public void Restock()
    {
        switch (Data.StoreType)
        {
            case StoreType.General:
                BuyItems = new InventoryItem[6];
                for (int i = 0; i < BuyItems.Length; i++)
                {
                    BuyItems[i] = ItemDatabase.Instance.GetProduct(Data.BuyInfo.GeneralTypes, Data.BuyInfo.Level);
                    BuyItems[i].Reparent(this);
                }

                SpecialItems = new InventoryItem[0];
                break;
            case StoreType.Weapon:
                BuyItems = new InventoryItem[6];
                for (int i = 0; i < BuyItems.Length; i++)
                {
                    BuyItems[i] = ItemDatabase.Instance.GetProduct(Data.BuyInfo.WeaponTypes, Data.BuyInfo.Level);
                    BuyItems[i].Reparent(this);
                }

                SpecialItems = new InventoryItem[6];
                for (int i = 0; i < SpecialItems.Length; i++)
                {
                    SpecialItems[i] = ItemDatabase.Instance.GetProduct(Data.SpecialInfo.WeaponTypes, Data.SpecialInfo.Level);
                    SpecialItems[i].Reparent(this);
                }
                break;
            case StoreType.Armor:
                BuyItems = new InventoryItem[8];
                for (int i = 0; i < BuyItems.Length; i++)
                {
                    if (i < 4)
                        BuyItems[i] = ItemDatabase.Instance.GetProduct(Data.BuyInfo.GeneralTypes, Data.BuyInfo.Level);
                    else
                        BuyItems[i] = ItemDatabase.Instance.GetProduct(Data.BuyInfo.ArmorTypes, Data.BuyInfo.Level);
                    BuyItems[i].Reparent(this);
                }

                SpecialItems = new InventoryItem[8];
                for (int i = 0; i < SpecialItems.Length; i++)
                {
                    if (i < 4)
                        SpecialItems[i] = ItemDatabase.Instance.GetProduct(Data.SpecialInfo.GeneralTypes, Data.SpecialInfo.Level);
                    else
                        SpecialItems[i] = ItemDatabase.Instance.GetProduct(Data.SpecialInfo.ArmorTypes, Data.SpecialInfo.Level);
                    SpecialItems[i].Reparent(this);
                }
                break;
            case StoreType.Magic:
                BuyItems = new InventoryItem[12];
                for (int i = 0; i < BuyItems.Length; i++)
                {
                    BuyItems[i] = ItemDatabase.Instance.GetProduct(Data.BuyInfo.GeneralTypes, Data.BuyInfo.Level);
                    BuyItems[i].Reparent(this);
                }

                SpecialItems = new InventoryItem[12];
                for (int i = 0; i < SpecialItems.Length; i++)
                {
                    SpecialItems[i] = ItemDatabase.Instance.GetProduct(Data.SpecialInfo.GeneralTypes, Data.SpecialInfo.Level);
                    SpecialItems[i].Reparent(this);
                }
                break;
            case StoreType.Spell:
                BuyItems = new InventoryItem[12];
                for (int i = 0; i < BuyItems.Length; i++)
                {
                    BuyItems[i] = ItemDatabase.Instance.GetProduct(Data.BuyInfo.MagicTypes, Data.BuyInfo.Levels);
                    BuyItems[i].Reparent(this);
                }

                SpecialItems = new InventoryItem[0];
                break;
        }

        foreach (var item in BuyItems)
        {
            item.TryIdentify(10000);
        }
        foreach (var item in SpecialItems)
        {
            item.TryIdentify(10000);
        }
    }

    public bool IsItemValidForStore(InventoryItem item)
    {
        bool isValid = false;
        switch (Data.StoreType)
        {
            case StoreType.General:
                isValid = true;
                break;
            case StoreType.Weapon:
                isValid = item.Data is Weapon;
                break;
            case StoreType.Armor:
                isValid = item.Data is Armor;
                break;
            case StoreType.Magic:
                isValid = true;
                break;
            case StoreType.Spell:
                isValid = !(item.Data is Weapon);
                if (item.Data is Armor)
                {
                    Armor armor = item.Data as Armor;
                    isValid = armor.Type == ArmorType.Generic;
                }
                break;
        }
        return isValid;
    }

    public void ConfirmPurchase(InventoryItem item)
    {
        for(int i=0;i<BuyItems.Length;i++)
        {
            if (BuyItems[i] == item)
                BuyItems[i] = null;
        }
        for (int i = 0; i < SpecialItems.Length; i++)
        {
            if (SpecialItems[i] == item)
                SpecialItems[i] = null;
        }
    }
}
