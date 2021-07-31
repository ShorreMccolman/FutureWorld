using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class Projectile : GameStateEntity
{
    public Vector3 Direction { get; protected set; }
    public string Sender { get; protected set; }
    public int Attack { get; protected set; }
    public int Damage { get; protected set; }
    public int Potency { get; protected set; }
    public bool IsFriendly { get; protected set; }
    public AttackType DamageType => _data.DamageType;
    public OnHitEffect OnHit => _data.OnHitEffect;
    public float Speed => _data.Speed;
    public GameObject Prefab => _data.model;
    public bool IsSpell => _data.IsSpell;

    ProjectileData _data;

    public Projectile(ProjectileData data) : base(null)
    {
        _data = data;
    }

    public Projectile(XmlNode node) : base(null, node)
    {
        Direction = XmlHelper.GetVector3(node.SelectSingleNode("Direction"));
        _data = ProjectileDatabase.Instance.GetProjectileData(node.SelectSingleNode("ID").InnerText);
        Sender = node.SelectSingleNode("Sender").InnerText;
        Attack = int.Parse(node.SelectSingleNode("Attack").InnerText);
        Damage = int.Parse(node.SelectSingleNode("Damage").InnerText);
        Potency = int.Parse(node.SelectSingleNode("Potency").InnerText);
        IsFriendly = bool.Parse(node.SelectSingleNode("Friendly").InnerText);
    }

    public override XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("Projectile");
        element.AppendChild(XmlHelper.Attribute(doc, "ID", _data.ID));
        element.AppendChild(XmlHelper.Attribute(doc, "Direction", Direction));
        element.AppendChild(XmlHelper.Attribute(doc, "Sender", Sender));
        element.AppendChild(XmlHelper.Attribute(doc, "Attack", Attack));
        element.AppendChild(XmlHelper.Attribute(doc, "Damage", Damage));
        element.AppendChild(XmlHelper.Attribute(doc, "Potency", Potency));
        element.AppendChild(XmlHelper.Attribute(doc, "Friendly", IsFriendly));
        element.AppendChild(base.ToXml(doc));
        return element;
    }

    public void SetOnHitPotency(int potency)
    {
        Potency = potency;
    }

    public void SetDamage(string sender, int attack, int damage)
    {
        Sender = sender;
        Attack = attack;
        Damage = damage;
    }

    public void Setup(Vector3 direction, bool isFriendly)
    {
        Direction = direction;
        IsFriendly = isFriendly;
    }
}
