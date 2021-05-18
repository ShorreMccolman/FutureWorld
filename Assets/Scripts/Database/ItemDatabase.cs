using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using System.Linq;

public enum EquipSlot
{
    None,
    Helmet,
    Cloak,
    Amulet,
    Gauntlets,
    Boots,
    Body,
    Primary,
    TwoHanded,
    Secondary,
    Ranged,
    Ring,
    Belt
}

public enum TreasureLevel
{
    L1,
    L2,
    L3,
    L4,
    L5,
    L6,
    Artifact
}

[System.Serializable]
public struct ItemChances
{
    public int L1;
    public int L2;
    public int L3;
    public int L4;
    public int L5;
    public int L6;

    public int Chance(TreasureLevel level)
    {
        switch(level)
        {
            case TreasureLevel.L1:
                return L1;
            case TreasureLevel.L2:
                return L2;
            case TreasureLevel.L3:
                return L3;
            case TreasureLevel.L4:
                return L4;
            case TreasureLevel.L5:
                return L5;
            case TreasureLevel.L6:
                return L6;
        }
        return 0;
    }
}

[System.Serializable]
public class Item
{
    public string ID;
    public string DisplayName;
    [TextArea(minLines:2,maxLines:15)]
    public string Description;
    public ItemChances Chances;
    public int Level;
    public int Value;
    public int Width = 1;
    public int Height = 1;
    public string startWithSkill;
    public Sprite sprite;
    public Sprite equipSprite;
    public GameObject model;
    public Vector3 EquipOffset;

    public virtual string GetTypeDescription() 
    {
        if (ID.Contains("key"))
            return "Key";

        return "Description not implemented"; 
    }
    public virtual string GetItemDescription() { return Description; }
}

[System.Serializable]
public class Gold : Item
{
    public override string GetTypeDescription()
    {
        return "Gold Coins";
    }
}

[System.Serializable]
public class Scroll : Item
{
    public string Date;

    public override string GetTypeDescription()
    {
        return "Message Scroll";
    }

    public override string GetItemDescription()
    {
        return "A message scroll. To read this message, pick the scroll up and left-click over the character picture in the inventory screen.";
    }
}

public enum ConsumeEffect
{
    None,
    RestoreHP,
    RestoreMP,

    BoostStats,
    BoostAC,
    BoostResistance,

    SkillPoints,
    
    CurePoison,
    Poison
}

[System.Serializable]
public class Consumable : Item
{
    public ConsumeEffect ConsumeEffect;
    public int Potency;

    public string[] Recipe = new string[2];

    public override string GetTypeDescription()
    {
        if (ID.Contains("herb"))
        {
            return "Herb";
        }
        else if (ID.Contains("shoe"))
        {
            return "Horseshoe";
        }

        if (ID.Contains("bottle"))
        {
            if (ID.Contains("empty"))
            {
                return "Potion Bottle";
            }
            else if (ID.Contains("red"))
            {
                return "Red Potion";
            }
            else if (ID.Contains("blue"))
            {
                return "Blue Potion";
            }
            else if (ID.Contains("yellow"))
            {
                return "Yellow Potion";
            }
            else if (ID.Contains("purple"))
            {
                return "Purple Potion";
            }
            else if (ID.Contains("orange"))
            {
                return "Orange Potion";
            }
            else if (ID.Contains("green"))
            {
                return "Green Potion";
            }
            else if (ID.Contains("black"))
            {
                return "Black Potion";
            }
            else if (ID.Contains("white"))
            {
                return "White Potion";
            }
        }

        return "Unknown Item Type";
    }

    public override string GetItemDescription()
    {
        if (ID.Contains("herb"))
        {
            return "A magical herb of unusual properties. " + DisplayName + " can be used to make potions. (To use, pick the herb up and right-click over an empty potion bottle.)";
        }
        else if (ID.Contains("shoe"))
        {
            return Description + " To use, pick the horseshoe up and right-click over a character's portrait.";
        }
        else if (ID.Contains("bottle"))
        {
            if (ID.Contains("empty"))
                return Description;

            return Description + " To drink, pick the potion up and right-click over a character's portrait. To mix, pick the potion up and right-click over another potion.";
        }

        return "Unknown Item Type";
    }
}

public enum WeaponType
{
    Club,
    Axe,
    Blaster,
    Bow,
    Dagger,
    Mace,
    Spear,
    Staff,
    Sword
}

[System.Serializable]
public class Weapon : Item
{
    public EquipSlot EquipSlot;
    public WeaponType Type;
    public DiceRoll DamageRoll;
    public int BaseDamage;

    public Weapon()
    {
        EquipSlot = EquipSlot.Primary;
        Type = WeaponType.Club;
        DamageRoll = new DiceRoll(1, 1);
        BaseDamage = 0;
    }

    public override string GetTypeDescription()
    {
        if (Type == WeaponType.Blaster)
            return "Ancient Weapon";

        return Type.ToString();
    }
}

public enum ArmorType
{
    Generic,
    Leather,
    Chain,
    Plate,
    Shield
}

[System.Serializable]
public class Armor : Item
{
    public EquipSlot EquipSlot;
    public ArmorType Type;
    public int AC;

    public override string GetTypeDescription()
    {
        switch(Type)
        {
            case ArmorType.Chain:
                return "Chain Mail";
            case ArmorType.Leather:
                return "Leather Armor";
            case ArmorType.Shield:
                return "Shield";
            case ArmorType.Plate:
                return "Plate Mail";
            case ArmorType.Generic:
                switch(EquipSlot)
                {
                    case EquipSlot.Amulet:
                        return "Amulet";
                    case EquipSlot.Boots:
                        return "Boots";
                    case EquipSlot.Cloak:
                        return "Cloak";
                    case EquipSlot.Helmet:
                        return "Helmet";
                    case EquipSlot.Gauntlets:
                        return "Gauntlets";
                    case EquipSlot.Ring:
                        return "Ring";
                    case EquipSlot.Belt:
                        return "Belt";
                }
                break;
        }

        return "Unknown Item Type";
    }
}

[System.Serializable]
public class Magic : Item
{
    public SkillProficiency Requirement;
    public SpellSchool Type;
    public int SpellNumber;

    public override string GetTypeDescription()
    {
        return "Book of Learning";
    }

    public override string GetItemDescription()
    {
        return "A spell book. To learn this spell, pick the book up and left-click over the picture of your character in the inventory screen. Your character must know the " + Type.ToString() + " magic skill to learn this spell.";
    }
}

public class IngredientPair
{
    public string Result;
    public List<string> Ingredients;

    public IngredientPair(Consumable consumable)
    {
        Result = consumable.ID;
        Ingredients = new List<string>();
        foreach (var ing in consumable.Recipe)
            Ingredients.Add(ing);
    }
}

public class ItemDatabase {
    public static ItemDatabase Instance;

    public EnchantmentDatabase EnchantDB { get; private set; }

    Dictionary<string, Item> _itemDict;
    Dictionary<EnchantmentType, List<EnchantmentData>> _treasureLevelDict;
    Dictionary<string, IngredientPair> _recipeDict;

    public ItemDatabase()
    {
        Instance = this;

        _itemDict = new Dictionary<string, Item>();

        ItemDBObject[] itemDBObjects = Resources.LoadAll<ItemDBObject>("Database/Items");
        if(itemDBObjects.Length == 0)
        {
            Debug.LogError("Failed to load ItemDB");
            return;
        }

        List<Consumable> consumables = new List<Consumable>();
        foreach (var itemdb in itemDBObjects)
        {
            foreach (var item in itemdb.GetItems())
            {
                if (item.model == null)
                    item.model = itemdb.defaultModel;
                _itemDict.Add(item.ID, item);

                if(item is Consumable)
                {
                    consumables.Add(item as Consumable);
                }
            }
        }
        BuildRecipeList(consumables);

        EnchantDB = new EnchantmentDatabase();
    }

    void BuildRecipeList(List<Consumable> consumables)
    {
        _recipeDict = new Dictionary<string, IngredientPair>();
        foreach (var consumable in consumables) {
            if (!_recipeDict.Keys.Contains(consumable.ID))
            {
                if(consumable.Recipe.Length == 2)
                {
                    _recipeDict.Add(consumable.ID, new IngredientPair(consumable));
                }
            }
        }
    }

    public List<InventoryItem> TryCombine(Consumable A, Consumable B)
    {
        List<InventoryItem> items = new List<InventoryItem>();
        IngredientPair pair = _recipeDict.Values.ToList().Find(x => x.Ingredients.Contains(A.ID) && x.Ingredients.Contains(B.ID));
        if(pair != null)
        {
            items.Add(new InventoryItem(null, GetItem(pair.Result), TreasureLevel.L1));
            if(A.ID.Contains("bottle") && B.ID.Contains("bottle"))
            {
                items.Add(new InventoryItem(null, GetItem("bottle_empty"), TreasureLevel.L1));
            }
        }
        return items;
    }

    public InventoryItem SearchByStartingSkill(Skill skill, Inventory inventory)
    {
        if (skill.Type == SkillType.Misc)
        {
            string key = "";
            int rand = Random.Range(0, 3);
            if (rand == 0)
                key = "herb_red";
            else if (rand == 1)
                key = "herb_blue";
            else if (rand == 2)
                key = "herb_yellow";
            if (_itemDict.ContainsKey(key))
                return new InventoryItem(inventory, _itemDict[key], TreasureLevel.L1);
        }
        else
        {
            foreach (var item in _itemDict.Values)
            {
                if (item.startWithSkill == skill.ID)
                    return new InventoryItem(inventory, item, TreasureLevel.L1);
            }
        }

        return null;
    }

    public Queue<InventoryItem> ItemQueueBySpawnQuantities(SpawnQuantities quantities)
    {
        Queue<InventoryItem> spawnItems = new Queue<InventoryItem>();

        for (int i = 0; i < quantities.Artifact; i++)
        {
            InventoryItem item = new InventoryItem(null, GetArtifact(), TreasureLevel.Artifact);
            spawnItems.Enqueue(item);
        }

        for (int i = 0; i < quantities.Custom.Length; i++)
        {
            InventoryItem item = new InventoryItem(null, GetItem(quantities.Custom[i]), TreasureLevel.Artifact);
            spawnItems.Enqueue(item);
        }

        List<TreasureLevel> Levels = new List<TreasureLevel>();
        for (int i = 0; i < quantities.L1; i++)
            Levels.Add(TreasureLevel.L1);
        for (int i = 0; i < quantities.L2; i++)
            Levels.Add(TreasureLevel.L2);
        for (int i = 0; i < quantities.L3; i++)
            Levels.Add(TreasureLevel.L3);
        for (int i = 0; i < quantities.L4; i++)
            Levels.Add(TreasureLevel.L4);
        for (int i = 0; i < quantities.L5; i++)
            Levels.Add(TreasureLevel.L5);
        for (int i = 0; i < quantities.L6; i++)
            Levels.Add(TreasureLevel.L6);

        List<TreasureLevel> additionalLevels = new List<TreasureLevel>();
        for (int j = 0; j < Levels.Count; j++)
        {
            int additionalItems = Random.Range(0, 5);
            for (int i = 0; i < additionalItems; i++)
                additionalLevels.Add(quantities.GetValidTreasureLevel());
        }
        Levels.AddRange(additionalLevels);

        for(int i=0;i<Levels.Count;i++)
        {
            TreasureLevel level = Levels[i];
            float rand = Random.Range(0f, 1f);
            if(rand < 0.2)
            {
                continue;
            }
            else if (rand < .8)
            {
                InventoryItem gold = new InventoryItem(null, GetItem(GameConstants.GoldDropIDForItemLevel[level]), level);
                spawnItems.Enqueue(gold);
                continue;
            }

            InventoryItem item = new InventoryItem(null, GetItemByTreasureLevel(level), level);
            spawnItems.Enqueue(item);
        }

        return spawnItems;
    }

    public Item GetArtifact()
    {
        //List<Item> possible = new List<Item>();
        //foreach (var item in _itemDict.Values)
        //    if (item.IsArtifact)
        //        possible.Add(item);

        return null;
    }

    public Item GetItemByTreasureLevel(TreasureLevel level)
    {
        List<Item> possible = new List<Item>();
        foreach (var item in _itemDict.Values)
        if(item.Chances.Chance(level) != 0)
            possible.Add(item);

        return GetItemFromWeightedList(possible, level);
    }

    public Item GetItemFromWeightedList(List<Item> items, TreasureLevel level)
    {
        int max = 0;
        foreach (var pos in items)
        {
            max += pos.Chances.Chance(level);
        }

        int roll = Random.Range(0, max);
        int current = 0;
        foreach (var pos in items)
        {
            int chance = pos.Chances.Chance(level);
            if (roll >= current && roll < current + chance)
                return pos;

            current += chance;
        }

        return null;
    }

    public InventoryItem GetProduct(List<GeneralItemType> Items, TreasureLevel level)
    {
        List<Item> possible = new List<Item>();
        foreach(var item in _itemDict.Values)
        {
            if (item.Chances.Chance(level) == 0)
                continue;

            if(Items.Contains(GeneralItemType.Herbs))
            {
                if (item.ID.Contains("herb") || item.ID == "bottle_empty")
                {
                    possible.Add(item);
                    continue;
                }
            }
            if(Items.Contains(GeneralItemType.Alchemy))
            {
                if (item.ID.Contains("bottle") && !item.ID.Contains("empty"))
                {
                    possible.Add(item);
                    continue;
                }
            }
            if(Items.Contains(GeneralItemType.Clothes))
            {
                if(item is Armor)
                {
                    Armor armor = item as Armor;
                    if(armor.EquipSlot == EquipSlot.Cloak || armor.EquipSlot == EquipSlot.Boots || armor.EquipSlot == EquipSlot.Belt)
                    {
                        possible.Add(item);
                        continue;
                    }
                }
            }
            if (Items.Contains(GeneralItemType.Armor))
            {
                if (item is Armor)
                {
                    Armor armor = item as Armor;
                    if (armor.EquipSlot == EquipSlot.Helmet || armor.EquipSlot == EquipSlot.Gauntlets)
                    {
                        possible.Add(item);
                        continue;
                    }
                }
            }
            if (Items.Contains(GeneralItemType.Jewelery))
            {
                if (item is Armor)
                {
                    Armor armor = item as Armor;
                    if (armor.EquipSlot == EquipSlot.Amulet || armor.EquipSlot == EquipSlot.Ring)
                    {
                        possible.Add(item);
                        continue;
                    }
                }
            }
            if (Items.Contains(GeneralItemType.Spells))
            {
                if (item is Magic)
                {
                    possible.Add(item);
                }
            }
        }

        return new InventoryItem(null, GetItemFromWeightedList(possible, level), level);
    }

    public InventoryItem GetProduct(List<SpellSchool> schools, List<TreasureLevel> levels)
    {
        List<Item> possible = new List<Item>();
        foreach (var item in _itemDict.Values)
        {
            bool hasChance = false;
            foreach (var level in levels)
            {
                if (item.Chances.Chance(level) > 0)
                    hasChance = true;
            }

            if (!hasChance)
                continue;

            if (item is Magic)
            { 
               Magic magic = item as Magic;

               if (schools.Contains(magic.Type))
               {
                   possible.Add(item);
                   continue;
               }
            }
        }

        int rand = Random.Range(0, possible.Count);

        return new InventoryItem(null, possible[rand], TreasureLevel.L1);
    }

    public InventoryItem GetProduct(List<WeaponType> Items, TreasureLevel level)
    {
        List<Item> possible = new List<Item>();
        foreach (var item in _itemDict.Values)
        {
            if (item.Chances.Chance(level) == 0)
                continue;

            Weapon weapon = item as Weapon;
            if (weapon == null)
                continue;

            if (Items.Contains(weapon.Type))
            {
                possible.Add(item);
                continue;
            }
        }

        return new InventoryItem(null, GetItemFromWeightedList(possible, level), level);
    }

    public InventoryItem GetProduct(List<ArmorType> Items, TreasureLevel level)
    {
        List<Item> possible = new List<Item>();
        foreach (var item in _itemDict.Values)
        {
            if (item.Chances.Chance(level) == 0)
                continue;

            Armor armor = item as Armor;
            if (armor == null)
                continue;

            if (Items.Contains(armor.Type))
            {
                possible.Add(item);
                continue;
            }
        }

        return new InventoryItem(null, GetItemFromWeightedList(possible, level), level);
    }

    public Item GetItem(string ID)
    {
        if (!_itemDict.ContainsKey(ID))
            return null;

        return _itemDict[ID];
    }

    public InventoryItem GetInventoryItem(string ID)
    {
        if (!_itemDict.ContainsKey(ID))
        {
            return null;
        }

        return new InventoryItem(null, _itemDict[ID], TreasureLevel.L1);
    }

    public InventoryItem GetInventoryItem(string ID, TreasureLevel level, GameStateEntity owner)
    {
        if (!_itemDict.ContainsKey(ID))
        {
            return null;
        }

        return new InventoryItem(owner, _itemDict[ID], level);
    }
}
