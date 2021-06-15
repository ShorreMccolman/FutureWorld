using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class Resident : GameStateEntity
{
    public List<int> OptionProgress;

    public ResidentData Data { get; protected set; }

    public string BountyID { get; protected set; }

    public Resident(GameStateEntity parent, ResidentData data) : base(parent)
    {
        Data = data;

        OptionProgress = new List<int>();
        foreach (var stage in data.Options)
        {
            OptionProgress.Add(0);
        }
        BountyID = "";
    }

    public Resident(GameStateEntity parent, XmlNode node) : base(parent, node)
    {
        OptionProgress = new List<int>();
        XmlNodeList nodes = node.SelectNodes("Progress");
        for (int i = 0; i < nodes.Count; i++)
            OptionProgress.Add((int)int.Parse(nodes.Item(i).InnerText));
        BountyID = node.SelectSingleNode("Bounty").InnerText;
    }

    public void RestoreDataAfterLoad(ResidentData data) { Data = data; }

    public override XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("Resident");
        foreach (var res in OptionProgress)
        {
            element.AppendChild(XmlHelper.Attribute(doc, "Progress", res));
        }
        element.AppendChild(XmlHelper.Attribute(doc, "Bounty", BountyID));
        element.AppendChild(base.ToXml(doc));

        return element;
    }

    public void ProgressOption(int option)
    {
        DialogStep step = Data.Options[option].Steps[OptionProgress[option]];

        if (step.GoldReceived > 0)
            Party.Instance.CollectGold(step.GoldReceived);

        if (OptionProgress[option] + 1 < Data.Options[option].Steps.Count)
        {
            OptionProgress[option]++;
        }
    }

    public void TryUpdateBounty(float currentTime)
    {
        System.DateTime date = TimeManagement.Instance.GetDT(currentTime);
        System.DateTime update = TimeManagement.Instance.GetDT(LastUpdate);

        if (BountyID == "" || date.Year > update.Year || date.Month > update.Month)
        {
            BountyID = EnemyDatabase.Instance.GetRandomEnemy().Data.ID;
        }
        LastUpdate = currentTime;
    }

    public void CompleteBounty()
    {
        BountyID = "complete";
    }

    public int GetOptionProgress(int option)
    {
        return OptionProgress[option];
    }
}