using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class NPC : GameStateEntity
{
    public string Name { get; protected set; }

    public string DisplayName { get { return Name + " the " + _data.ProfessionName; } }
    public string JoinText { get { return _data.JoinText; } }
    public string ActionText { get { return _data.ActionText + " " + Name + " takes " + _data.Rate + " percent of all gold you find."; } }

    public int Cost { get { return _data.Cost; } }
    public int Rate { get { return _data.Rate; } }

    public List<Topic> Topics { get; protected set; }

    NPCData _data;

    public NPC(NPCData data, Enemy enemy) : base(enemy)
    {
        int rand = Random.Range(0, GameConstants.RandomNPCNamesFemale.Length);
        Name = GameConstants.RandomNPCNamesFemale[rand];

        Topics = TopicDatabase.Instance.GetNewsTopics(2, this);

        _data = data;
    }

    public NPC(XmlNode node, Enemy enemy) : base(enemy, node)
    {

    }

    public override XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("NPC");

        return element;
    }
}
