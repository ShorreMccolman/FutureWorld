using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;

public interface CombatEntity
{
    float MoveCooldown { get; set; }

    float GetCooldown();

    void CombatStep();
}

public class GameStateEntity {

    public GameStateEntity Parent { get; private set; }
    public Entity3D Entity { get; private set; }
    public System.Guid GSEID { get; private set; }

    protected TransformData _savePos;

    public GameStateEntity(GameStateEntity parent)
    {
        Parent = parent;
        if (Parent == null) GameStateManager.Instance.RegisterParentEntity(this);
        GSEID = System.Guid.NewGuid();
    }

    public GameStateEntity(GameStateEntity parent, XmlNode node) : this(parent)
    {
        Parent = parent;

        XmlNode stateNode = node.SelectSingleNode("EntityState");

        GSEID = new System.Guid(stateNode.SelectSingleNode("GSEID").InnerText);
        _savePos = XmlHelper.GetTransformData(stateNode.SelectSingleNode("Position"));
    }

    ~GameStateEntity()
    {
        GameStateManager.Instance.Unregister(this);
    }

    public bool Reparent(GameStateEntity parent)
    {
        if (parent == Parent)
            return false;

        if(Parent == null)
            GameStateManager.Instance.Unregister(this);

        Parent = parent;

        if (Parent == null)
            GameStateManager.Instance.RegisterParentEntity(this);

        OnReparent();
        return true;
    }

    protected virtual void OnReparent() { }

    public virtual XmlNode ToXml(XmlDocument doc)
    {
        XmlNode node = doc.CreateElement("EntityState");
        if(Entity != null)
            node.AppendChild(XmlHelper.Attribute(doc, "Position", Entity.transform));
        node.AppendChild(XmlHelper.Attribute(doc, "GSEID", GSEID.ToString()));

        return node;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is GameStateEntity))
            return false;

        GameStateEntity entity = (GameStateEntity)obj;

        return GSEID.Equals(entity.GSEID);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public void CreateEntity(GameObject PartyEntityObject)
    {
        PartyEntityObject.transform.position = _savePos.Position;
        PartyEntityObject.transform.rotation = _savePos.Rotation;
        Entity = PartyEntityObject.GetComponent<Entity3D>();
    }

    public void CreateEntity(GameObject PartyEntityObject, Transform Spawn)
    {
        PartyEntityObject.transform.position = Spawn.position;
        PartyEntityObject.transform.rotation = Spawn.rotation;
        Entity = PartyEntityObject.GetComponent<Entity3D>();
    }

    public void CreateEntity(GameObject PartyEntityObject, Vector3 Position)
    {
        PartyEntityObject.transform.position = Position;
        PartyEntityObject.transform.rotation = Quaternion.identity;
        Entity = PartyEntityObject.GetComponent<Entity3D>();
    }

    protected void Populate<T>(ref List<T> list, System.Type entityType, XmlNode node, string groupName, string itemName)
    {
        list = new List<T>();
        var constTypes = new System.Type[] { entityType, typeof(XmlNode) };

        XmlNode itemsNode = node.SelectSingleNode(groupName);
        if (itemsNode != null)
        {
            XmlNodeList itemNodeList = itemsNode.SelectNodes(itemName);
            for (int i = 0; i < itemNodeList.Count; i++)
            {
                object[] args = new object[] { this, itemNodeList.Item(i) };
                T t = (T)typeof(T).GetConstructor(constTypes).Invoke(args);
                list.Add(t);
            }
        }
    }

    protected int ParseInt(XmlNode node, string name)
    {
        return int.Parse(node.SelectSingleNode(name).InnerText);
    }
}

//<Party>
//    <Member>
//        <HP>30</HP>
//        <Name>bob</Name>
//    </Member>
//    <Member>
//        <HP>10</HP>
//        <Name>barb</Name>
//    </Member>
//</Party>
