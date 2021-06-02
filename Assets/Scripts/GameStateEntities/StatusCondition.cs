using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;


public class StatusCondition : GameStateEntity
{
    public StatusEffectOption Option { get; private set; }
    public StatusEffect Effect { get; private set; }
    public float Duration { get; private set; }
    public int Potency { get; private set; }

    public StatusCondition(GameStateEntity parent, StatusEffect data, float duration) : base(parent)
    {
        Option = data.Option;
        Effect = data;
        Duration = duration;
        Potency = 0;
    }

    public StatusCondition(GameStateEntity parent, StatusEffect data, int potency, float duration) : base(parent)
    {
        Option = data.Option;
        Effect = data;
        Duration = duration;
        Potency = potency;
    }

    public StatusCondition(GameStateEntity parent, XmlNode node) : base(parent, node)
    {
        XmlNode skillNode = node.SelectSingleNode("StatusCondition");
        Option = (StatusEffectOption)int.Parse(node.SelectSingleNode("Option").InnerText);
        Duration = float.Parse(node.SelectSingleNode("Duration").InnerText);
        Potency = int.Parse(node.SelectSingleNode("Potency").InnerText);
        Effect = StatusEffectDatabase.Instance.GetEffect(Option);
    }

    public override XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("StatusCondition");
        element.AppendChild(XmlHelper.Attribute(doc, "Option", (int)Option));
        element.AppendChild(XmlHelper.Attribute(doc, "Duration", Duration));
        element.AppendChild(XmlHelper.Attribute(doc, "Potency", Potency));
        element.AppendChild(base.ToXml(doc));
        return element;
    }

    public bool Tick(float delta)
    {
        Duration += Effect.TicksUp ? delta : -delta;
        return Duration < 0;
    }
}
