using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;

public enum PartyMemberState
{
    Good,
    Unconcious,
    Dead,
    Eradicated
}

[System.Serializable]
public struct Resistances
{
    public int Fire;
    public int Electricity;
    public int Cold;
    public int Poison;
    public int Magic;
    public int Physical;

    public Resistances(int fire, int electricity, int cold, int poison, int magic, int physical)
    {
        Fire = fire;
        Electricity = electricity;
        Cold = cold;
        Poison = poison;
        Magic = magic;
        Physical = physical;
    }

    public Resistances(XmlNode node)
    {
        XmlNode resistanceNode = node.SelectSingleNode("Resistances");
        Fire = int.Parse(resistanceNode.SelectSingleNode("Fire").InnerText);
        Electricity = int.Parse(resistanceNode.SelectSingleNode("Electricity").InnerText);
        Cold = int.Parse(resistanceNode.SelectSingleNode("Cold").InnerText);
        Poison = int.Parse(resistanceNode.SelectSingleNode("Poison").InnerText);
        Magic = int.Parse(resistanceNode.SelectSingleNode("Magic").InnerText);
        Physical = int.Parse(resistanceNode.SelectSingleNode("Physical").InnerText);
    }

    public XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("Resistances");
        element.AppendChild(XmlHelper.Attribute(doc, "Fire", Fire));
        element.AppendChild(XmlHelper.Attribute(doc, "Electricity", Electricity));
        element.AppendChild(XmlHelper.Attribute(doc, "Cold", Cold));
        element.AppendChild(XmlHelper.Attribute(doc, "Poison", Poison));
        element.AppendChild(XmlHelper.Attribute(doc, "Magic", Magic));
        element.AppendChild(XmlHelper.Attribute(doc, "Physical", Physical));
        return element;
    }

    public int GetResistanceByType(AttackType type)
    {
        switch (type)
        {
            case AttackType.Fire:
                return Fire;
            case AttackType.Electricity:
                return Electricity;
            case AttackType.Cold:
                return Cold;
            case AttackType.Poison:
                return Poison;
            case AttackType.Magic:
                return Magic;
            case AttackType.Physical:
                return Physical;
            default:
                return 0;
        }
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

    public int Might { get; private set; }
    public int Intellect { get; private set; }
    public int Personality { get; private set; }
    public int Endurance { get; private set; }
    public int Accuracy { get; private set; }
    public int Speed { get; private set; }
    public int Luck { get; private set; }

    public string QuickSpell { get; private set; }

    public Resistances Resistances { get; private set; }

    Status _status;
    Equipment _equipment;

    public int HitPoints()
    {
        int health = GameConstants.BaseHPByCharacterClass[Class];
        health += GameConstants.HPScalingByCharacterClass_1[Class] * (Level - 1);
        health += GameConstants.EnduranceModByCharacterClass[Class] * GameConstants.GetStatisticalEffect(EffectiveEndurance);
        return health;
    }

    public int SpellPoints()
    {
        int health = GameConstants.BaseMPByCharacterClass[Class];
        health += GameConstants.MPScalingByCharacterClass_1[Class] * (Level - 1);
        health += GameConstants.PersonalityModByCharacterClass[Class] * GameConstants.GetStatisticalEffect(EffectivePersonality);
        health += GameConstants.IntellectModByCharacterClass[Class] * GameConstants.GetStatisticalEffect(EffectiveIntellect);
        return health;
    }

    public int EffectiveMight
    {
        get { return _status.ModifyMight(Might + _equipment.MightBonus()); }
    }

    public int EffectiveEndurance
    {
        get { return _status.ModifyEndurance(Endurance + _equipment.EnduranceBonus()); }
    }

    public int EffectiveSpeed
    {
        get { return _status.ModifySpeed(Speed + _equipment.SpeedBonus()); }
    }

    public int EffectiveIntellect
    {
        get { return _status.ModifyIntellect(Intellect + _equipment.IntellectBonus()); }
    }

    public int EffectivePersonality
    {
        get { return _status.ModifyPersonality(Personality + _equipment.PersonalityBonus()); }
    }

    public int EffectiveAccuracy
    {
        get { return _status.ModifyAccuracy(Accuracy + _equipment.AccuracyBonus()); }
    }

    public int EffectiveLuck
    {
        get { return _status.ModifyLuck(Luck + _equipment.LuckBonus()); }
    }

    public int BaseArmorClass()
    {
        return GameConstants.GetStatisticalEffect(EffectiveSpeed);
    }

    public int BaseAttack()
    {
        return GameConstants.GetStatisticalEffect(EffectiveAccuracy);
    }

    public int BaseDamage()
    {
        return GameConstants.GetStatisticalEffect(EffectiveMight);
    }

    public int BaseRangedAttack(int bonus)
    {
        return GameConstants.GetStatisticalEffect(EffectiveAccuracy);
    }

    public int BaseRangedDamage()
    {
        return 0;
    }

    public int BaseResistance()
    {
        return GameConstants.GetStatisticalEffect(EffectiveLuck);
    }

    public int EffectiveResistance(AttackType type)
    {
        return _status.ModifyResistance(type, Resistances.GetResistanceByType(type) + _equipment.ResistanceBonus(type));
    }

    public Profile(GameStateEntity parent, Status status, Equipment equipment, CharacterData data) : base(parent)
    {
        CharacterName = data.Name;
        PortraitID = data.PortraitID;
        Class = data.Class;
        SkillPoints = 0;
        Age = Random.Range(20, 35);
        Level = 1;
        Experience = Random.Range(250, 350);
        Might = data.Might;
        Intellect = data.Intellect;
        Personality = data.Personality;
        Endurance = data.Endurance;
        Accuracy = data.Accuracy;
        Speed = data.Speed;
        Luck = data.Luck;
        QuickSpell = "";
        Resistances = new Resistances(0, 0, 0, 0, 0, 0);
        _status = status;
        _equipment = equipment;
    }

    public Profile(GameStateEntity parent, Status status, Equipment equipment, XmlNode node) : base(parent, node)
    {
        XmlNode profileNode = node.SelectSingleNode("Profile");
        CharacterName = profileNode.SelectSingleNode("CharacterName").InnerText;
        PortraitID = ParseInt(profileNode,"PortraitID");
        Class = (CharacterClass)int.Parse(profileNode.SelectSingleNode("Class").InnerText);
        SkillPoints = int.Parse(profileNode.SelectSingleNode("SkillPoints").InnerText);
        Age = int.Parse(profileNode.SelectSingleNode("Age").InnerText);
        Level = int.Parse(profileNode.SelectSingleNode("Level").InnerText);
        Experience = int.Parse(profileNode.SelectSingleNode("Experience").InnerText);

        Might = int.Parse(profileNode.SelectSingleNode("Might").InnerText);
        Intellect = int.Parse(profileNode.SelectSingleNode("Intellect").InnerText);
        Personality = int.Parse(profileNode.SelectSingleNode("Personality").InnerText);
        Endurance = int.Parse(profileNode.SelectSingleNode("Endurance").InnerText);
        Accuracy = int.Parse(profileNode.SelectSingleNode("Accuracy").InnerText);
        Speed = int.Parse(profileNode.SelectSingleNode("Speed").InnerText);
        Luck = int.Parse(profileNode.SelectSingleNode("Luck").InnerText);
        
        QuickSpell = profileNode.SelectSingleNode("QuickSpell").InnerText;

        Resistances = new Resistances(profileNode);

        _status = status;
        _equipment = equipment;
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

        element.AppendChild(XmlHelper.Attribute(doc, "Might", Might));
        element.AppendChild(XmlHelper.Attribute(doc, "Intellect", Intellect));
        element.AppendChild(XmlHelper.Attribute(doc, "Personality", Personality));
        element.AppendChild(XmlHelper.Attribute(doc, "Endurance", Endurance));
        element.AppendChild(XmlHelper.Attribute(doc, "Accuracy", Accuracy));
        element.AppendChild(XmlHelper.Attribute(doc, "Speed", Speed));
        element.AppendChild(XmlHelper.Attribute(doc, "Luck", Luck));
        
        element.AppendChild(XmlHelper.Attribute(doc, "QuickSpell", QuickSpell));

        element.AppendChild(Resistances.ToXml(doc));

        element.AppendChild(base.ToXml(doc));
        return element;
    }

    public bool CanTrainLevel()
    {
        int xpForLevel = Level * (Level + 1) * 500;
        return Experience >= xpForLevel;
    }

    public bool CanTrainLevel(out int xpNeeded)
    {
        int xpForLevel = Level * (Level + 1) * 500;

        xpNeeded = xpForLevel - Experience;
        return Experience >= xpForLevel;
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
