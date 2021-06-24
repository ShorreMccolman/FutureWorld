using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterClass
{
    Knight = 0,
    Paladin = 1,
    Druid = 2,
    Archer = 3,
    Cleric = 4,
    Sorcerer = 5
}

public enum CharacterStat
{
    None = 0,
    Might = 1,
    Intellect = 2,
    Personality = 3,
    Endurance = 4,
    Accuracy = 5,
    Speed = 6,
    Luck = 7,

    TotalHP = 10,
    TotalSP = 11,
    ArmorClass = 12,
    Attack = 13,
    DamageUpper = 14,
    DamageLower = 15,
    Recovery = 16,
    RangedAttack = 17,
    RangedDamageUpper = 18,
    RangedDamageLower = 19,
    RangedRecovery = 20,

    FireResist = 40,
    ElecResist = 41,
    ColdResist = 42,
    PoisonResist = 43,
    MagicResist = 44,

    Random = 99
}

public class CharacterData
{
    public int ID { get; private set; }
    public CharacterClass Class { get; private set; }

    bool didChange;
    bool hasBeenCreated;

    public int PortraitID { get; private set; }

    public string Name { get; private set; }

    int might;
    public int Might
    {
        get { return might; }
        set
        {
            didChange = VerifyStat(CharacterStat.Might, value);
            if (didChange)
            {
                PointsSpent += value - might;
                might = value;
            }
        }
    }
    int intellect;
    public int Intellect
    {
        get { return intellect; }
        set
        {
            didChange = VerifyStat(CharacterStat.Intellect, value);
            if (didChange)
            {
                PointsSpent += value - intellect;
                intellect = value;
            }
        }
    }
    int personality;
    public int Personality
    {
        get { return personality; }
        set
        {
            didChange = VerifyStat(CharacterStat.Personality, value);
            if (didChange)
            {
                PointsSpent += value - personality;
                personality = value;
            }
        }
    }
    int endurance;
    public int Endurance
    {
        get { return endurance; }
        set
        {
            didChange = VerifyStat(CharacterStat.Endurance, value);
            if (didChange)
            {
                PointsSpent += value - endurance;
                endurance = value;
            }
        }
    }
    int accuracy;
    public int Accuracy
    {
        get { return accuracy; }
        set
        {
            didChange = VerifyStat(CharacterStat.Accuracy, value);
            if (didChange)
            {
                PointsSpent += value - accuracy;
                accuracy = value;
            }
        }
    }
    int speed;
    public int Speed
    {
        get { return speed; }
        set
        {
            didChange = VerifyStat(CharacterStat.Speed, value);
            if (didChange)
            {
                PointsSpent += value - speed;
                speed = value;
            }
        }
    }

    int luck;
    public int Luck
    {
        get { return luck; }
        set
        {
            didChange = VerifyStat(CharacterStat.Luck, value);
            if (didChange)
            {
                PointsSpent += value - luck;
                luck = value;
            }
        }
    }

    public List<string> Skills { get; private set; }

    public List<string> SuggestedSkills { get; private set; }

    public int PointsSpent { get; private set; }

    public bool AutoEquip { get; private set; }

    public CharacterData(int id, CharacterClass characterClass, bool autoEquip = false)
    {
        ID = id;
        PortraitID = id;
        Class = characterClass;
        GenerateRandomName();
        InitStatsForClass();
        InitSkillsForClass();
        PointsSpent = 0;
        AutoEquip = autoEquip;
    }

    public void GenerateRandomName()
    {
        string[] names;
        if (PortraitID % 2 == 0)
            names = GameConstants.RandomCharacterNamesMale;
        else
            names = GameConstants.RandomCharacterNamesFemale;

        Name = names[Random.Range(0, names.Length)];
    }

    public void SetName(string name)
    {
        Name = name;
    }

    public void ChangePortrait(int id)
    {
        PortraitID = id;
        GenerateRandomName();
    }

    public void ChangeClass(CharacterClass characterClass)
    {
        Class = characterClass;
        InitStatsForClass();
        InitSkillsForClass();
        PointsSpent = 0;
    }

    public void InitStatsForClass()
    {
        Might = GameConstants.InitialStatForClass(CharacterStat.Might, Class);
        Intellect = GameConstants.InitialStatForClass(CharacterStat.Intellect, Class);
        Personality = GameConstants.InitialStatForClass(CharacterStat.Personality, Class);
        Endurance = GameConstants.InitialStatForClass(CharacterStat.Endurance, Class);
        Accuracy = GameConstants.InitialStatForClass(CharacterStat.Accuracy, Class);
        Speed = GameConstants.InitialStatForClass(CharacterStat.Speed, Class);
        Luck = GameConstants.InitialStatForClass(CharacterStat.Luck, Class);
    }

    bool VerifyStat(CharacterStat stat, int value)
    {
        if (!hasBeenCreated)
        {
            if (value < GameConstants.InitialStatForClass(stat, Class) - 2)
                return false;

            if (value > 30)
                return false;
        }

        return true;
    }

    public void RemoveSkill(string skillName)
    {
        if(Skills.Contains(skillName))
        {
            Skills.Remove(skillName);
        }
    }

    public void SelectSkill(string skillName)
    {
        if (Skills.Count >= 4)
            return;

        if (Skills.Contains(skillName))
            return;

        Skills.Add(skillName);
    }

    public bool HasSkill(string skillName)
    {
        return Skills.Contains(skillName);
    }

    void InitSkillsForClass()
    {
        Skills = new List<string>();
        SuggestedSkills = new List<string>();

        switch(Class)
        {
            case CharacterClass.Knight:
                Skills.Add("Sword");
                Skills.Add("Leather");

                SuggestedSkills.Add("Axe");
                SuggestedSkills.Add("Bow");
                SuggestedSkills.Add("Dagger");
                SuggestedSkills.Add("Spear");
                SuggestedSkills.Add("Chain");
                SuggestedSkills.Add("Shield");
                SuggestedSkills.Add("Bodybuilding");
                SuggestedSkills.Add("Disarm");
                SuggestedSkills.Add("Perception");
                break;
            case CharacterClass.Paladin:
                Skills.Add("Sword");
                Skills.Add("Spirit");

                SuggestedSkills.Add("Axe");
                SuggestedSkills.Add("Bow");
                SuggestedSkills.Add("Dagger");
                SuggestedSkills.Add("Spear");
                SuggestedSkills.Add("Leather");
                SuggestedSkills.Add("Chain");
                SuggestedSkills.Add("Diplomacy");
                SuggestedSkills.Add("Disarm");
                SuggestedSkills.Add("Perception");
                break;
            case CharacterClass.Archer:
                Skills.Add("Bow");
                Skills.Add("Air");

                SuggestedSkills.Add("Axe");
                SuggestedSkills.Add("Dagger");
                SuggestedSkills.Add("Sword");
                SuggestedSkills.Add("Leather");
                SuggestedSkills.Add("Fire");
                SuggestedSkills.Add("Diplomacy");
                SuggestedSkills.Add("Disarm");
                SuggestedSkills.Add("Identify");
                SuggestedSkills.Add("Perception");
                break;
            case CharacterClass.Druid:
                Skills.Add("Staff");
                Skills.Add("Earth");

                SuggestedSkills.Add("Mace");
                SuggestedSkills.Add("Leather");
                SuggestedSkills.Add("Water");
                SuggestedSkills.Add("Body");
                SuggestedSkills.Add("Mind");
                SuggestedSkills.Add("Identify");
                SuggestedSkills.Add("Learning");
                SuggestedSkills.Add("Meditation");
                SuggestedSkills.Add("Repair");
                break;
            case CharacterClass.Cleric:
                Skills.Add("Mace");
                Skills.Add("Body");

                SuggestedSkills.Add("Staff");
                SuggestedSkills.Add("Leather");
                SuggestedSkills.Add("Shield");
                SuggestedSkills.Add("Mind");
                SuggestedSkills.Add("Spirit");
                SuggestedSkills.Add("Diplomacy");
                SuggestedSkills.Add("Identify");
                SuggestedSkills.Add("Meditation");
                SuggestedSkills.Add("Repair");
                break;
            case CharacterClass.Sorcerer:
                Skills.Add("Dagger");
                Skills.Add("Fire");

                SuggestedSkills.Add("Staff");
                SuggestedSkills.Add("Leather");
                SuggestedSkills.Add("Air");
                SuggestedSkills.Add("Earth");
                SuggestedSkills.Add("Water");
                SuggestedSkills.Add("Diplomacy");
                SuggestedSkills.Add("Identify");
                SuggestedSkills.Add("Meditation");
                SuggestedSkills.Add("Repair");
                break;
        }
    }

    public string GetSkillNameByIndex(int index)
    {
        if(Skills.Count < index + 1)
        {
            return "";
        }
        return Skills[index];
    }

    public string Log()
    {
        string log = "CharacterData - \n" +
                "Name: " + Name + "\n" +
                "Class: " + Class + "\n" +
                "Might: " + Might + "\n" +
                "Intellect: " + Intellect + "\n" +
                "Personality: " + Personality + "\n" +
                "Endurance: " + Endurance + "\n" +
                "Accuracy: " + Accuracy + "\n" +
                "Speed: " + Speed + "\n" +
                "Luck: " + Luck + "\n" +
                "Skills: ";

        foreach(var skill in Skills)
        {
            log += skill + ",";
        }


        return log;
    }
}
