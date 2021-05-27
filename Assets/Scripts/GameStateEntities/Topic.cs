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

    public Topic(XmlNode node, GameStateEntity parent) : base(parent, node)
    {

    }

    public override XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("Topic");

        return element;
    }
}
