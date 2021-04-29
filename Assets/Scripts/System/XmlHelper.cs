using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;

public struct TransformData
{
    public Vector3 Position { get; private set; }
    public Quaternion Rotation { get; private set; }

    public TransformData(Vector3 position, Quaternion rotation)
    {
        Position = position;
        Rotation = rotation;
    }
}

public static class XmlHelper {

    public static XmlNode Attribute(XmlDocument document, string valueName, Transform value)
    {
        XmlNode nameNode = document.CreateElement(valueName);
        nameNode.AppendChild(Attribute(document, "Position", value.position));
        nameNode.AppendChild(Attribute(document, "Rotation", value.rotation));
        return nameNode;
    }

    public static XmlNode Attribute(XmlDocument document, string valueName, Vector3 value)
    {
        XmlNode nameNode = document.CreateElement(valueName);
        XmlNode x = document.CreateElement("x");
        x.InnerText = value.x.ToString();
        nameNode.AppendChild(x);
        XmlNode y = document.CreateElement("y");
        y.InnerText = value.y.ToString();
        nameNode.AppendChild(y);
        XmlNode z = document.CreateElement("z");
        z.InnerText = value.z.ToString();
        nameNode.AppendChild(z);
        return nameNode;
    }

    public static XmlNode Attribute(XmlDocument document, string valueName, Quaternion value)
    {
        XmlNode nameNode = document.CreateElement(valueName);
        XmlNode x = document.CreateElement("x");
        x.InnerText = value.eulerAngles.x.ToString();
        nameNode.AppendChild(x);
        XmlNode y = document.CreateElement("y");
        y.InnerText = value.eulerAngles.y.ToString();
        nameNode.AppendChild(y);
        XmlNode z = document.CreateElement("z");
        z.InnerText = value.eulerAngles.z.ToString();
        nameNode.AppendChild(z);
        return nameNode;
    }

    public static XmlNode Attribute(XmlDocument document, string collectionName, string valueName, List<string> entities)
    {
        XmlNode nameNode = document.CreateElement(collectionName);
        if (entities == null)
            Debug.LogError(collectionName + " is null");
        foreach (var entity in entities)
        {
            nameNode.AppendChild(Attribute(document, valueName, entity));
        }
        return nameNode;
    }

    public static XmlNode Attribute(XmlDocument document, string collectionName, string valueName, List<int> entities)
    {
        XmlNode nameNode = document.CreateElement(collectionName);
        if (entities == null)
            Debug.LogError(collectionName + " is null");
        foreach (var entity in entities)
        {
            nameNode.AppendChild(Attribute(document, valueName, entity));
        }
        return nameNode;
    }

    public static XmlNode Attribute<T>(XmlDocument document, string collectionName, List<T> entities) where T : GameStateEntity
    {
        XmlNode nameNode = document.CreateElement(collectionName);
        foreach (var entity in entities)
        {
            nameNode.AppendChild(entity.ToXml(document));
        }
        return nameNode;
    }

    public static XmlNode Attribute(XmlDocument document, string valueName, string value)
    {
        XmlNode nameNode = document.CreateElement(valueName);
        nameNode.InnerText = value;
        return nameNode;
    }

    public static XmlNode Attribute(XmlDocument document, string valueName, bool value)
    {
        XmlNode nameNode = document.CreateElement(valueName);
        nameNode.InnerText = value.ToString();
        return nameNode;
    }

    public static XmlNode Attribute(XmlDocument document, string valueName, int value)
    {
        XmlNode nameNode = document.CreateElement(valueName);
        nameNode.InnerText = value.ToString();
        return nameNode;
    }

    public static XmlNode Attribute(XmlDocument document, string valueName, float value)
    {
        XmlNode nameNode = document.CreateElement(valueName);
        nameNode.InnerText = value.ToString();
        return nameNode;
    }

    public static TransformData GetTransformData(XmlNode node)
    {
        if (node == null)
            return new TransformData(Vector3.zero, Quaternion.identity);

        Vector3 pos = GetVector3(node.SelectSingleNode("Position"));
        Quaternion rot = GetQuaternion(node.SelectSingleNode("Rotation"));
        return new TransformData(pos, rot);
    }

    public static Vector3 GetVector3(XmlNode node)
    {
        float x = float.Parse(node.SelectSingleNode("x").InnerText);
        float y = float.Parse(node.SelectSingleNode("y").InnerText);
        float z = float.Parse(node.SelectSingleNode("z").InnerText);

        return new Vector3(x, y, z);
    }

    public static Quaternion GetQuaternion(XmlNode node)
    {
        float x = float.Parse(node.SelectSingleNode("x").InnerText);
        float y = float.Parse(node.SelectSingleNode("y").InnerText);
        float z = float.Parse(node.SelectSingleNode("z").InnerText);

        return Quaternion.Euler(x, y, z);
    }

}
