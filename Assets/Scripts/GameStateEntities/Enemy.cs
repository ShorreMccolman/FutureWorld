using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class Enemy : GameStateEntity, CombatEntity
{
    public int CurrentHP { get; protected set; }

    public EnemyData Data { get; private set; }

    public float Cooldown { get; private set; }

    public bool MovementLocked { get; private set; }

    public Enemy(EnemyData data) : base(null)
    {
        Data = data;
        CurrentHP = Data.HitPoints;
        Cooldown = data.CombatData.Recovery;
    }

    public Enemy(XmlNode node) : base(null, node)
    {
        CurrentHP = int.Parse(node.SelectSingleNode("Health").InnerText);
        Cooldown = float.Parse(node.SelectSingleNode("Cooldown").InnerText);
    }

    public override XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("Enemy");
        element.AppendChild(XmlHelper.Attribute(doc, "Health", CurrentHP));
        element.AppendChild(XmlHelper.Attribute(doc, "Cooldown", Cooldown));
        element.AppendChild(base.ToXml(doc));
        return element;
    }

    public bool TickCooldown(float delta)
    {
        if (CurrentHP < 0)
            return false;

        bool wasOn = Cooldown > 0;
        if (!wasOn)
            return false;

        Cooldown -= delta;
        return Cooldown <= 0;
    }

    public float GetCooldown()
    {
        return Cooldown;
    }

    public void LockMovement(bool isLocked)
    {
        MovementLocked = isLocked;
    }

    public void DoAttack()
    {
        PartyController.Instance.Party.EnemyAttack(Data);
        Cooldown = Data.CombatData.Recovery;
    }

    public void OnHit(int damage)
    {
        CurrentHP -= damage;
        if(CurrentHP <= 0)
        {
            EnemyEntity ee = Entity as EnemyEntity;
            ee.OnDeath();
        }
    }
}
