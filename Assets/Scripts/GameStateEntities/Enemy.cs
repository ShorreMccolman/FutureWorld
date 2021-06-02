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

public class Enemy : GameStateEntity, CombatEntity
{
    public int CurrentHP { get; protected set; }

    public EnemyData Data { get; private set; }

    public float Cooldown { get; private set; }

    public float MoveCooldown { get; set; }

    public bool MovementLocked { get; private set; }

    public bool IsHostile { get; protected set; }

    public NPC NPC { get; protected set; }

    public Enemy(EnemyData data, NPC npc = null) : base(null)
    {
        Data = data;
        CurrentHP = Data.HitPoints;
        Cooldown = data.CombatData.Recovery;
        MoveCooldown = 0;
        IsHostile = Data.IsHostile;
        NPC = npc;
    }

    public Enemy(XmlNode node) : base(null, node)
    {
        Data = EnemyDatabase.Instance.GetEnemyData(node.SelectSingleNode("ID").InnerText);
        CurrentHP = int.Parse(node.SelectSingleNode("Health").InnerText);
        Cooldown = float.Parse(node.SelectSingleNode("Cooldown").InnerText);
        IsHostile = bool.Parse(node.SelectSingleNode("Hostile").InnerText);

        XmlNode npc = node.SelectSingleNode("NPC");
        if(npc != null)
        {
            NPC = new NPC(this, npc);
        }
    }

    public override XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("Enemy");
        element.AppendChild(XmlHelper.Attribute(doc, "ID", Data.ID));
        element.AppendChild(XmlHelper.Attribute(doc, "Health", CurrentHP));
        element.AppendChild(XmlHelper.Attribute(doc, "Cooldown", Cooldown));
        element.AppendChild(XmlHelper.Attribute(doc, "Hostile", IsHostile));
        if(NPC != null)
            element.AppendChild(NPC.ToXml(doc));
        element.AppendChild(base.ToXml(doc));
        return element;
    }

    public void InitNPC(NPCData data)
    {
        NPC = new NPC(data, this);
    }

    public void CombatStep()
    {
        MoveCooldown = 3f;
    }
    public bool TickCooldown(float delta)
    {
        if (CurrentHP < 0)
            return false;

        bool wasOn = Cooldown > 0;
        if (!wasOn)
            return false;

        Cooldown -= delta;
        if (Cooldown < 0)
            Cooldown = 0;
        return Cooldown <= 0;
    }

    public float GetCooldown()
    {
        return Cooldown;
    }

    public void LockMovement(bool isLocked)
    {
        MovementLocked = isLocked;
        MoveCooldown = 0;
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
