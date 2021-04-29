using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class History : GameStateEntity {

    public History(GameStateEntity parent, CharacterData data) : base(parent)
    {

    }

    public History(GameStateEntity parent, XmlNode node) : base(parent, node)
    {

    }

    public override XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("History");

        element.AppendChild(base.ToXml(doc));
        return element;
    }
}
