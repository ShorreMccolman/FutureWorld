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

    public QuestLog QuestLog { get; private set; }

    private List<PartyMember> _members;
    public List<PartyMember> Members { get { return _members; } }

    private List<string> _visitedLocations;
    public List<string> VisitedLocations { get; private set; }

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

        TimeManagement.Instance.OnTick += TickEvent;

        CurrentFood = 7;
        CurrentGold = 200;
        CurrentBalance = 0;
        CurrentTime = 0;
        _visitedLocations = new List<string>();
    }

    public Party(XmlNode node) : base(null, node)
    {
        CurrentLocationID = node.SelectSingleNode("LocationID").InnerText;
        CurrentFood = int.Parse(node.SelectSingleNode("Food").InnerText);
        CurrentGold = int.Parse(node.SelectSingleNode("Gold").InnerText);
        CurrentBalance = int.Parse(node.SelectSingleNode("Balance").InnerText);
        CurrentTime = int.Parse(node.SelectSingleNode("Time").InnerText);
        QuestLog = new QuestLog(this, node.SelectSingleNode("QuestLog"));
        Populate<PartyMember>(ref _members, typeof(Party), node, "Members", "PartyMember");

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
        CurrentTime += tick;
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

    public void CollectGold(InventoryItem gold)
    {
        CollectGold(gold.EffectiveValue);
    }

    public void CollectGold(int amount)
    {
        CurrentGold += amount;
        HUD.Instance.SendInfoMessage("You found " + amount + " gold!", 2.0f);
        HUD.Instance.UpdateDisplay();
    }

    public void FillFoodTo(int quantity)
    {
        CurrentFood = quantity;
        HUD.Instance.UpdateDisplay();
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