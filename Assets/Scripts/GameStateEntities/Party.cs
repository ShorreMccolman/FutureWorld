using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Linq;

public class Party : GameStateEntity
{
    public PartyMember ActiveMember { get; private set; }
    public string CurrentLocationID { get; private set; }

    public int CurrentFood { get; private set; }
    public int CurrentGold { get; private set; }
    public int CurrentBalance { get; private set; }
    public float CurrentTime { get; private set; }

    List<NPC> _hires;
    public List<NPC> Hires { get { return _hires; } }

    public QuestLog QuestLog { get; private set; }

    private List<PartyMember> _members;
    public List<PartyMember> Members { get { return _members; } }

    private List<string> _visitedLocations;
    public List<string> VisitedLocations { get; private set; }

    public List<string> _monthlyKills;

    public delegate void OnChangeEvent();
    public OnChangeEvent MemberChanged;

    public Party(CharacterData[] characterData) : base(null)
    {
        CurrentLocationID = "Game";

        _members = new List<PartyMember>();
        foreach(var character in characterData)
        {
            PartyMember member = new PartyMember(this, character);
            Members.Add(member);
        }

        Members[0].Inventory.AddItem(ItemDatabase.Instance.GetInventoryItem("letter_1"));

        QuestLog = new QuestLog(this);
        _hires = new List<NPC>();

        TimeManagement.Instance.OnTick += TickEvent;

        CurrentFood = 7;
        CurrentGold = 200;
        CurrentBalance = 0;
        CurrentTime = 0;
        _visitedLocations = new List<string>();
        _monthlyKills = new List<string>();
    }

    public Party(XmlNode node) : base(null, node)
    {
        CurrentLocationID = node.SelectSingleNode("LocationID").InnerText;
        CurrentFood = int.Parse(node.SelectSingleNode("Food").InnerText);
        CurrentGold = int.Parse(node.SelectSingleNode("Gold").InnerText);
        CurrentBalance = int.Parse(node.SelectSingleNode("Balance").InnerText);
        CurrentTime = float.Parse(node.SelectSingleNode("Time").InnerText);
        QuestLog = new QuestLog(this, node.SelectSingleNode("QuestLog"));
        Populate<PartyMember>(ref _members, typeof(Party), node, "Members", "PartyMember");
        Populate<NPC>(ref _hires, typeof(Party), node, "Hires", "NPC");

        TimeManagement.Instance.OnTick += TickEvent;
    }

    public override XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("Party");
        element.AppendChild(XmlHelper.Attribute(doc, "LocationID", CurrentLocationID));
        element.AppendChild(XmlHelper.Attribute(doc, "Food", CurrentFood));
        element.AppendChild(XmlHelper.Attribute(doc, "Gold", CurrentGold));
        element.AppendChild(XmlHelper.Attribute(doc, "Balance", CurrentBalance));
        element.AppendChild(XmlHelper.Attribute(doc, "Time", CurrentTime));
        element.AppendChild(XmlHelper.Attribute(doc, "Members", Members));
        if(Hires.Count > 0)
            element.AppendChild(XmlHelper.Attribute(doc, "Hires", Hires));
        element.AppendChild(QuestLog.ToXml(doc));
        element.AppendChild(base.ToXml(doc));

        return element;
    }

    public Transform DropPosition
    {
        get
        {
            PartyEntity pe = Entity as PartyEntity;
            return pe.DropPosition;
        }
    }

    public void TickEvent(float tick)
    {
        System.DateTime dt = TimeManagement.Instance.GetDT(CurrentTime);
        CurrentTime += tick;
        System.DateTime newDT = TimeManagement.Instance.GetDT(CurrentTime);

        if(dt.Month < newDT.Month)
        {
            _monthlyKills.Clear();
        }
    }

    public void SetActiveMember(PartyMember member)
    {
        ActiveMember = member;
        MemberChanged?.Invoke();
    }

    public void VisitLocation(string locationID)
    {
        VisitedLocations.Add(locationID);
    }

    public void Deposit(int quantity)
    {
        if (quantity > CurrentGold)
            quantity = CurrentGold;

        CurrentGold -= quantity;
        CurrentBalance += quantity;
    }

    public void Withdraw(int quantity)
    {
        if (CurrentBalance == 0)
            return;

        if (quantity > CurrentBalance)
            quantity = CurrentBalance;

        CurrentGold += quantity;
        CurrentBalance -= quantity;
    }

    public void OnEnemyDeath(Enemy enemy)
    {
        foreach(var member in Members)
        {
            if (member.IsConcious())
                member.Profile.EarnXP(enemy.Data.Experience);
        }

        if (!_monthlyKills.Contains(enemy.Data.ID))
            _monthlyKills.Add(enemy.Data.ID);
    }

    public bool HasKilled(string enemyID)
    { 
        return _monthlyKills.Contains(enemyID); 
    }

    public void CollectGold(InventoryItem gold)
    {
        CollectGold(gold.EffectiveValue, true);
    }

    public void CollectGold(int amount, bool payHires = false)
    {
        int paid = 0;
        if (payHires)
        {
            foreach (var hire in Hires)
            {
                paid += Mathf.CeilToInt(amount * hire.Rate * 0.01f);
            }
        }

        CurrentGold += amount - paid;
        if(paid == 0)
            HUD.Instance.SendInfoMessage("You found " + amount + " gold!", 2.0f);
        else
            HUD.Instance.SendInfoMessage("You found " + amount + " gold (followers take " + paid + ")!", 2.0f);
        HUD.Instance.UpdateDisplay();
    }

    public void FillFoodTo(int quantity)
    {
        CurrentFood = quantity;
        HUD.Instance.UpdateDisplay();
    }

    public bool TryHire(NPC npc)
    {
        if(Hires.Count == 2)
        {
            HUD.Instance.SendInfoMessage("I cannot join you, your party is full", 2.0f);
            return false;
        }

        bool success = PartyController.Instance.Party.TryPay(npc.Cost);
        if (success)
        {
            Hires.Add(npc);
            HUD.Instance.UpdateDisplay();
        }
        return success;
    }

    public bool DismissHire(NPC npc)
    {
        if (!Hires.Contains(npc))
            return false;

        Hires.Remove(npc);
        HUD.Instance.UpdateDisplay();
        return true;
    }

    public bool TryEat(int cost)
    {
        if (CurrentFood < cost)
        {
            HUD.Instance.SendInfoMessage("You don't have enough food to rest", 2.0f);
            return false;
        }

        CurrentFood -= cost;
        HUD.Instance.UpdateDisplay();
        return true;
    }

    public bool TryPay(int cost)
    {
        if (CurrentGold < cost) {
            HUD.Instance.SendInfoMessage("You don't have enough gold...", 2.0f);
            return false;
        }

        CurrentGold -= cost;
        HUD.Instance.UpdateDisplay();
        return true;
    }

    public void EarnXP(int value)
    {
        foreach(var member in Members)
        {
            member.Profile.EarnXP(value);
        }
    }

    public bool CheckForItem(string ID)
    {
        bool success = false;

        foreach(var member in Members)
        {
            if(member.Inventory.DoesHaveItem(ID))
            {
                success = true;
                break;
            }
        }

        return success;
    }

    public bool IsDead()
    {
        foreach(var member in Members)
        {
            if (member.Vitals.Condition == PartyMemberState.Good)
                return false;
        }

        return true;
    }

    public bool EnemyAttack(EnemyData data)
    {
        List<PartyMember> validTargets = Members.FindAll(x => x.Vitals.Condition == PartyMemberState.Good);
        PartyMember member = validTargets[Random.Range(0, validTargets.Count)];
        bool success = member.OnEnemyAttack(data);

        if (success)
        {
            HUD.Instance.UpdateDisplay();

            if (IsDead())
                GameController.Instance.TriggerDeath();

        }
        return success;
    }
}