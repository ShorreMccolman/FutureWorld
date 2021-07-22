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
    public bool IsFriendly { get; protected set; }
    public AttackType DamageType => _data.DamageType;
    public float Speed => _data.Speed;
    public GameObject Prefab => _data.model;

    ProjectileData _data;

    public Projectile(ProjectileData data) : base(null)
    {
        _data = data;
    }

    public Projectile(XmlNode node) : base(null, node)
    {
        
    }

    public override XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("Projectile");
        element.AppendChild(base.ToXml(doc));
        return element;
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
