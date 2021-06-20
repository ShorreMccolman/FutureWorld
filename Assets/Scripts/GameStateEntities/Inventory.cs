using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Linq;


public class InventoryGrid
{
    int _width;
    int _height;
    bool[] _grid;

    public InventoryGrid(int width, int height)
    {
        _width = width;
        _height = height;
        _grid = new bool[_width * _height];
    }

    public bool IsSlotOccupied(int slot) { return _grid[slot]; }

    public bool RemoveItemFromSlot(InventoryItem item)
    {
        if (item == null)
            return false;

        int specifiedSlot = item.Slot;

        List<int> slots = new List<int>();

        int x = specifiedSlot % _width;
        int y = specifiedSlot / _width;

        if (x + item.Data.Width > _width)
        {
            return false;
        }

        if (y + item.Data.Height > _height)
        {
            return false;
        }

        bool slotOccupied = true;
        for (int j = 0; j < item.Data.Width; j++)
        {
            for (int k = 0; k < item.Data.Height; k++)
            {

                int index = specifiedSlot + j + k * _width;
                slots.Add(index);
                if (!_grid[index])
                {
                    slotOccupied = false;
                    return false;
                }
            }
            if (!slotOccupied)
                return false;
        }

        if (slotOccupied)
        {
            foreach (var slot in slots)
            {
                _grid[slot] = false;
            }

            item.RemoveFromSlot();
            return true;
        }

        return false;
    }

    public int AddItemRandomly(InventoryItem item)
    {
        if (item == null)
            return -1;

        List<int> nums = new List<int>();
        for (int i = 0; i < _grid.Length; i++)
            nums.Add(i);

        nums = nums.OrderBy(x => Random.value).ToList();

        List<int> slots = new List<int>();

        for (int i = 0; i < _grid.Length; i++)
        {
            int rand = nums[i];

            slots.Clear();

            int x = rand % _width;
            int y = rand / _width;

            if (x + item.Data.Width > _width)
            {
                continue;
            }

            if (y + item.Data.Height > _height)
            {
                continue;
            }

            bool slotOccupied = false;
            for (int j = 0; j < item.Data.Width; j++)
            {
                for (int k = 0; k < item.Data.Height; k++)
                {

                    int index = rand + j + k * _width;
                    slots.Add(index);
                    if (_grid[index])
                    {
                        slotOccupied = true;
                        break;
                    }
                }
                if (slotOccupied)
                    break;
            }

            if (!slotOccupied)
            {
                foreach (var slot in slots)
                {
                    _grid[slot] = true;
                }

                item.AddAtSlot(rand, _width, _height);
                return i;
            }

        }

        return -1;
    }

    public int AddItem(InventoryItem item)
    {
        if (item == null)
            return -1;

        List<int> slots = new List<int>();

        for(int i=0; i<_grid.Length;i++)
        {
            slots.Clear();

            int x = i % _width;
            int y = i / _width;

            if (x + item.Data.Width > _width)
            {
                continue;
            }

            if (y + item.Data.Height > _height)
            {
                continue;
            }

            bool slotOccupied = false;
            for (int j=0; j < item.Data.Width;j++)
            {
                for (int k=0;k < item.Data.Height;k++)
                {

                    int index = i + j + k * _width;
                    slots.Add(index);
                    if (_grid[index])
                    {
                        slotOccupied = true;
                        break;
                    }
                }
                if (slotOccupied)
                    break;
            }

            if(!slotOccupied)
            {
                foreach(var slot in slots)
                {
                    _grid[slot] = true;
                }

                item.AddAtSlot(i, _width, _height);
                return i;
            }

        }
        
        return -1;
    }

    public bool AddItem(int specifiedSlot, InventoryItem item)
    {
        if (item == null)
            return false;

        List<int> slots = new List<int>();

        int x = specifiedSlot % _width;
        int y = specifiedSlot / _width;

        if (x + item.Data.Width > _width)
        {
            return false;
        }

        if (y + item.Data.Height > _height)
        {
            return false;
        }

        bool slotOccupied = false;
        for (int j = 0; j < item.Data.Width; j++)
        {
            for (int k = 0; k < item.Data.Height; k++)
            {

                int index = specifiedSlot + j + k * _width;
                slots.Add(index);
                if (_grid[index])
                {
                    slotOccupied = true;
                    return false;
                }
            }
            if (slotOccupied)
                return false;
        }

        if (!slotOccupied)
        {
            foreach (var slot in slots)
            {
                _grid[slot] = true;
            }

            item.AddAtSlot(specifiedSlot, _width, _height);
            return true;
        }
        
        return false;
    }


}

public class Inventory : GameStateEntity
{
    InventoryGrid _inventoryGrid;

    public List<InventoryItem> Items { get; private set; }

    int _width, _height;

    public Inventory(GameStateEntity parent, CharacterData data, Equipment equipment) : base(parent)
    {
        _width = 14;
        _height = 9;

        _inventoryGrid = new InventoryGrid(_width, _height);

        InitInventory(data.Skills, equipment, data.AutoEquip);
    }

    public Inventory(GameStateEntity parent, SpawnQuantities quantities) : base(parent)
    {
        _width = 9;
        _height = 9;

        _inventoryGrid = new InventoryGrid(_width, _height);

        Items = new List<InventoryItem>();

        Queue<InventoryItem> spawnItems = ItemDatabase.Instance.ItemQueueBySpawnQuantities(quantities);
        while(spawnItems.Count > 0)
        {
            InventoryItem item = spawnItems.Dequeue();
            if(_inventoryGrid.AddItemRandomly(item) == -1)
            {
                Debug.Log("Could not add item");
            } else
                Items.Add(item);
        }
    }

    public Inventory(GameStateEntity parent, XmlNode node) : base(parent, node)
    {
        Items = new List<InventoryItem>();

        XmlNode inventoryNode = node.SelectSingleNode("Inventory");
        _width = int.Parse(inventoryNode.SelectSingleNode("Width").InnerText);
        _height = int.Parse(inventoryNode.SelectSingleNode("Height").InnerText);

        _inventoryGrid = new InventoryGrid(_width, _height);
        XmlNodeList itemListNode = inventoryNode.SelectNodes("Item");
        for (int i = 0; i < itemListNode.Count; i++)
        {
            InventoryItem item = new InventoryItem(this, itemListNode.Item(i));
            Items.Add(item);
            if(!_inventoryGrid.AddItem(item.Slot, item))
            {
                Debug.LogError("Could not load inventory item into specified slot");
            }
        }
    }

    public override XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("Inventory");
        element.AppendChild(XmlHelper.Attribute(doc, "Width", _width));
        element.AppendChild(XmlHelper.Attribute(doc, "Height", _height));
        foreach (var item in Items)
        {
            element.AppendChild(item.ToXml(doc));
        }

        element.AppendChild(base.ToXml(doc));
        return element;
    }

    public void InitInventory(List<string> skills, Equipment equipment, bool autoEquip)
    {
        Items = new List<InventoryItem>();
        bool success = true;

        InventoryItem ring = ItemDatabase.Instance.GetInventoryItem("ring_1", TreasureLevel.L2, this);
        success = AttemptAdd(ring);
        ring.TryIdentify(100000);

        foreach (var id in skills)
        {
            Skill skill = SkillDatabase.Instance.GetSkill(id);
            InventoryItem item = ItemDatabase.Instance.SearchByStartingSkill(skill, this);
            if (item != null && item.Data.ID.Contains("herb"))
            {
                InventoryItem bottle = ItemDatabase.Instance.GetInventoryItem("bottle_empty", TreasureLevel.L1, this);
                success = AttemptAdd(bottle);
            }
            if (autoEquip && (item.Data is Weapon || item.Data is Armor))
                equipment.EquipItem(item);
            else
                AttemptAdd(item);

            item.TryIdentify(100000);
        }
    }

    bool AttemptAdd(InventoryItem item)
    {
        int slot = _inventoryGrid.AddItem(item);
        if (slot >= 0)
        {
            Items.Add(item);
            return true;
        }
        return false;
    }

    public InventoryItem CheckForItemAtSlot(int slot)
    {
        if (!_inventoryGrid.IsSlotOccupied(slot))
            return null;

        foreach(var item in Items)
        {
            if(item.OccupyingSlots.Contains(slot))
            {
                return item;
            }
        }

        Debug.LogError("Could not locate inventory item at occupied slot " + slot);
        return null;
    }

    public InventoryItem AddItem(InventoryItem item, int slot)
    {
        InventoryItem invItem = item;
        invItem.Reparent(this);

        if (!_inventoryGrid.AddItem(slot, invItem))
            return null;

        Items.Add(invItem);
        return invItem;
    }

    public InventoryItem AddItem(InventoryItem item)
    {
        InventoryItem invItem = item;
        invItem.Reparent(this);

        int slot = _inventoryGrid.AddItem(invItem);
        if (slot < 0)
            return null;

        Items.Add(invItem);
        return invItem;
    }

    public bool RemoveItem(InventoryItem item)
    {
        bool success = _inventoryGrid.RemoveItemFromSlot(item);
        if(success)
            Items.Remove(item);
        return success;
    }

    public bool TradeItem(InventoryItem heldItem, InventoryItem pickupItem)
    {
        int targetSlot = pickupItem.Slot;

        bool success = _inventoryGrid.RemoveItemFromSlot(pickupItem);
        if (!success)
            return false;

        success = _inventoryGrid.AddItem(targetSlot, heldItem);
        if(!success)
        {
            int slot = _inventoryGrid.AddItem(heldItem);
            success = slot >= 0;
        }

        if(success)
        {
            Items.Remove(pickupItem);
            Items.Add(heldItem);
        } else
        {
            _inventoryGrid.AddItem(targetSlot, pickupItem);
        }
        return success;
    }

    public bool DoesHaveItem(string ID)
    {
        foreach(var item in Items)
        {
            if (item.Data.ID == ID)
                return true;
        }
        return false;
    }
}
