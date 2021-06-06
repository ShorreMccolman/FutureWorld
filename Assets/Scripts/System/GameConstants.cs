using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConstants {

    public static string[] RandomCharacterNamesMale = new string[]{
        "Holden", "Ashford", "Marco", "Amos",
        "Din", "Gideon", "Luke", "Boba",
        "Seath", "Oscar", "Gilligan", "Sullyvan"};

    public static string[] RandomCharacterNamesFemale = new string[]{
        "Julie", "Drummer", "Naomi", "Clarissa",
        "Fennec", "Cara", "Bo", "Morgan",
        "Pricilla", "Irena", "Kahlan" };

    public static string[] RandomNPCNamesMale = new string[]
    {
        "Alan",
        "Erik",
        "Toombs",
        "Bones",
        "Kirk",
        "Murdock",
        "Shux",
        "Hunter",
        "Strax",
        "Tim",
        "Tom",
        "Pip",
        "Pat",
        "Rickart",
        "Frederick",
        "Toby",
        "Mohammed"
    };

    public static string[] RandomNPCNamesFemale = new string[]
    {
        "Alex",
        "Shandra",
        "Sequoia",
        "Scarlet",
        "Erica",
        "Constance",
        "Debra",
        "Alice",
        "Missy",
        "Amanda",
        "Jenny",
        "Kerry",
        "Candi",
        "Monique",
        "Pam"
    };

    public static Dictionary<int, string> monthDict = new Dictionary<int, string>()
    { {1, "January" }, {2, "February" }, {3, "March" }, {4, "April" }, {5, "May" }, {6, "June" }, {7, "July" },
        {8, "August" }, {9, "September" }, {10, "October" }, {11, "November" }, {12,"December"} };

    public static Dictionary<CharacterClass, int> InitialMightForClass = new Dictionary<CharacterClass, int>()
    {
        {CharacterClass.Knight, 14 },
        {CharacterClass.Paladin, 14 },
        {CharacterClass.Archer, 9 },
        {CharacterClass.Druid, 7 },
        {CharacterClass.Cleric, 7 },
        {CharacterClass.Sorcerer, 7 }
    };

    public static Dictionary<CharacterClass, int> InitialIntellectForClass = new Dictionary<CharacterClass, int>()
    {
        {CharacterClass.Knight, 7 },
        {CharacterClass.Paladin, 7 },
        {CharacterClass.Archer, 14 },
        {CharacterClass.Druid, 14 },
        {CharacterClass.Cleric, 9 },
        {CharacterClass.Sorcerer, 14 }
    };

    public static Dictionary<CharacterClass, int> InitialPersonalityForClass = new Dictionary<CharacterClass, int>()
    {
        {CharacterClass.Knight, 7 },
        {CharacterClass.Paladin, 14 },
        {CharacterClass.Archer, 7 },
        {CharacterClass.Druid, 14 },
        {CharacterClass.Cleric, 14 },
        {CharacterClass.Sorcerer, 9 }
    };

    public static Dictionary<CharacterClass, int> InitialEnduranceForClass = new Dictionary<CharacterClass, int>()
    {
        {CharacterClass.Knight, 14 },
        {CharacterClass.Paladin, 11 },
        {CharacterClass.Archer, 11 },
        {CharacterClass.Druid, 11 },
        {CharacterClass.Cleric, 11 },
        {CharacterClass.Sorcerer, 11 }
    };

    public static Dictionary<CharacterClass, int> InitialAccuracyForClass = new Dictionary<CharacterClass, int>()
    {
        {CharacterClass.Knight, 11 },
        {CharacterClass.Paladin, 11 },
        {CharacterClass.Archer, 14 },
        {CharacterClass.Druid, 7 },
        {CharacterClass.Cleric, 11 },
        {CharacterClass.Sorcerer, 7 }
    };

    public static Dictionary<CharacterClass, int> InitialSpeedForClass = new Dictionary<CharacterClass, int>()
    {
        {CharacterClass.Knight, 11 },
        {CharacterClass.Paladin, 9 },
        {CharacterClass.Archer, 11 },
        {CharacterClass.Druid, 11 },
        {CharacterClass.Cleric, 7 },
        {CharacterClass.Sorcerer, 14 }
    };

    public static Dictionary<CharacterClass, int> InitialLuckForClass = new Dictionary<CharacterClass, int>()
    {
        {CharacterClass.Knight, 9 },
        {CharacterClass.Paladin, 7 },
        {CharacterClass.Archer, 7 },
        {CharacterClass.Druid, 9 },
        {CharacterClass.Cleric, 14 },
        {CharacterClass.Sorcerer, 11 }
    };

    public static int InitialStatForClass(CharacterStat stat, CharacterClass characterClass)
    {
        switch(stat)
        {
            case CharacterStat.Might:
                return InitialMightForClass[characterClass];
            case CharacterStat.Intellect:
                return InitialIntellectForClass[characterClass];
            case CharacterStat.Personality:
                return InitialPersonalityForClass[characterClass];
            case CharacterStat.Endurance:
                return InitialEnduranceForClass[characterClass];
            case CharacterStat.Accuracy:
                return InitialAccuracyForClass[characterClass];
            case CharacterStat.Speed:
                return InitialSpeedForClass[characterClass];
            case CharacterStat.Luck:
                return InitialLuckForClass[characterClass];
        }
        return 0;
    }

    public static Dictionary<CharacterClass, List<string>> InvalidSkillsByCharacterClass = new Dictionary<CharacterClass, List<string>>()
    {
        { CharacterClass.Knight, new List<string>() { "Fire", "Water", "Air", "Earth", "Body", "Mind", "Spirit", "Dark", "Light", "Meditation" } },
        { CharacterClass.Paladin, new List<string>() { "Fire", "Water", "Air", "Earth", "Dark", "Light" }  },
        { CharacterClass.Druid, new List<string>() { "Sword", "Axe", "Spear", "Dark", "Light", "Chain", "Plate",  }  },
        { CharacterClass.Archer, new List<string>() { "Body", "Mind", "Spirit", "Dark", "Light", "Shield", "Plate" }  },
        { CharacterClass.Cleric, new List<string>() { "Sword", "Dagger", "Axe", "Spear", "Fire", "Water", "Air", "Earth", "Plate" }  },
        { CharacterClass.Sorcerer, new List<string>() { "Sword", "Axe", "Spear", "Mace", "Body", "Mind", "Spirit", "Shield", "Chain", "Plate" }  }
    };

    public static Dictionary<CharacterClass, int> BaseHPByCharacterClass = new Dictionary<CharacterClass, int>()
    {
        {CharacterClass.Knight, 34 },
        {CharacterClass.Paladin, 28 },
        {CharacterClass.Druid, 22 },
        {CharacterClass.Archer, 28 },
        {CharacterClass.Cleric, 22 },
        {CharacterClass.Sorcerer, 22 }
    };

    public static Dictionary<CharacterClass, int> HPIncreaseByCharacterClass = new Dictionary<CharacterClass, int>()
    {
        {CharacterClass.Knight, 4 },
        {CharacterClass.Paladin, 3 },
        {CharacterClass.Druid, 2 },
        {CharacterClass.Archer, 3 },
        {CharacterClass.Cleric, 2 },
        {CharacterClass.Sorcerer, 2 }
    };

    public static int GetStatisticalEffect(int level)
    {
        int current = 0;
        foreach(var key in StatisticEffect.Keys)
        {
            if (level < key)
            {
                break;
            }
            current = key;
        }
        return StatisticEffect[current];
    }

    private static Dictionary<int, int> StatisticEffect = new Dictionary<int, int>()
    {
        { 0, -6 },
        { 3, -5 },
        { 5, -4 },
        { 7, -3 },
        { 9, -2 },
        { 11, -1 },
        { 13, 0 },
        { 15, 1 },
        { 17, 2 },
        { 19, 3 },
        { 21, 4 },
        { 25, 5 },
        { 30, 6 },
        { 35, 7 },
        { 40, 8 },
        { 50, 9 },
        { 75, 10 },
        { 100, 11 },
        { 125, 12 },
        { 150, 13 },
        { 175, 14 },
        { 200, 15 },
        { 225, 16 },
        { 250, 17 },
        { 275, 18 },
        { 300, 19 },
        { 350, 20 },
        { 400, 25 },
        { 500, 30 }
    };

    public static Dictionary<CharacterClass, int> EnduranceModByCharacterClass = new Dictionary<CharacterClass, int>()
    {
        {CharacterClass.Knight, 4 },
        {CharacterClass.Paladin, 3 },
        {CharacterClass.Druid, 2 },
        {CharacterClass.Archer, 3 },
        {CharacterClass.Cleric, 2 },
        {CharacterClass.Sorcerer, 2 }
    };

    public static Dictionary<CharacterClass, int> PersonalityModByCharacterClass = new Dictionary<CharacterClass, int>()
    {
        {CharacterClass.Knight, 0 },
        {CharacterClass.Paladin, 1 },
        {CharacterClass.Druid, 0 },
        {CharacterClass.Archer, 0 },
        {CharacterClass.Cleric, 3 },
        {CharacterClass.Sorcerer, 0 }
    };

    public static Dictionary<CharacterClass, int> IntellectModByCharacterClass = new Dictionary<CharacterClass, int>()
    {
        {CharacterClass.Knight, 0 },
        {CharacterClass.Paladin, 0 },
        {CharacterClass.Druid, 3 },
        {CharacterClass.Archer, 1 },
        {CharacterClass.Cleric, 0 },
        {CharacterClass.Sorcerer, 3 }
    };

    public static Dictionary<CharacterClass, int> HPScalingByCharacterClass_1 = new Dictionary<CharacterClass, int>()
    {
        {CharacterClass.Knight, 4 },
        {CharacterClass.Paladin, 3 },
        {CharacterClass.Druid, 2 },
        {CharacterClass.Archer, 3 },
        {CharacterClass.Cleric, 2 },
        {CharacterClass.Sorcerer, 2 }
    };

    public static Dictionary<CharacterClass, int> HPScalingByCharacterClass_2 = new Dictionary<CharacterClass, int>()
    {
        {CharacterClass.Knight, 7 },
        {CharacterClass.Paladin, 5 },
        {CharacterClass.Druid, 3 },
        {CharacterClass.Archer, 4 },
        {CharacterClass.Cleric, 3 },
        {CharacterClass.Sorcerer, 3 }
    };

    public static Dictionary<CharacterClass, int> BaseMPByCharacterClass = new Dictionary<CharacterClass, int>()
    {
        {CharacterClass.Knight, 0 },
        {CharacterClass.Paladin, 6 },
        {CharacterClass.Druid, 13 },
        {CharacterClass.Archer, 6 },
        {CharacterClass.Cleric, 13 },
        {CharacterClass.Sorcerer, 13 }
    };

    public static Dictionary<CharacterClass, int> MPScalingByCharacterClass_1 = new Dictionary<CharacterClass, int>()
    {
        {CharacterClass.Knight, 0 },
        {CharacterClass.Paladin, 1 },
        {CharacterClass.Druid, 3 },
        {CharacterClass.Archer, 1 },
        {CharacterClass.Cleric, 3 },
        {CharacterClass.Sorcerer, 3 }
    };

    public static Dictionary<CharacterClass, int> MPScalingByCharacterClass_2 = new Dictionary<CharacterClass, int>()
    {
        {CharacterClass.Knight, 0 },
        {CharacterClass.Paladin, 2 },
        {CharacterClass.Druid, 4 },
        {CharacterClass.Archer, 2 },
        {CharacterClass.Cleric, 4 },
        {CharacterClass.Sorcerer, 4 }
    };

    public static Dictionary<WeaponType, int> RecoveryByWeaponType = new Dictionary<WeaponType, int>()
    {
        {WeaponType.Club, 100 },
        {WeaponType.Axe, 100 },
        {WeaponType.Blaster, 30 },
        {WeaponType.Bow, 100 },
        {WeaponType.Dagger, 60 },
        {WeaponType.Mace, 80 },
        {WeaponType.Spear, 80 },
        {WeaponType.Staff, 100 },
        {WeaponType.Sword, 90 }
    };

    public static Dictionary<ArmorType, int> RecoveryByArmorType = new Dictionary<ArmorType, int>()
    {
        {ArmorType.Shield, 10 },
        {ArmorType.Leather, 10 },
        {ArmorType.Chain, 20 },
        {ArmorType.Plate, 30 }
    };

    public static Dictionary<CharacterStat, string> ColorForStat = new Dictionary<CharacterStat, string>()
    {
        { CharacterStat.Might, "Red"},
        { CharacterStat.Personality, "Blue"},
        { CharacterStat.Intellect, "Orange"},
        { CharacterStat.Accuracy, "Yellow"},
        { CharacterStat.Endurance, "Green"},
        { CharacterStat.Speed, "Purple"},
        { CharacterStat.Luck, "White"}
    };

    public static EnemyRank RandomRank()
    {
        float roll = Random.Range(0f, 1f);
        if(roll < 0.65f)
        {
            return EnemyRank.Soldier;
        }
        else if (roll < 0.90f)
        {
            return EnemyRank.Commander;
        }
        else
        {
            return EnemyRank.Captain;
        }
    }

    public static Dictionary<TreasureLevel, int[]> StandardEnchantmentRangeForItemlevel = new Dictionary<TreasureLevel, int[]>()
    {
        {TreasureLevel.L1, new int[] {0, 0} },
        {TreasureLevel.L2, new int[] {1, 5} },
        {TreasureLevel.L3, new int[] {3, 8} },
        {TreasureLevel.L4, new int[] {6, 12} },
        {TreasureLevel.L5, new int[] {10, 17} },
        {TreasureLevel.L6, new int[] {15, 25} }
    };

    public static Dictionary<TreasureLevel, EnchantmentType[]> EnchantmentTypesForItemlevel = new Dictionary<TreasureLevel, EnchantmentType[]>()
    {
        {TreasureLevel.L1, new EnchantmentType[] { } },
        {TreasureLevel.L2, new EnchantmentType[] { } },
        {TreasureLevel.L3, new EnchantmentType[] { EnchantmentType.A, EnchantmentType.B } },
        {TreasureLevel.L4, new EnchantmentType[] { EnchantmentType.A, EnchantmentType.B, EnchantmentType.C } },
        {TreasureLevel.L5, new EnchantmentType[] { EnchantmentType.B, EnchantmentType.C, EnchantmentType.D } },
        {TreasureLevel.L6, new EnchantmentType[] { EnchantmentType.D } }
    };

    public static Dictionary<TreasureLevel, double> StandardWeaponEnchantmentChancesForItemLevel = new Dictionary<TreasureLevel, double>()
    {
        {TreasureLevel.L1, 0 },
        {TreasureLevel.L2, 0 },
        {TreasureLevel.L3, 0.1 },
        {TreasureLevel.L4, 0.2 },
        {TreasureLevel.L5, 0.3 },
        {TreasureLevel.L6, 0.5 }
    };

    public static Dictionary<TreasureLevel, double> StandardArmorEnchantmentChancesForItemLevel = new Dictionary<TreasureLevel, double>()
    {
        {TreasureLevel.L1, 0 },
        {TreasureLevel.L2, 0.4 },
        {TreasureLevel.L3, 0.4 },
        {TreasureLevel.L4, 0.4 },
        {TreasureLevel.L5, 0.4 },
        {TreasureLevel.L6, 0.75 }
    };

    public static Dictionary<TreasureLevel, double> SpecialArmorEnchantmentChancesForItemLevel = new Dictionary<TreasureLevel, double>()
    {
        {TreasureLevel.L1, 0 },
        {TreasureLevel.L2, 0 },
        {TreasureLevel.L3, 0.1 },
        {TreasureLevel.L4, 0.15 },
        {TreasureLevel.L5, 0.2 },
        {TreasureLevel.L6, 0.25 }
    };

    public static Dictionary<TreasureLevel, int> GoldDropBaseAmountForItemLevel = new Dictionary<TreasureLevel, int>()
    {
        {TreasureLevel.L1, 50 },
        {TreasureLevel.L2, 100 },
        {TreasureLevel.L3, 200 },
        {TreasureLevel.L4, 500 },
        {TreasureLevel.L5, 1000 },
        {TreasureLevel.L6, 2000 }
    };

    public static Dictionary<TreasureLevel, int> GoldDropRangeForItemLevel = new Dictionary<TreasureLevel, int>()
    {
        {TreasureLevel.L1, 50 },
        {TreasureLevel.L2, 100 },
        {TreasureLevel.L3, 300 },
        {TreasureLevel.L4, 500 },
        {TreasureLevel.L5, 1000 },
        {TreasureLevel.L6, 3000 }
    };

    public static Dictionary<TreasureLevel, string> GoldDropIDForItemLevel = new Dictionary<TreasureLevel, string>()
    {
        {TreasureLevel.L1, "gold_1" },
        {TreasureLevel.L2, "gold_1" },
        {TreasureLevel.L3, "gold_2" },
        {TreasureLevel.L4, "gold_2" },
        {TreasureLevel.L5, "gold_3" },
        {TreasureLevel.L6, "gold_3" }
    };

    public static Dictionary<StoreType, RefreshPeriod> RefreshForStoreType = new Dictionary<StoreType, RefreshPeriod>()
    {
        {StoreType.General, RefreshPeriod.Daily },
        {StoreType.Weapon, RefreshPeriod.Weekly },
        {StoreType.Armor, RefreshPeriod.Weekly },
        {StoreType.Magic, RefreshPeriod.Weekly },
        {StoreType.Spell, RefreshPeriod.Even }
    };

    public static Dictionary<StoreType, string> MerchantTitleForStoreType = new Dictionary<StoreType, string>()
    {
        {StoreType.General, "Shopkeeper" },
        {StoreType.Weapon, "Blacksmith" },
        {StoreType.Armor, "Armorsmith" },
        {StoreType.Magic, "Alchemist" },
        {StoreType.Spell, "Guildmaster" },
    };

    public const string EXPRESSION_NEUTRAL = "neutral";
    public const string EXPRESSION_UNCONCIOUS = "unconcious";
    public const string EXPRESSION_DEAD = "dead";
    public const string EXPRESSION_POISON = "poison";
    public const string EXPRESSION_DISEASE = "disease";
    public const string EXPRESSION_WEAK = "weak";
    public const string EXPRESSION_SLEEP = "sleep";
    public const string EXPRESSION_HAPPY = "happy";
    public const float EXPRESSION_HAPPY_DURATION = 1.25f;
    public const string EXPRESSION_SAD = "sad";
    public const float EXPRESSION_SAD_DURATION = 1.25f;
    public const string EXPRESSION_HIT = "hit";
    public const float EXPRESSION_HIT_DURATION = 1.00f;
    public const string EXPRESSION_UNSURE = "unsure";
    public const float EXPRESSION_UNSURE_DURATION = 1.00f;
    public const string EXPRESSION_IDLE = "idle";
    public const float EXPRESSION_IDLE_DURATION = 0.5f;

    public const int SKILL_POINTS_PER_LEVEL = 5;
    public const int TRAINING_REQUIREMENT_EXPERT = 4;
    public const int TRAINING_REQUIREMENT_MASTER = 9;

    public const float REST_DURATION = 34f * 60f * 60f;

    public static Dictionary<SkillProficiency, string> LabelForProficiency = new Dictionary<SkillProficiency, string>()
    {
        {SkillProficiency.Novice, "Novice" },
        {SkillProficiency.Expert, "Expert" },
        {SkillProficiency.Master, "Master" },
    };
}
