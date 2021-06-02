using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class Topic : GameStateEntity
{
    public string Header { get { return _data.Header; } }
    public string Body { get { return _data.Body; } }

    TopicData _data;
    string _id;
    int _index;


    public Topic(TopicData data, string ID, int index, GameStateEntity parent) : base(parent)
    {
        _data = data;
        _id = ID;
        _index = index;
    }

    public Topic(GameStateEntity parent, XmlNode node) : base(parent, node)
    {
        _id = node.SelectSingleNode("ID").InnerText;
        _index = int.Parse(node.SelectSingleNode("Index").InnerText);

        _data = TopicDatabase.Instance.GetTopicData(_id, _index);
    }

    public override XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("Topic");
        element.AppendChild(XmlHelper.Attribute(doc, "ID", _id));
        element.AppendChild(XmlHelper.Attribute(doc, "Index", _index));
        element.AppendChild(base.ToXml(doc));
        return element;
    }
}
