using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;

public enum PartyMemberState
{
    Concious,
    Unconcious,
    Dead,
    Eradicated
}

public class PrimaryStats : EffectiveStats
{
    public int Might => _dict[CharacterStat.Might].BaseValue;
    public int Intellect => _dict[CharacterStat.Intellect].BaseValue;
    public int Personality => _dict[CharacterStat.Personality].BaseValue;
    public int Endurance => _dict[CharacterStat.Endurance].BaseValue;
    public int Accuracy => _dict[CharacterStat.Accuracy].BaseValue;
    public int Speed => _dict[CharacterStat.Speed].BaseValue;
    public int Luck => _dict[CharacterStat.Luck].BaseValue;

    public int EffectiveMight => Evaluate(CharacterStat.Might);
    public int EffectiveIntellect => Evaluate(CharacterStat.Intellect);
    public int EffectivePersonality => Evaluate(CharacterStat.Personality);
    public int EffectiveEndurance => Evaluate(CharacterStat.Endurance);
    public int EffectiveAccuracy => Evaluate(CharacterStat.Accuracy);
    public int EffectiveSpeed => Evaluate(CharacterStat.Speed);
    public int EffectiveLuck => Evaluate(CharacterStat.Luck);

    public PrimaryStats(int might, int intellect, int personality, int endurance, int accuracy, int speed, int luck)
    {
        _dict = new Dictionary<CharacterStat, EffectiveStat>()
        {
            {CharacterStat.Might, new EffectiveStat(CharacterStat.Might, might) },
            {CharacterStat.Intellect, new EffectiveStat(CharacterStat.Intellect, intellect) },
            {CharacterStat.Personality, new EffectiveStat(CharacterStat.Personality, personality) },
            {CharacterStat.Endurance, new EffectiveStat(CharacterStat.Endurance, endurance) },
            {CharacterStat.Accuracy, new EffectiveStat(CharacterStat.Accuracy, accuracy) },
            {CharacterStat.Speed, new EffectiveStat(CharacterStat.Speed, speed) },
            {CharacterStat.Luck, new EffectiveStat(CharacterStat.Luck, luck) }
        };
    }
}

public class Resistances : EffectiveStats, IResistance
{
    public int Fire => _dict[CharacterStat.FireResist].BaseValue;
    public int Elec => _dict[CharacterStat.ElecResist].BaseValue;
    public int Cold => _dict[CharacterStat.ColdResist].BaseValue;
    public int Poison => _dict[CharacterStat.PoisonResist].BaseValue;
    public int Magic => _dict[CharacterStat.MagicResist].BaseValue;

    public int EffectiveFire => Evaluate(CharacterStat.FireResist);
    public int EffectiveElec => Evaluate(CharacterStat.ElecResist);
    public int EffectiveCold => Evaluate(CharacterStat.ColdResist);
    public int EffectivePoison => Evaluate(CharacterStat.PoisonResist);
    public int EffectiveMagic => Evaluate(CharacterStat.MagicResist);

    public Resistances(int fire, int elec, int cold, int poison, int magic)
    {
        _dict = new Dictionary<CharacterStat, EffectiveStat>()
        {
            {CharacterStat.FireResist, new EffectiveStat(CharacterStat.FireResist, fire) },
            {CharacterStat.ElecResist, new EffectiveStat(CharacterStat.ElecResist, elec) },
            {CharacterStat.ColdResist, new EffectiveStat(CharacterStat.ColdResist, cold) },
            {CharacterStat.PoisonResist, new EffectiveStat(CharacterStat.PoisonResist, poison) },
            {CharacterStat.MagicResist, new EffectiveStat(CharacterStat.MagicResist, magic) }
        };
    }

    public int ResistanceForAttackType(AttackType type)
    {
        switch (type)
        {
            case AttackType.Fire:
                return EffectiveFire;
            case AttackType.Electricity:
                return EffectiveElec;
            case AttackType.Cold:
                return EffectiveCold;
            case AttackType.Poison:
                return EffectivePoison;
            case AttackType.Magic:
                return EffectiveMagic;
        }
        return 0;
    }
}

public class Profile : GameStateEntity {

    public string CharacterName { get; private set; }
    public CharacterClass Class { get; private set; }
    public int PortraitID { get; private set; }
    public int SkillPoints { get; private set; }
    public int Age { get; private set; }
    public int Level { get; private set; }
    public int Experience { get; private set; }

    PrimaryStats _stats;
    public PrimaryStats Stats => _stats;

    Resistances _resistances;
    public Resistances Resistances => _resistances;

    string _quickSpell;
    public string QuickSpell => string.IsNullOrEmpty(_quickSpell) ? "None" : _quickSpell;

    public string FullName => CharacterName + " the " + Class.ToString();

    public int MaxHitPoints =>
        GameConstants.BaseHPByCharacterClass[Class]
        + GameConstants.HPScalingByCharacterClass_1[Class] * (Level - 1)
        + GameConstants.EnduranceModByCharacterClass[Class] * GameConstants.GetStatisticalEffect(Stats.EffectiveEndurance);
    public int MaxSpellPoints =>
        GameConstants.BaseMPByCharacterClass[Class]
        + GameConstants.MPScalingByCharacterClass_1[Class] * (Level - 1)
        + GameConstants.PersonalityModByCharacterClass[Class] * GameConstants.GetStatisticalEffect(Stats.EffectivePersonality)
        + GameConstants.IntellectModByCharacterClass[Class] * GameConstants.GetStatisticalEffect(Stats.EffectiveIntellect);
    public int BaseArmorClass => GameConstants.GetStatisticalEffect(Stats.EffectiveSpeed);
    public int BaseAttack => GameConstants.GetStatisticalEffect(Stats.EffectiveAccuracy);
    public int BaseDamage => GameConstants.GetStatisticalEffect(Stats.EffectiveMight);
    public int BaseRangedAttack => GameConstants.GetStatisticalEffect(Stats.EffectiveAccuracy);
    public int BaseRangedDamage => 0;
    public int BaseResistance => GameConstants.GetStatisticalEffect(Stats.EffectiveLuck);

    Status _status;
    Equipment _equipment;
    Skillset _skillset;

    public event System.Action OnStatsChanged;

    public Profile(GameStateEntity parent, Status status, Equipment equipment, Skillset skillset, CharacterData data) : base(parent)
    {
        CharacterName = data.Name;
        PortraitID = data.PortraitID;
        Class = data.Class;
        SkillPoints = 0;
        Age = Random.Range(20, 35);
        Level = 1;
        Experience = Random.Range(250, 350);

        _stats = new PrimaryStats(data.Might, data.Intellect, data.Personality, data.Endurance, data.Accuracy, data.Speed, data.Luck);
        _resistances = new Resistances(0, 0, 0, 0, 0);

        _quickSpell = "";

        _status = status;
        _equipment = equipment;
        _skillset = skillset;

        _status.OnStatusChanged += UpdateEffectiveStats;
        _equipment.OnEquipmentChanged += UpdateEffectiveStats;

        UpdateEffectiveStats();
    }

    public Profile(GameStateEntity parent, Status status, Equipment equipment, Skillset skillset, XmlNode node) : base(parent, node)
    {
        XmlNode profileNode = node.SelectSingleNode("Profile");
        CharacterName = profileNode.SelectSingleNode("CharacterName").InnerText;
        PortraitID = ParseInt(profileNode,"PortraitID");
        Class = (CharacterClass)int.Parse(profileNode.SelectSingleNode("Class").InnerText);
        SkillPoints = int.Parse(profileNode.SelectSingleNode("SkillPoints").InnerText);
        Age = int.Parse(profileNode.SelectSingleNode("Age").InnerText);
        Level = int.Parse(profileNode.SelectSingleNode("Level").InnerText);
        Experience = int.Parse(profileNode.SelectSingleNode("Experience").InnerText);

        int might = int.Parse(profileNode.SelectSingleNode("Might").InnerText);
        int intellect = int.Parse(profileNode.SelectSingleNode("Intellect").InnerText);
        int personality = int.Parse(profileNode.SelectSingleNode("Personality").InnerText);
        int endurance = int.Parse(profileNode.SelectSingleNode("Endurance").InnerText);
        int accuracy = int.Parse(profileNode.SelectSingleNode("Accuracy").InnerText);
        int speed = int.Parse(profileNode.SelectSingleNode("Speed").InnerText);
        int luck = int.Parse(profileNode.SelectSingleNode("Luck").InnerText);

        _stats = new PrimaryStats(might, intellect, personality, endurance, accuracy, speed, luck);

        int fire = int.Parse(profileNode.SelectSingleNode("Fire").InnerText);
        int elec = int.Parse(profileNode.SelectSingleNode("Electricity").InnerText);
        int cold = int.Parse(profileNode.SelectSingleNode("Cold").InnerText);
        int poison = int.Parse(profileNode.SelectSingleNode("Poison").InnerText);
        int magic = int.Parse(profileNode.SelectSingleNode("Magic").InnerText);

        _resistances = new Resistances(fire, elec, cold, poison, magic);

        _quickSpell = profileNode.SelectSingleNode("QuickSpell").InnerText;

        _status = status;
        _equipment = equipment;
        _skillset = skillset;

        _status.OnStatusChanged += UpdateEffectiveStats;
        _equipment.OnEquipmentChanged += UpdateEffectiveStats;

        UpdateEffectiveStats();
    }

    public override XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("Profile");
        element.AppendChild(XmlHelper.Attribute(doc, "CharacterName", CharacterName));
        element.AppendChild(XmlHelper.Attribute(doc, "PortraitID", PortraitID));
        element.AppendChild(XmlHelper.Attribute(doc, "Class", (int)Class));
        element.AppendChild(XmlHelper.Attribute(doc, "SkillPoints", SkillPoints));
        element.AppendChild(XmlHelper.Attribute(doc, "Age", Age));
        element.AppendChild(XmlHelper.Attribute(doc, "Level", Level));
        element.AppendChild(XmlHelper.Attribute(doc, "Experience", Experience));

        element.AppendChild(XmlHelper.Attribute(doc, "Might", Stats.Might));
        element.AppendChild(XmlHelper.Attribute(doc, "Intellect", Stats.Intellect));
        element.AppendChild(XmlHelper.Attribute(doc, "Personality", Stats.Personality));
        element.AppendChild(XmlHelper.Attribute(doc, "Endurance", Stats.Endurance));
        element.AppendChild(XmlHelper.Attribute(doc, "Accuracy", Stats.Accuracy));
        element.AppendChild(XmlHelper.Attribute(doc, "Speed", Stats.Speed));
        element.AppendChild(XmlHelper.Attribute(doc, "Luck", Stats.Luck));

        element.AppendChild(XmlHelper.Attribute(doc, "Fire", Resistances.Fire));
        element.AppendChild(XmlHelper.Attribute(doc, "Electricity", Resistances.Elec));
        element.AppendChild(XmlHelper.Attribute(doc, "Cold", Resistances.Cold));
        element.AppendChild(XmlHelper.Attribute(doc, "Poison", Resistances.Poison));
        element.AppendChild(XmlHelper.Attribute(doc, "Magic", Resistances.Magic));

        element.AppendChild(XmlHelper.Attribute(doc, "QuickSpell", _quickSpell));

        element.AppendChild(base.ToXml(doc));
        return element;
    }

    public void UpdateEffectiveStats()
    {
        _stats.Flush();
        _equipment.ModifyStats(_stats);
        _status.ModifyStats(_stats);

        _resistances.Flush();
        _equipment.ModifyStats(_resistances);
        _status.ModifyStats(_resistances);

        OnStatsChanged?.Invoke();
    }

    public int NeededXP
    {
        get { return Level * (Level + 1) * 500; }
    }

    public string GetTrainingDetails()
    {
        if(CanTrainLevel())
        {
            return "You have enough experience to train to level " + (Level + 1);
        }

        return "You need " + (NeededXP - Experience) + " more experience to train to level " + (Level + 1);
    }

    public bool CanTrainLevel()
    {
        return Experience >= NeededXP;
    }

    public void TrainLevel()
    {
        Level++;
        SkillPoints += GameConstants.SKILL_POINTS_PER_LEVEL;
    }

    public void AddSkillPoints(int amount)
    {
        SkillPoints += amount;
    }

    public void AddStatPoints(CharacterStat stat, int amount)
    {
        Stats.GetStat(stat).IncreaseBase(amount);
        OnStatsChanged?.Invoke();
    }

    public void EarnXP(int value)
    {
        Experience += value;
    }

    public bool CanUpgradeSkill(InventorySkill skill, out int difference)
    {
        difference = skill.Level - SkillPoints + 1;
        return SkillPoints > skill.Level;
    }

    public bool TryUpgradeSkill(InventorySkill skill)
    {
        int diff;
        if (!CanUpgradeSkill(skill, out diff))
            return false;

        skill.Upgrade();
        SkillPoints -= skill.Level;
        return true;
    }
}
