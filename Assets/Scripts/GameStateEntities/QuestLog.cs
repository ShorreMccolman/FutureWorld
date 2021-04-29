using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class QuestLog : GameStateEntity
{
    List<QuestChain> _activeQuests;
    public List<QuestChain> Quests { get { return _activeQuests; } }

    List<string> _guildMemberships;
    public List<string> GuildMemberships { get; private set; }

    List<QuestChain> _completedQuests;

    public QuestLog(GameStateEntity parent) : base(parent)
    {
        _activeQuests = new List<QuestChain>();
        _completedQuests = new List<QuestChain>();

        _guildMemberships = new List<string>();

        AcceptQuest(QuestLine.Letter);
    }

    public QuestLog(GameStateEntity parent, XmlNode node) : base(parent, node)
    {
        Populate<QuestChain>(ref _activeQuests, typeof(QuestLog), node, "Active", "Quest");
        Populate<QuestChain>(ref _completedQuests, typeof(QuestLog), node, "Completed", "Quest");
        Populate<string>(ref _guildMemberships, typeof(string), node, "Memberships", "ID");
    }

    public override XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("QuestLog");
        element.AppendChild(XmlHelper.Attribute(doc, "Active", _activeQuests));
        element.AppendChild(XmlHelper.Attribute(doc, "Completed", _completedQuests));
        element.AppendChild(XmlHelper.Attribute(doc, "Memberships", "ID", GuildMemberships));
        element.AppendChild(base.ToXml(doc));

        return element;
    }

    public bool AcceptQuest(QuestLine quest)
    {
        QuestChain active = _activeQuests.Find(x => x.Data.ID == quest);
        if (active != null)
        {
            Debug.LogError("Already accepted " + quest.ToString());
            return false;
        }

        QuestChain completed = _completedQuests.Find(x => x.Data.ID == quest);
        if (completed != null)
        {
            Debug.LogError("Already completed " + quest.ToString());
            return false;
        }

        _activeQuests.Add(new QuestChain(this, QuestDatabase.Instance.GetQuest(quest)));
        return true;
    }

    public bool ProgressQuest(QuestLine quest)
    {
        QuestChain active = _activeQuests.Find(x => x.Data.ID == quest);
        if (active == null)
        {
            Debug.LogError("Have not accepted " + quest.ToString());
            return false;
        }

        QuestChain completed = _completedQuests.Find(x => x.Data.ID == quest);
        if (completed != null)
        {
            Debug.LogError("Already completed " + quest.ToString());
            return false;
        }

        return active.ProgressQuest();
    }

    public bool CompleteQuest(QuestLine quest)
    {
        QuestChain active = _activeQuests.Find(x => x.Data.ID == quest);
        if (active == null)
        {
            Debug.LogError("Have not accepted " + quest.ToString());
            return false;
        }

        QuestChain completed = _completedQuests.Find(x => x.Data.ID == quest);
        if (completed != null)
        {
            Debug.LogError("Already completed " + quest.ToString());
            return false;
        }

        _activeQuests.Remove(active);
        _completedQuests.Add(active);
        return true;
    }

    public bool IsMemberOfGuild(string ID)
    {
        if (ID == null || ID == "" || ID.Length < 2)
            return true;

        return _guildMemberships.Contains(ID);
    }

    public bool JoinGuild(string ID)
    {
        if (IsMemberOfGuild(ID))
            return false;

        _guildMemberships.Add(ID);
        return true;
    }
}
