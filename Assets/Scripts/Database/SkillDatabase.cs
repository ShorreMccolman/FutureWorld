using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    Weapon,
    Armor,
    Magic,
    Misc
}

[System.Serializable]
public class Skill
{
    public string ID;
    public string DisplayName;
    public string TrainingName;
    public SkillType Type;
}

public class SkillDatabase {
    public static SkillDatabase Instance;

    Dictionary<string, Skill> _skillDict;

    public SkillDatabase()
    {
        Instance = this;

        _skillDict = new Dictionary<string, Skill>();

        SkillDBObject skillDBObject = Resources.Load<SkillDBObject>("Database/SkillDB");
        if (skillDBObject == null)
        {
            Debug.LogError("Failed to load SkillDB");
            return;
        }

        foreach (var skill in skillDBObject.WeaponSkills)
        {
            _skillDict.Add(skill.ID, skill);
        }
        foreach (var skill in skillDBObject.ArmorSkills)
        {
            _skillDict.Add(skill.ID, skill);
        }
        foreach (var skill in skillDBObject.MagicSkills)
        {
            _skillDict.Add(skill.ID, skill);
        }
        foreach (var skill in skillDBObject.MiscSkills)
        {
            _skillDict.Add(skill.ID, skill);
        }
    }

    public Skill GetSkill(string ID)
    {
        if (!_skillDict.ContainsKey(ID))
        {
            return null;
        }

        return _skillDict[ID];
    }

    public InventorySkill GetInventorySkill(string ID, Skillset skillset)
    {
        if (!_skillDict.ContainsKey(ID))
        {
            return null;
        }

        return new InventorySkill(skillset, _skillDict[ID]);
    }
}
