using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpellSchool
{
    Fire,
    Air,
    Water,
    Earth,

    Spirit,
    Mind,
    Body,

    Light,
    Dark,

    Count
}

[System.Serializable]
public class SpellData
{
    public string ID;
    public int Number;
    public int SPCost;
    public string DisplayName;
    [TextArea(minLines: 2, maxLines: 15)]
    public string Description;
    public string[] MasteryDescriptions;
    public SpellSchool School;
    public Sprite Icon;
    public SpellBehaviour Behaviour;
}

public class SpellDatabase
{
    public static SpellDatabase Instance;

    Dictionary<SpellSchool, Dictionary<string, SpellData>> _spellDict;

    public SpellDatabase()
    {
        Instance = this;

        _spellDict = new Dictionary<SpellSchool, Dictionary<string, SpellData>>();

        SpellDBObject[] spellDBObjects = Resources.LoadAll<SpellDBObject>("Database/Spells");
        if (spellDBObjects == null)
        {
            Debug.LogError("Failed to load any Spell DB");
            return;
        }

        foreach (var obj in spellDBObjects)
        {
            Dictionary<string, SpellData> spells = new Dictionary<string, SpellData>();
            foreach (var spell in obj.GetSpells())
            {
                spells.Add(spell.ID, spell);
            }
            _spellDict.Add(obj.GetSchool(), spells);
        }
    }

    public string GetSpellName(string ID)
    {
        foreach (var dict in _spellDict.Values)
        {
            if (dict.ContainsKey(ID))
            {
                return dict[ID].DisplayName;
            }
        }

        return "None";
    }

    public SpellData GetSpell(string ID)
    {
        if (string.IsNullOrEmpty(ID))
            return null;

        foreach(var dict in _spellDict.Values)
        {
            if (dict.ContainsKey(ID))
            {
                return dict[ID];
            }
        }

        return null;
    }

    public SpellData GetSpell(SpellSchool school, int ID)
    {
        foreach (var spell in _spellDict[school].Values)
        {
            if (spell.Number == ID)
                return spell;
        }

        return null;
    }

    public List<SpellData> GetAllSpellsForSchool(SpellSchool school)
    {
        List<SpellData> spells = new List<SpellData>();
        foreach(var data in _spellDict[school].Values)
        {
            spells.Add(data);
        }

        return spells;
    }
}

