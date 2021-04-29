using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public enum SkillProficiency
{
    Novice = 0,
    Expert = 1,
    Master = 2
}

public class InventorySkill : GameStateEntity
{
    public string ID { get; private set; }
    public int Level { get; private set; }
    public SkillProficiency Proficiency { get; private set; }
    public Skill Skill { get; private set; }

    public InventorySkill(Skillset parent, Skill skill) : base(parent)
    {
        ID = skill.ID;
        Level = 1;
        Proficiency = SkillProficiency.Novice;
        Skill = skill;
    }

    public InventorySkill(Skillset parent, XmlNode node) : base(parent, node)
    {
        ID = node.SelectSingleNode("ID").InnerText;
        Level = int.Parse(node.SelectSingleNode("Level").InnerText);
        Proficiency = (SkillProficiency)int.Parse(node.SelectSingleNode("Proficiency").InnerText);
        Skill = SkillDatabase.Instance.GetSkill(ID);
    }

    public override XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("Skill");
        element.AppendChild(XmlHelper.Attribute(doc, "ID", ID));
        element.AppendChild(XmlHelper.Attribute(doc, "Level", Level));
        element.AppendChild(XmlHelper.Attribute(doc, "Proficiency", (int)Proficiency));
        element.AppendChild(base.ToXml(doc));
        return element;
    }

    public void Upgrade()
    {
        Level++;
    }

    public void IncreaseMastery()
    {
        if (Proficiency == SkillProficiency.Novice)
            Proficiency = SkillProficiency.Expert;
        else if (Proficiency == SkillProficiency.Expert)
            Proficiency = SkillProficiency.Master;
    }
}

public class Skillset : GameStateEntity
{
    public List<InventorySkill> Skills { get; private set; }

    public Skillset(GameStateEntity parent, CharacterData data) : base(parent)
    {
        Skills = new List<InventorySkill>();
        foreach(var id in data.Skills)
        {
            InventorySkill skill = SkillDatabase.Instance.GetInventorySkill(id, this); 
            Skills.Add(skill);
        }
    }

    public Skillset(GameStateEntity parent, XmlNode node) : base(parent, node)
    {
        XmlNode skillNode = node.SelectSingleNode("Skillset");
        Skills = new List<InventorySkill>();
        XmlNodeList skillNodes = skillNode.SelectNodes("Skill");
        for (int i = 0; i < skillNodes.Count; i++)
        {
            Skills.Add(new InventorySkill(this, skillNodes.Item(i)));
        }
    }

    public override XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("Skillset");
        foreach(var skill in Skills)
        {
            element.AppendChild(skill.ToXml(doc));
        }
        element.AppendChild(base.ToXml(doc));
        return element;
    }

    public bool KnowsSkill(string ID)
    {
        return Skills.Find(s => s.Skill.ID == ID) != null;
    }

    public bool KnowsSkillAtProficiency(string ID, SkillProficiency proficiency)
    {
        if (!KnowsSkill(ID))
            return false;

        InventorySkill skill = GetSkillByID(ID);
        if (proficiency == SkillProficiency.Novice)
            return true;
        else if (proficiency == SkillProficiency.Expert)
            return skill.Proficiency != SkillProficiency.Novice;
        else
            return skill.Proficiency == SkillProficiency.Master;
    }

    public bool CanTrain(string ID, SkillProficiency proficiency)
    {
        if (!KnowsSkill(ID))
            return false;

        InventorySkill skill = GetSkillByID(ID);
        if (proficiency == skill.Proficiency)
            return false;

        if (skill.Proficiency == SkillProficiency.Master)
            return false;

        if (proficiency == SkillProficiency.Expert)
        {
            return skill.Level >= GameConstants.TRAINING_REQUIREMENT_EXPERT;
        }
        else if (proficiency == SkillProficiency.Master)
        {
            return skill.Level >= GameConstants.TRAINING_REQUIREMENT_MASTER;
        }

        return false;
    }

    public void TrainSkill(string ID)
    {
        InventorySkill skill = GetSkillByID(ID);
        skill.IncreaseMastery();
    }

    public InventorySkill GetSkillByID(string ID)
    {
        return Skills.Find(s => s.Skill.ID == ID);
    }

    public List<InventorySkill> GetSkillsByType(SkillType type)
    {
        return Skills.FindAll(s => s.Skill.Type == type);
    }

    public void LearnSkill(Skill skill)
    {
        Skills.Add(new InventorySkill(this, skill));
    }

    public bool CanEquip(Item item)
    {
        Weapon weapon = item as Weapon;
        if(weapon != null)
        {
            if (weapon.Type == WeaponType.Club)
                return true;

            return Skills.Exists(s => s.Skill.ID == weapon.Type.ToString());
        }

        Armor armor = item as Armor;
        if(armor != null)
        {
            if (armor.Type == ArmorType.Generic)
                return true;

            return Skills.Exists(s => s.Skill.ID == armor.Type.ToString());
        }

        return false;
    }

    public bool CanDualWield(Item item)
    {
        if(item is Weapon)
        {
            Weapon weapon = item as Weapon;
            foreach(var skill in Skills)
            {
                if(skill.ID == weapon.Type.ToString())
                {
                    switch(weapon.Type)
                    {
                        case WeaponType.Dagger:
                            return skill.Proficiency != SkillProficiency.Novice;
                        case WeaponType.Sword:
                            return skill.Proficiency == SkillProficiency.Master;
                    }
                }
            }
        }
        return false;
    }

    public int GetArmorClass(ArmorType type)
    {
        InventorySkill skill;
        switch (type)
        {
            case ArmorType.Leather:
                skill = GetSkillByID("Leather");
                return skill.Level;
            case ArmorType.Chain:
                skill = GetSkillByID("Chain");
                return skill.Level;
            case ArmorType.Plate:
                skill = GetSkillByID("Plate");
                return skill.Level;
            case ArmorType.Shield:
                skill = GetSkillByID("Shield");
                switch(skill.Proficiency)
                {
                    case SkillProficiency.Novice:
                        return skill.Level;
                    case SkillProficiency.Expert:
                        return skill.Level * 2;
                    case SkillProficiency.Master:
                        return skill.Level * 3;
                }
                break;
        }

        return 0;
    }

    public int GetArmorClass(WeaponType type)
    {
        InventorySkill skill;
        switch (type)
        {
            case WeaponType.Spear:
                skill = GetSkillByID("Spear");
                if (skill.Proficiency == SkillProficiency.Novice)
                    return 0;
                return skill.Level;
        }

        return 0;
    }

    public int GetAttack(WeaponType type)
    {
        InventorySkill skill;
        switch (type)
        {
            case WeaponType.Club:
                return 1;
            case WeaponType.Axe:
                skill = GetSkillByID("Axe");
                return skill.Level;
            case WeaponType.Bow:
                skill = GetSkillByID("Bow");
                return skill.Level;
            case WeaponType.Dagger:
                skill = GetSkillByID("Dagger");
                return skill.Level;
            case WeaponType.Mace:
                skill = GetSkillByID("Mace");
                return skill.Level;
            case WeaponType.Spear:
                skill = GetSkillByID("Spear");
                return skill.Level;
            case WeaponType.Staff:
                skill = GetSkillByID("Staff");
                return skill.Level;
            case WeaponType.Sword:
                skill = GetSkillByID("Sword");
                return skill.Level;
        }

        return 0;
    }

    public int GetDamage(WeaponType type)
    {
        InventorySkill skill;
        switch (type)
        {
            case WeaponType.Axe:
                skill = GetSkillByID("Axe");
                if(skill.Proficiency == SkillProficiency.Master)
                    return skill.Level;
                break;

            case WeaponType.Spear:
                skill = GetSkillByID("Spear");
                if (skill.Proficiency == SkillProficiency.Master)
                    return skill.Level;
                break;

            case WeaponType.Mace:
                skill = GetSkillByID("Mace");
                if (skill.Proficiency == SkillProficiency.Expert || skill.Proficiency == SkillProficiency.Master)
                    return skill.Level;
                break;
        }

        return 0;
    }

    public int DamageModifier(WeaponType type, int damage)
    {
        InventorySkill skill;
        switch (type)
        {
            case WeaponType.Dagger:
                skill = GetSkillByID("Dagger");
                if (skill.Proficiency == SkillProficiency.Master)
                {
                    float rand = Random.Range(0, 100f);
                    if(rand <= skill.Level)
                    {
                        return damage * 3;
                    }
                }
                break;
        }
        return damage;
    }

    public int GetWeaponRecovery(WeaponType type, Enchantment enchantment)
    {
        int value = GameConstants.RecoveryByWeaponType[type];

        InventorySkill skill;
        switch (type)
        {
            case WeaponType.Axe:
                skill = GetSkillByID("Axe");
                if (skill.Proficiency == SkillProficiency.Expert)
                    value -= skill.Level;
                break;
            case WeaponType.Sword:
                skill = GetSkillByID("Sword");
                if (skill.Proficiency == SkillProficiency.Expert)
                    value -= skill.Level;
                break;
            case WeaponType.Bow:
                skill = GetSkillByID("Bow");
                if (skill.Proficiency == SkillProficiency.Expert)
                    value -= skill.Level;
                break;
        }

        if(enchantment != null)
            value -= enchantment.StrengthOfOption(EnchantmentEffectOption.WeaponRecovery);

        return value;
    }

    public int GetRecovery(ArmorType type, Enchantment enchantment)
    {
        int value = GameConstants.RecoveryByArmorType[type];

        InventorySkill skill;
        switch (type)
        {
            case ArmorType.Leather:
                skill = GetSkillByID("Leather");
                if (skill.Proficiency == SkillProficiency.Expert)
                    value -= skill.Level;
                else if (skill.Proficiency == SkillProficiency.Master)
                    return 0;
                break;
            case ArmorType.Chain:
                skill = GetSkillByID("Chain");
                if (skill.Proficiency == SkillProficiency.Expert)
                    value -= skill.Level;
                else if (skill.Proficiency == SkillProficiency.Master)
                    return 0;
                break;
            case ArmorType.Plate:
                skill = GetSkillByID("Plate");
                if (skill.Proficiency == SkillProficiency.Expert)
                    value -= skill.Level;
                else if (skill.Proficiency == SkillProficiency.Master)
                    return 0;
                break;
        }

        return Mathf.Max(0, value);
    }

    public int GetBonusHP(CharacterClass characterClass)
    {
        InventorySkill skill = GetSkillByID("Bodybuilding");
        if (skill != null)
        {
            switch (skill.Proficiency)
            {
                case SkillProficiency.Novice:
                    return skill.Level * GameConstants.HPScalingByCharacterClass_1[characterClass];
                case SkillProficiency.Expert:
                    return 2 * skill.Level * GameConstants.HPScalingByCharacterClass_1[characterClass];
                case SkillProficiency.Master:
                    return 3 * skill.Level * GameConstants.HPScalingByCharacterClass_1[characterClass];
            }
        }
        return 0;
    }

    public int GetBonusMP(CharacterClass characterClass)
    {
        InventorySkill skill = GetSkillByID("Meditation");
        if(skill != null)
        {
            switch(skill.Proficiency)
            {
                case SkillProficiency.Novice:
                    return skill.Level * GameConstants.MPScalingByCharacterClass_1[characterClass];
                case SkillProficiency.Expert:
                    return 2 * skill.Level * GameConstants.MPScalingByCharacterClass_1[characterClass];
                case SkillProficiency.Master:
                    return 3 * skill.Level * GameConstants.MPScalingByCharacterClass_1[characterClass];
            }
        }
        return 0;
    }
}
