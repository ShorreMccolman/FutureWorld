using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnchantmentType
{
    Standard,
    A,
    B,
    C,
    D
}

[System.Serializable]
public struct EnchantmentChances
{
    public int Weapon;
    public int Ranged;

    public int Body;
    public int Shield;
    public int Helm;
    public int Belt;
    public int Cloak;
    public int Gauntlet;
    public int Boots;
    public int Ring;
    public int Amulet;

    public int ChanceForSlot(EquipSlot slot, bool isWeapon)
    {
        switch(slot)
        {
            case EquipSlot.Primary:
                return Weapon;
            case EquipSlot.Ranged:
                return Ranged;
            case EquipSlot.Secondary:
                if (isWeapon)
                    return Weapon;
                else
                    return Shield;

            case EquipSlot.Amulet:
                return Amulet;
            case EquipSlot.Belt:
                return Belt;
            case EquipSlot.Body:
                return Body;
            case EquipSlot.Boots:
                return Boots;
            case EquipSlot.Cloak:
                return Cloak;
            case EquipSlot.Gauntlets:
                return Gauntlet;
            case EquipSlot.Helmet:
                return Helm;
            case EquipSlot.Ring:
                return Ring;
            case EquipSlot.TwoHanded:
                return Weapon;
        }
        return 0;
    }
}

public enum EnchantmentEffectOption
{
    AllResistances,
    AllStats,

    BonusHP,
    BonusSP,
    BonusMight,
    BonusEndurance,
    BonusSpeed,
    BonusAccuracy,
    BonusPersonality,
    BonusIntellect,
    BonusLuck,
    BonusAC,

    ColdDamage,

    DamageDemon,
    DamageDragon,

    DisarmSteal,

    ElectricDamage,

    FireDamage,

    Explodes,

    ImmuneDisease,
    ImmuneInsanity,
    ImmuneParalize,
    ImmunePoison,
    ImmuneSleep,
    ImmuneStone,

    Knockback,

    OnHitRecovery,
    OnHitMissileReduction,

    PoisonDamage,

    WeaponRecovery,

    RegenHP,
    RegenMP,

    ResistanceFire,
    ResistanceElec,
    ResistanceCold,
    ResistancePoison,

    SpellEffectAir,
    SpellEffectFire,
    SpellEffectWater,
    SpellEffectEarth,
    SpellEffectMind,
    SpellEffectBody,
    SpellEffectSpirit,
    SpellEffectLight,
    SpellEffectDark,

    Value,

    Vampire,

    BonusLevel,
}

[System.Serializable]
public class EnchantmentEffectPair
{
    public EnchantmentEffectOption Option;
    public int[] StrengthRange;
}

[System.Serializable]
public class EnchantmentData
{
    public string ID;
    public string DisplayName;
    public string Description;
    public EnchantmentType Type;
    public EnchantmentChances Chances;
    public int ValueBonus;
    public int ValueMulitplier;
    public List<EnchantmentEffectPair> Effects;
}

public class EnchantmentDatabase
{
    Dictionary<string, EnchantmentData> _enchantDict;
    Dictionary<EnchantmentType, List<EnchantmentData>> _enchantTypeDict;

    public EnchantmentDatabase()
    {
        _enchantDict = new Dictionary<string, EnchantmentData>();
        _enchantTypeDict = new Dictionary<EnchantmentType, List<EnchantmentData>>();

        EnchantmentDBObject[] enchantDBObjects = Resources.LoadAll<EnchantmentDBObject>("Database");
        if (enchantDBObjects == null)
        {
            Debug.LogError("Failed to load any Enchantment DB");
            return;
        }

        foreach (var obj in enchantDBObjects)
        {
            foreach (var enchant in obj.GetEnchantments()) {
                _enchantDict.Add(enchant.ID, enchant);

                if (!_enchantTypeDict.ContainsKey(enchant.Type))
                {
                    List<EnchantmentData> data = new List<EnchantmentData>();
                    data.Add(enchant);
                    _enchantTypeDict.Add(enchant.Type, data);
                }
                else
                {
                    _enchantTypeDict[enchant.Type].Add(enchant);
                }
            }
        }
    }

    public EnchantmentData GetEnchantmentData(string ID)
    {
        if (!_enchantDict.ContainsKey(ID))
        {
            return null;
        }

        return _enchantDict[ID];
    }

    public bool ShouldEnchant(Item item)
    {
        if (!(item is Weapon) && !(item is Armor))
            return false;

        return true;
    }

    public Enchantment GetRandomEnchantment(GameStateEntity parent, Weapon weapon, TreasureLevel level)
    {
        double chance = GameConstants.StandardWeaponEnchantmentChancesForItemLevel[level];
        if (Random.Range(0f, 1f) < chance)
        {
            EnchantmentData data = GetRandomizedEnchantment(new EnchantmentType[] { EnchantmentType.Standard }, weapon.EquipSlot, true);
            if (data == null)
                return null;

            return new Enchantment(parent, level, data);
        }
        return null;
    }

    public Enchantment GetRandomEnchantment(GameStateEntity parent, Armor armor, TreasureLevel level)
    {
        double chance = GameConstants.StandardArmorEnchantmentChancesForItemLevel[level];
        double specChance = GameConstants.SpecialArmorEnchantmentChancesForItemLevel[level];

        double rand = Random.Range(0f, 1f);

        if(rand < chance)
        {
            EnchantmentData data = GetRandomizedEnchantment(new EnchantmentType[] { EnchantmentType.Standard }, armor.EquipSlot, false);
            if (data == null)
                return null;

            return new Enchantment(parent, level, data);
        }
        else if (rand < chance + specChance)
        {
            EnchantmentData data = GetRandomizedEnchantment(GameConstants.EnchantmentTypesForItemlevel[level], armor.EquipSlot, false);
            if (data == null)
                return null;

            return new Enchantment(parent, level, data);
        }

        return null;
    }

    EnchantmentData GetRandomizedEnchantment(EnchantmentType[] types, EquipSlot slot, bool isWeapon)
    {
        List<EnchantmentData> possible = new List<EnchantmentData>();
        foreach (var type in types)
        {
            possible.AddRange(_enchantTypeDict[type]);
        }

        int max = 0;
        foreach(var pos in possible)
        {
            max += pos.Chances.ChanceForSlot(slot, isWeapon);
        }

        int roll = Random.Range(0, max);
        int current = 0;
        foreach(var pos in possible)
        {
            int chance = pos.Chances.ChanceForSlot(slot, isWeapon);
            if (chance == 0)
                continue;

            if (roll >= current && roll < current + chance)
                return pos;

            current += chance;
        }

        return null;
    }
}
