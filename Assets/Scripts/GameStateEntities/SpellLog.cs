using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class SpellLog : GameStateEntity
{
    Dictionary<SpellSchool, List<int>> _knownSpells;
    public Dictionary<SpellSchool, List<int>> Spells { get { return _knownSpells; } }

    Skillset _skillset;

    public SpellLog(GameStateEntity parent, Skillset skillset, CharacterData data) : base(parent)
    {
        _skillset = skillset;
        _knownSpells = new Dictionary<SpellSchool, List<int>>();

        for(int i=0;i<(int)SpellSchool.Count;i++)
        {
            if(data.HasSkill(((SpellSchool)i).ToString()))
            {
                LearnSpell((SpellSchool)i, 0);
            }
        }
    }

    public SpellLog(GameStateEntity parent, Skillset skillset, XmlNode node) : base(parent, node)
    {
        _skillset = skillset;
        _knownSpells = new Dictionary<SpellSchool, List<int>>();

        XmlNodeList schoolNodes = node.SelectNodes("Schools");
        for(int i=0;i< schoolNodes.Count;i++)
        {
            XmlNode school = schoolNodes.Item(i);
            List<int> spells = new List<int>();

            XmlNodeList spellNodes = school.SelectNodes("ID");
            for(int j=0;j<spellNodes.Count;j++)
            {
                spells.Add(int.Parse(spellNodes.Item(j).Name));
            }

            _knownSpells.Add((SpellSchool)int.Parse(school.Name), spells);
        }
    }

    public override XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("SpellLog");
        XmlNode schools = doc.CreateElement("Schools");
        foreach(var school in _knownSpells.Keys)
        {
            schools.AppendChild(XmlHelper.Attribute(doc, ((int)school).ToString(), "ID", _knownSpells[school]));
        }
        element.AppendChild(schools);
        element.AppendChild(base.ToXml(doc));

        return element;
    }

    public bool KnowsSpell(SpellSchool school, int ID)
    {
        if (!_knownSpells.ContainsKey(school))
            return false;

        return _knownSpells[school].Contains(ID);
    }

    public bool LearnSpell(SpellSchool school, int number)
    {
        if (KnowsSpell(school, number))
            return false;

        SpellData data = SpellDatabase.Instance.GetSpell(school, number);

        if (!_skillset.KnowsSkill(data.School.ToString()))
            return false;

        if (!_knownSpells.ContainsKey(data.School))
            _knownSpells.Add(data.School, new List<int>() { number });
        else
            _knownSpells[data.School].Add(number);

        return true;
    }
}
