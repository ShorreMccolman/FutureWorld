using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class Equipment : GameStateEntity
{
    public InventoryItem Ring0 { get; private set; }
    public InventoryItem Ring1 { get; private set; }
    public InventoryItem Ring2 { get; private set; }
    public InventoryItem Ring3 { get; private set; }
    public InventoryItem Ring4 { get; private set; }
    public InventoryItem Ring5 { get; private set; }

    public Dictionary<EquipSlot, InventoryItem> Weapons { get; private set; }
    public Dictionary<EquipSlot, InventoryItem> Armor { get; private set; }

    public event System.Action OnEquipmentChanged;

    public Equipment(GameStateEntity parent, CharacterData data) : base(parent)
    {
        Weapons = new Dictionary<EquipSlot, InventoryItem>();
        Armor = new Dictionary<EquipSlot, InventoryItem>();

        if (!data.AutoEquip)
        {
            InventoryItem club = ItemDatabase.Instance.GetInventoryItem("club_1", TreasureLevel.L1, this);
            EquipItem(club);
        }
    }

    public Equipment(GameStateEntity parent, XmlNode node) : base(parent, node)
    {
        Weapons = new Dictionary<EquipSlot, InventoryItem>();
        Armor = new Dictionary<EquipSlot, InventoryItem>();

        XmlNode equipmentNode = node.SelectSingleNode("Equipment");
        XmlNode weaponsNode = equipmentNode.SelectSingleNode("Weapons");
        XmlNodeList weaponsListNode = weaponsNode.SelectNodes("Item");
        for (int i = 0; i < weaponsListNode.Count; i++)
        {
            InventoryItem item = new InventoryItem(this, weaponsListNode.Item(i));
            Weapons.Add((item.Data as Weapon).EquipSlot, item);
        }

        XmlNode armorNode = equipmentNode.SelectSingleNode("Armor");
        XmlNodeList armorListNode = armorNode.SelectNodes("Item");
        for (int i = 0; i < armorListNode.Count; i++)
        {
            InventoryItem item = new InventoryItem(this, armorListNode.Item(i));
            Armor.Add((item.Data as Armor).EquipSlot, item);
        }

        XmlNode ringNode = equipmentNode.SelectSingleNode("Ring0");
        if (ringNode.SelectSingleNode ("Item") != null)
        {
            InventoryItem item = new InventoryItem(this, ringNode.SelectSingleNode("Item"));
            Ring0 = item;
        }

        ringNode = equipmentNode.SelectSingleNode("Ring1");
        if (ringNode.SelectSingleNode("Item") != null)
        {
            InventoryItem item = new InventoryItem(this, ringNode.SelectSingleNode("Item"));
            Ring1 = item;
        }

        ringNode = equipmentNode.SelectSingleNode("Ring2");
        if (ringNode.SelectSingleNode("Item") != null)
        {
            InventoryItem item = new InventoryItem(this, ringNode.SelectSingleNode("Item"));
            Ring2 = item;
        }

        ringNode = equipmentNode.SelectSingleNode("Ring3");
        if (ringNode.SelectSingleNode("Item") != null)
        {
            InventoryItem item = new InventoryItem(this, ringNode.SelectSingleNode("Item"));
            Ring3 = item;
        }

        ringNode = equipmentNode.SelectSingleNode("Ring4");
        if (ringNode.SelectSingleNode("Item") != null)
        {
            InventoryItem item = new InventoryItem(this, ringNode.SelectSingleNode("Item"));
            Ring4 = item;
        }

        ringNode = equipmentNode.SelectSingleNode("Ring5");
        if (ringNode.SelectSingleNode("Item") != null)
        {
            InventoryItem item = new InventoryItem(this, ringNode.SelectSingleNode("Item"));
            Ring5 = item;
        }
    }

    public override XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("Equipment");
        XmlNode weapons = doc.CreateElement("Weapons");
        foreach (var item in Weapons.Values)
            weapons.AppendChild(item.ToXml(doc));
        element.AppendChild(weapons);

        XmlNode armor = doc.CreateElement("Armor");
        foreach (var item in Armor.Values)
            armor.AppendChild(item.ToXml(doc));
        element.AppendChild(armor);

        XmlNode ring0 = doc.CreateElement("Ring0");
        if (Ring0 != null)
            ring0.AppendChild(Ring0.ToXml(doc));
        element.AppendChild(ring0);

        XmlNode ring1 = doc.CreateElement("Ring1");
        if (Ring1 != null)
            ring1.AppendChild(Ring1.ToXml(doc));
        element.AppendChild(ring1);

        XmlNode ring2 = doc.CreateElement("Ring2");
        if (Ring2 != null)
            ring2.AppendChild(Ring2.ToXml(doc));
        element.AppendChild(ring2);

        XmlNode ring3 = doc.CreateElement("Ring3");
        if (Ring3 != null)
            ring3.AppendChild(Ring3.ToXml(doc));
        element.AppendChild(ring3);

        XmlNode ring4 = doc.CreateElement("Ring4");
        if (Ring4 != null)
            ring4.AppendChild(Ring4.ToXml(doc));
        element.AppendChild(ring4);

        XmlNode ring5 = doc.CreateElement("Ring5");
        if (Ring5 != null)
            ring5.AppendChild(Ring5.ToXml(doc));
        element.AppendChild(ring5);

        element.AppendChild(base.ToXml(doc));
        return element;
    }

    public InventoryItem RemoveAtSlot(EquipSlot slot)
    {
        if (Weapons.ContainsKey(slot))
        {
            InventoryItem item = Weapons[slot];
            Weapons.Remove(slot);
            OnEquipmentChanged?.Invoke();
            return item;
        }

        if (Armor.ContainsKey(slot))
        {
            InventoryItem item = Armor[slot];
            Armor.Remove(slot);
            OnEquipmentChanged?.Invoke();
            return item;
        }

        return null;
    }

    public InventoryItem ReplaceRing(InventoryItem item, int slot)
    {
        Armor armor = item.Data as Armor;
        if (armor != null)
        {
            InventoryItem returned = null;
            if (slot == 0)
                returned = Ring0;
            else if (slot == 1)
                returned = Ring1;
            else if (slot == 2)
                returned = Ring2;
            else if (slot == 3)
                returned = Ring3;
            else if (slot == 4)
                returned = Ring4;
            else if (slot == 5)
                returned = Ring5;
            EquipRing(item, slot);
            return returned;
        }

        return null;
    }

    public bool CanEquip(InventoryItem item)
    {
        Weapon weapon = item.Data as Weapon;
        if (weapon != null)
        {
            if(weapon.EquipSlot != EquipSlot.TwoHanded)
                return true;

            return !IsBothHandsOccupied();
        }

        Armor armor = item.Data as Armor;
        if (armor != null)
        {
            if (armor.EquipSlot != EquipSlot.Secondary)
                return true;

            return !IsBothHandsOccupied();
        }

        return false;
    }

    public InventoryItem Replace(InventoryItem item, bool asSecondary = false)
    {
        Weapon weapon = item.Data as Weapon;
        if (weapon != null)
        {
            bool secondary = asSecondary;

            EquipSlot slot = weapon.EquipSlot;
            if (weapon.EquipSlot == EquipSlot.TwoHanded)
            {
                secondary = false;
                if (Weapons.ContainsKey(EquipSlot.TwoHanded))
                    slot = EquipSlot.TwoHanded;
                else if (Weapons.ContainsKey(EquipSlot.Primary))
                    slot = EquipSlot.Primary;
                else if (Weapons.ContainsKey(EquipSlot.Secondary))
                    slot = EquipSlot.Secondary;
                else if (Armor.ContainsKey(EquipSlot.Secondary))
                    slot = EquipSlot.Secondary;
            }
            else if (Weapons.ContainsKey(EquipSlot.TwoHanded))
            {
                slot = EquipSlot.TwoHanded;
            }
            else if (asSecondary)
            {
                slot = EquipSlot.Secondary;
            }

            InventoryItem returned = RemoveAtSlot(slot);

            EquipItem(item, secondary);
            return returned;
        }

        Armor armor = item.Data as Armor;
        if (armor != null)
        {
            EquipSlot slot = armor.EquipSlot;
            if (armor.EquipSlot == EquipSlot.Secondary && Weapons.ContainsKey(EquipSlot.TwoHanded))
            {
                slot = EquipSlot.TwoHanded;
            }

            InventoryItem returned = RemoveAtSlot(slot);
            EquipItem(item);
            return returned;
        }

        return null;
    }

    public bool EquipRing(InventoryItem item, int slot)
    {
        if (item == null)
            return false;

        Armor ring = item.Data as Armor;
        if (ring == null)
            return false;

        if (ring.EquipSlot != EquipSlot.Ring)
            return false;

        if (slot == 0)
        {
            if (Ring0 != null)
                return false;

            Ring0 = item;
        }
        else if (slot == 1)
        {
            if (Ring1 != null)
                return false;

            Ring1 = item;
        }
        else if (slot == 2)
        {
            if (Ring2 != null)
                return false;

            Ring2 = item;
        }
        else if (slot == 3)
        {
            if (Ring3 != null)
                return false;

            Ring3 = item;
        }
        else if (slot == 4)
        {
            if (Ring4 != null)
                return false;

            Ring4 = item;
        }
        else if (slot == 5)
        {
            if (Ring5 != null)
                return false;

            Ring5 = item;
        }
        else
        {
            return false;
        }

        OnEquipmentChanged?.Invoke();
        return true;
    }

    public bool EquipItem(InventoryItem item, bool asSecondary = false)
    {
        if (item == null)
            return false;

        if (item.Data is Weapon)
        {
            Weapon weapon = item.Data as Weapon;
            if (asSecondary)
            {
                if (Weapons.ContainsKey(EquipSlot.Secondary))
                    return false;

                Weapons.Add(EquipSlot.Secondary, item);
            }
            else
            {
                if (Weapons.ContainsKey(weapon.EquipSlot))
                    return false;

                Weapons.Add(weapon.EquipSlot, item);
            }

            OnEquipmentChanged?.Invoke();
            return true;
        }

        if(item.Data is Armor)
        {
            Armor armor = item.Data as Armor;

            if(armor.EquipSlot == EquipSlot.Ring)
            {
                if (Ring0 == null)
                {
                    Ring0 = item;
                }
                else if (Ring1 == null)
                {
                    Ring1 = item;
                }
                else if (Ring2 == null)
                {
                    Ring2 = item;
                }
                else if (Ring3 == null)
                {
                    Ring3 = item;
                }
                else if (Ring4 == null)
                {
                    Ring4 = item;
                }
                else if (Ring5 == null)
                {
                    Ring5 = item;
                }
                else
                {
                    return false;
                }
                OnEquipmentChanged?.Invoke();
                return true;
            }

            if (Armor.ContainsKey(armor.EquipSlot))
                return false;

            Armor.Add(armor.EquipSlot, item);
            OnEquipmentChanged?.Invoke();
            return true;
        }

        return false;
    }

    public bool RemoveItem(InventoryItem item, EquipSlot slot)
    {
        if (item == null)
            return false;

        if (item.Data is Weapon)
        {
            Weapon weapon = item.Data as Weapon;
            if (!Weapons.ContainsKey(slot))
                return false;

            Weapons.Remove(slot);
            OnEquipmentChanged?.Invoke();
            return true;
        }

        if (item.Data is Armor)
        {
            Armor armor = item.Data as Armor;

            if (armor.EquipSlot == EquipSlot.Ring)
            {
                if (Ring0 == item)
                {
                    Ring0 = null;
                }
                else if (Ring1 == item)
                {
                    Ring1 = null;
                }
                else if (Ring2 == item)
                {
                    Ring2 = null;
                }
                else if (Ring3 == item)
                {
                    Ring3 = null;
                }
                else if (Ring4 == item)
                {
                    Ring4 = null;
                }
                else if (Ring5 == item)
                {
                    Ring5 = null;
                }
                else
                {
                    return false;
                }
                OnEquipmentChanged?.Invoke();
                return true;
            }

            if (!Armor.ContainsKey(armor.EquipSlot))
                return false;

            Armor.Remove(armor.EquipSlot);
            OnEquipmentChanged?.Invoke();
            return true;
        }

        return false;
    }

    public int GetArmorClass(Skillset set)
    {
        int ac = 0;

        if (Weapons.ContainsKey(EquipSlot.Primary))
        {
            InventoryItem armor = Weapons[EquipSlot.Primary];
            Weapon data = armor.Data as Weapon;
            ac += set.GetArmorClass(data.Type);
        }

        if (Weapons.ContainsKey(EquipSlot.TwoHanded))
        {
            InventoryItem armor = Weapons[EquipSlot.TwoHanded];
            Weapon data = armor.Data as Weapon;
            ac += set.GetArmorClass(data.Type);
        }

        if (Armor.ContainsKey(EquipSlot.Helmet))
        {
            InventoryItem helmet = Armor[EquipSlot.Helmet];
            Armor data = helmet.Data as Armor;
            ac += data.AC;
        }

        if(Armor.ContainsKey(EquipSlot.Body))
        {
            InventoryItem armor = Armor[EquipSlot.Body];
            Armor data = armor.Data as Armor;
            ac += set.GetArmorClass(data.Type) + data.AC;
        }

        if (Armor.ContainsKey(EquipSlot.Secondary))
        {
            InventoryItem armor = Armor[EquipSlot.Secondary];
            Armor data = armor.Data as Armor;
            ac += set.GetArmorClass(data.Type) + data.AC;
        }

        if (Armor.ContainsKey(EquipSlot.Belt))
        {
            InventoryItem armor = Armor[EquipSlot.Belt];
            Armor data = armor.Data as Armor;
            ac += data.AC;
        }

        if (Armor.ContainsKey(EquipSlot.Boots))
        {
            InventoryItem armor = Armor[EquipSlot.Boots];
            Armor data = armor.Data as Armor;
            ac += data.AC;
        }

        if (Armor.ContainsKey(EquipSlot.Cloak))
        {
            InventoryItem armor = Armor[EquipSlot.Cloak];
            Armor data = armor.Data as Armor;
            ac += data.AC;
        }

        if (Armor.ContainsKey(EquipSlot.Gauntlets))
        {
            InventoryItem armor = Armor[EquipSlot.Gauntlets];
            Armor data = armor.Data as Armor;
            ac += data.AC;
        }

        if (Armor.ContainsKey(EquipSlot.Amulet))
        {
            InventoryItem armor = Armor[EquipSlot.Amulet];
            Armor data = armor.Data as Armor;
            ac += data.AC;
        }

        if(Ring0 != null)
        {
            Armor data = Ring0.Data as Armor;
            ac += data.AC;
        }
        if (Ring1 != null)
        {
            Armor data = Ring1.Data as Armor;
            ac += data.AC;
        }
        if (Ring2 != null)
        {
            Armor data = Ring2.Data as Armor;
            ac += data.AC;
        }
        if (Ring3 != null)
        {
            Armor data = Ring3.Data as Armor;
            ac += data.AC;
        }
        if (Ring4 != null)
        {
            Armor data = Ring4.Data as Armor;
            ac += data.AC;
        }
        if (Ring5 != null)
        {
            Armor data = Ring5.Data as Armor;
            ac += data.AC;
        }

        return ac;
    }

    public bool IsTwoHanding()
    {
        return Weapons.ContainsKey(EquipSlot.TwoHanded);
    }

    public bool IsShielding()
    {
        return Armor.ContainsKey(EquipSlot.Secondary);
    }

    public bool IsBothHandsOccupied()
    {
        bool left = Weapons.ContainsKey(EquipSlot.Primary);
        bool right = Weapons.ContainsKey(EquipSlot.Secondary) || Armor.ContainsKey(EquipSlot.Secondary);

        return left && right;
    }

    public bool HasRangedWeapon()
    {
        return Weapons.ContainsKey(EquipSlot.Ranged);
    }

    public int GetAttack(Skillset set)
    {
        int ac = 0;

        if (Weapons.ContainsKey(EquipSlot.Primary))
        {
            InventoryItem weapon = Weapons[EquipSlot.Primary];
            Weapon data = weapon.Data as Weapon;
            ac += set.GetAttack(data.Type);
        }
        else if (Weapons.ContainsKey(EquipSlot.TwoHanded))
        {
            InventoryItem weapon = Weapons[EquipSlot.TwoHanded];
            Weapon data = weapon.Data as Weapon;
            ac += set.GetAttack(data.Type);
        }

        return ac;
    }

    public int RollDamage(Skillset set)
    {
        int ac = 0;

        if (Weapons.ContainsKey(EquipSlot.Primary))
        {
            InventoryItem weapon = Weapons[EquipSlot.Primary];
            Weapon data = weapon.Data as Weapon;

            ac += set.GetDamage(data.Type) + data.BaseDamage + data.DamageRoll.Roll();
            ac = set.DamageModifier(data.Type, ac);
        }
        else if (Weapons.ContainsKey(EquipSlot.TwoHanded))
        {
            InventoryItem weapon = Weapons[EquipSlot.TwoHanded];
            Weapon data = weapon.Data as Weapon;

            ac += set.GetDamage(data.Type) + data.BaseDamage + data.DamageRoll.Roll();
            ac = set.DamageModifier(data.Type, ac);
        }

        return ac;
    }

    public int GetDamageUpper(Skillset set)
    {
        int ac = 0;

        if (Weapons.ContainsKey(EquipSlot.Primary))
        {
            InventoryItem weapon = Weapons[EquipSlot.Primary];
            Weapon data = weapon.Data as Weapon;
            ac += set.GetDamage(data.Type) + data.BaseDamage + data.DamageRoll.RollHigh();
        }
        else if (Weapons.ContainsKey(EquipSlot.TwoHanded))
        {
            InventoryItem weapon = Weapons[EquipSlot.TwoHanded];
            Weapon data = weapon.Data as Weapon;

            ac += set.GetDamage(data.Type) + data.BaseDamage + data.DamageRoll.RollHigh();
        }

        return ac;
    }

    public int GetDamageLower(Skillset set)
    {
        int ac = 0;

        if (Weapons.ContainsKey(EquipSlot.Primary))
        {
            InventoryItem weapon = Weapons[EquipSlot.Primary];
            Weapon data = weapon.Data as Weapon;
            ac += set.GetDamage(data.Type) + data.BaseDamage + data.DamageRoll.RollLow();
        }
        else if (Weapons.ContainsKey(EquipSlot.TwoHanded))
        {
            InventoryItem weapon = Weapons[EquipSlot.TwoHanded];
            Weapon data = weapon.Data as Weapon;

            ac += set.GetDamage(data.Type) + data.BaseDamage + data.DamageRoll.RollLow();
        }

        return ac;
    }

    public int GetRangedAttack(Skillset set)
    {
        int ac = 0;

        if (Weapons.ContainsKey(EquipSlot.Ranged))
        {
            InventoryItem weapon = Weapons[EquipSlot.Ranged];
            Weapon data = weapon.Data as Weapon;
            ac += set.GetAttack(data.Type) + data.BaseDamage;
        }

        return ac;
    }

    public int RollRangedDamage(Skillset set)
    {
        int ac = 0;

        if (Weapons.ContainsKey(EquipSlot.Ranged))
        {
            InventoryItem weapon = Weapons[EquipSlot.Ranged];
            Weapon data = weapon.Data as Weapon;

            ac += set.GetDamage(data.Type) + data.BaseDamage + data.DamageRoll.Roll();
        }

        return ac;
    }

    public int GetRangedDamageUpper(Skillset set)
    {
        int ac = 0;

        if (Weapons.ContainsKey(EquipSlot.Ranged))
        {
            InventoryItem weapon = Weapons[EquipSlot.Ranged];
            Weapon data = weapon.Data as Weapon;
            int damage = set.GetDamage(data.Type) + data.BaseDamage + data.DamageRoll.RollHigh();
            ac += Mathf.Max(damage, 1);
        }

        return ac;
    }

    public int GetRangedDamageLower(Skillset set)
    {
        int ac = 0;

        if (Weapons.ContainsKey(EquipSlot.Ranged))
        {
            InventoryItem weapon = Weapons[EquipSlot.Ranged];
            Weapon data = weapon.Data as Weapon;
            int damage = set.GetDamage(data.Type) + data.BaseDamage + data.DamageRoll.RollLow();
            ac += Mathf.Max(damage, 1);
        }

        return ac;
    }

    public int GetRecovery(Skillset set)
    {
        int ac = 100;
        InventoryItem item = SlowestRecoveryWeapon();
        if(item != null)
            ac = set.GetWeaponRecovery((item.Data as Weapon).Type);

        if (Armor.ContainsKey(EquipSlot.Body))
        {
            InventoryItem armor = Armor[EquipSlot.Body];
            if (armor.Data is Armor)
            {
                Armor data = armor.Data as Armor;
                ac += set.GetRecovery(data.Type);
            }
        }

        if(Armor.ContainsKey(EquipSlot.Secondary))
        {
            InventoryItem armor = Armor[EquipSlot.Secondary];
            if (armor.Data is Armor)
            {
                Armor data = armor.Data as Armor;
                ac += set.GetRecovery(data.Type);
            }
        }

        return ac;
    }

    public int GetRangedRecovery(Skillset set)
    {
        int ac = 100;

        if (Weapons.ContainsKey(EquipSlot.Ranged))
        {
            InventoryItem weapon = Weapons[EquipSlot.Ranged];
            Weapon data = weapon.Data as Weapon;
            ac = set.GetWeaponRecovery(data.Type);
        }

        if (Armor.ContainsKey(EquipSlot.Body))
        {
            InventoryItem armor = Armor[EquipSlot.Body];
            if (armor.Data is Armor)
            {
                Armor data = armor.Data as Armor;
                ac += set.GetRecovery(data.Type);
            }
        }

        if (Armor.ContainsKey(EquipSlot.Secondary))
        {
            InventoryItem armor = Armor[EquipSlot.Secondary];
            if (armor.Data is Armor)
            {
                Armor data = armor.Data as Armor;
                ac += set.GetRecovery(data.Type);
            }
        }

        return ac;
    }

    InventoryItem SlowestRecoveryWeapon()
    {
        EquipSlot firstSlot = EquipSlot.Primary;
        Weapon first = null, second = null;
        if (Weapons.ContainsKey(EquipSlot.Primary))
        {
            InventoryItem weapon = Weapons[EquipSlot.Primary];
            first = weapon.Data as Weapon;
        }
        if(Weapons.ContainsKey(EquipSlot.TwoHanded))
        {
            InventoryItem weapon = Weapons[EquipSlot.TwoHanded];
            first = weapon.Data as Weapon;
            firstSlot = EquipSlot.TwoHanded;
        }
        if (Weapons.ContainsKey(EquipSlot.Secondary))
        {
            InventoryItem weapon = Weapons[EquipSlot.Secondary];
            second = weapon.Data as Weapon;
        }
        if(first != null)
        {
            if(second == null)
            {
                return Weapons[firstSlot];
            }

            return GameConstants.RecoveryByWeaponType[first.Type] >= GameConstants.RecoveryByWeaponType[second.Type] ? Weapons[firstSlot] : Weapons[EquipSlot.Secondary];
        }
        return null;
    }

    public void ModifyStats(EffectiveStats stats)
    {

        
    }
}
