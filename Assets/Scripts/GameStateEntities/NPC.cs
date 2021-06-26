using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class NPC : GameStateEntity
{
    public string Name { get; protected set; }

    public Sprite Portrait { get; protected set; }
    string _portraitID;

    public string DisplayName { get { return Name + " the " + _data.ProfessionName; } }
    public string JoinText { get { return _data.JoinText; } }
    public string ActionText { get { return _data.ActionText + " " + Name + " takes " + _data.Rate + " percent of all gold you find."; } }

    public int Cost { get { return _data.Cost; } }
    public int Rate { get { return _data.Rate; } }
    public Profession Profession { get { return _data.Profession; } }

    List<Topic> _topics;
    public List<Topic> Topics { get { return _topics; } }

    NPCData _data;

    public static event System.Action<NPC, bool> OnNPCConverse;

    public NPC(NPCData data, Enemy enemy) : base(enemy)
    {
        int rand = Random.Range(0, GameConstants.RandomNPCNamesFemale.Length);

        Sprite[] sprites = SpriteHandler.FetchTemp("NPC");
        List<Sprite> options = new List<Sprite>();
        foreach (var sprite in sprites)
        {
            if (enemy.Data.isFemale)
            {
                if (sprite.name.Contains("female"))
                {
                    options.Add(sprite);
                }
            }
            else
            {
                if (!sprite.name.Contains("female"))
                {
                    options.Add(sprite);
                }
            }
        }

        Portrait = options[Random.Range(0, options.Count)];
        _portraitID = Portrait.name;

        if (enemy.Data.isFemale)
            Name = GameConstants.RandomNPCNamesFemale[rand];
        else
            Name = GameConstants.RandomNPCNamesMale[rand];

        _topics = TopicDatabase.Instance.GetNewsTopics(2, this);

        _data = data;
    }

    public NPC(GameStateEntity enemy, XmlNode node) : base(enemy, node)
    {
        Name = node.SelectSingleNode("Name").InnerText;
        _data = NPCDatabase.Instance.GetNPCData(node.SelectSingleNode("ID").InnerText);
        _portraitID = node.SelectSingleNode("Portrait").InnerText;
        Portrait = SpriteHandler.FetchSprite("NPC", _portraitID);
        Populate<Topic>(ref _topics, typeof(NPC), node, "Topics", "Topic");
    }

    public override XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("NPC");
        element.AppendChild(XmlHelper.Attribute(doc, "Name", Name));
        element.AppendChild(XmlHelper.Attribute(doc, "ID", _data.ID));
        element.AppendChild(XmlHelper.Attribute(doc, "Portrait", _portraitID));
        element.AppendChild(XmlHelper.Attribute(doc, "Topics", Topics));
        element.AppendChild(base.ToXml(doc));
        return element;
    }

    public void Converse(bool isHire)
    {
        OnNPCConverse?.Invoke(this, isHire);
    }
}
