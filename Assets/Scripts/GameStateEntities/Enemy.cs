using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public interface CombatEntity
{
    string GetName();
    float GetCooldown();
    bool IsConcious();

    Entity3D GetEntity();

    bool WaitForTurn();
    void ActivateTurn();
}

public class Enemy : GameStateEntity, CombatEntity
{
    public int CurrentHP { get; protected set; }
    public EnemyData Data { get; private set; }
    public float Cooldown { get; private set; }
    public bool IsHostile { get; protected set; }
    public NPC NPC { get; protected set; }

    public bool MovementLocked { get; private set; }
    public bool AwaitingTurn { get; private set; }

    public Entity3D GetEntity() => Entity;
    public bool IsConcious() => CurrentHP > 0;

    public event System.Action OnEnemyReady;

    public Enemy(EnemyData data, NPCData npc = null) : base(null)
    {
        Data = data;
        CurrentHP = Data.HitPoints;
        Cooldown = data.CombatData.Recovery;
        IsHostile = Data.IsHostile;
        if(npc != null)
            NPC = new NPC(npc, this);

        TimeManagement.OnTick += Tick;
        TurnController.OnTurnBasedToggled += ToggleCombat;
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
        TimeManagement.OnTick += Tick;
        TurnController.OnTurnBasedToggled += ToggleCombat;
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

    public Sprite GetPortrait()
    {
        if(NPC != null)
        {
            return NPC.Portrait;
        }
        return Data.Portrait;
    }

    public void ToggleCombat(bool enable)
    {
        if (enable)
            Cooldown = Data.CombatData.Recovery;
        else
            Cooldown = 0.1f;
    }

    public void ActivateTurn()
    {
        AwaitingTurn = true;
    }

    public bool WaitForTurn()
    {
        return AwaitingTurn;
    }

    public void Tick(float delta)
    {
        if(Cooldown > 0)
        {
            Cooldown -= delta;
            if(Cooldown <= 0)
            {
                OnEnemyReady?.Invoke();
            }
        }
    }

    public void LockMovement(bool isLocked)
    {
        MovementLocked = isLocked;
    }

    public string GetName()
    {
        return Data.DisplayName;
    }

    public float GetCooldown()
    {
        return Mathf.Max(0,Cooldown);
    }

    public void DoDodge()
    {
        Cooldown = Data.CombatData.Recovery;
        AwaitingTurn = false;
    }

    public void DoAttack()
    {
        List<PartyMember> validTargets = Party.Instance.Members.FindAll(x => x.Vitals.Condition == PartyMemberState.Concious);
        PartyMember member = validTargets[Random.Range(0, validTargets.Count)];
        if(member != null)
            member.OnEnemyAttack(Data);
        Cooldown = Data.CombatData.Recovery;
        AwaitingTurn = false;
    }

    public void OnHit(int damage)
    {
        IsHostile = true;
        CurrentHP -= damage;

        if(CurrentHP <= 0)
        {
            TimeManagement.OnTick -= Tick;
            TurnController.OnTurnBasedToggled -= ToggleCombat;

            EnemyEntity ee = Entity as EnemyEntity;
            ee.OnDeath();
        }
    }
}
