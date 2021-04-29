using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;

public class QuestChain : GameStateEntity
{
    public int CurrentStage { get; protected set; }
    public bool IsComplete { get; protected set; }

    public QuestData Data { get; protected set; }

    public QuestChain(GameStateEntity parent, QuestData data) : base(parent)
    {
        Data = data;
        CurrentStage = 0;
        IsComplete = false;
    }

    public QuestChain(GameStateEntity parent, XmlNode node) : base(parent, node)
    {
        Data = QuestDatabase.Instance.GetQuest((QuestLine)int.Parse(node.SelectSingleNode("ID").InnerText));
        CurrentStage = int.Parse(node.SelectSingleNode("Stage").InnerText);
        IsComplete = bool.Parse(node.SelectSingleNode("IsComplete").InnerText);
    }

    public override XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("Quest");
        element.AppendChild(XmlHelper.Attribute(doc, "ID", (int)Data.ID));
        element.AppendChild(XmlHelper.Attribute(doc, "Stage", CurrentStage));
        element.AppendChild(XmlHelper.Attribute(doc, "IsComplete", IsComplete));
        element.AppendChild(base.ToXml(doc));

        return element;
    }

    public bool ProgressQuest()
    {
        CurrentStage++;
        if (CurrentStage == Data.LogDescriptions.Length)
            IsComplete = true;

        return IsComplete;
    }

    public string GetCurrentStageDescription()
    {
        if (IsComplete)
            return Data.AwardDescription;

        return Data.LogDescriptions[CurrentStage];
    }
}
