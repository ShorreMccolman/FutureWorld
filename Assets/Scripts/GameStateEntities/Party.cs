using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Linq;

public class Party : GameStateEntity
{
    public static Party Instance { get; private set; }

    public PartyMember ActiveMember { get; private set; }
    public string CurrentLocationID { get; private set; }
    public int CurrentReputation { get; private set; }

    public float CurrentTime { get; private set; }
    public QuestLog QuestLog { get; private set; }

    int _currentFood;
    public int CurrentFood => _currentFood;

    int _currentGold;
    public int CurrentGold => _currentGold;

    int _currentBalance;
    public int CurrentBalance => _currentBalance;

    List<NPC> _hires;
    public List<NPC> Hires => _hires;

    List<PartyMember> _members;
    public List<PartyMember> Members => _members;

    List<string> _visitedLocations;
    List<string> _monthlyKills;

    public static event System.Action<PartyMember> OnMemberChanged;
    public static event System.Action<int, int> OnFundsChanged;
    public static event System.Action<int> OnFoodChanged;
    public static event System.Action<List<NPC>> OnHiresChanged;

    public Party(CharacterData[] characterData) : base(null)
    {
        Init();

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

        CurrentReputation = 0;
        _currentFood = 7;
        _currentGold = 200;
        _currentBalance = 0;
        CurrentTime = 0;
        _visitedLocations = new List<string>();
        _monthlyKills = new List<string>();
    }

    public Party(XmlNode node) : base(null, node)
    {
        Init();

        CurrentLocationID = node.SelectSingleNode("LocationID").InnerText;
        CurrentReputation = int.Parse(node.SelectSingleNode("Reputation").InnerText);
        _currentFood = int.Parse(node.SelectSingleNode("Food").InnerText);
        _currentGold = int.Parse(node.SelectSingleNode("Gold").InnerText);
        _currentBalance = int.Parse(node.SelectSingleNode("Balance").InnerText);
        CurrentTime = float.Parse(node.SelectSingleNode("Time").InnerText);
        QuestLog = new QuestLog(this, node.SelectSingleNode("QuestLog"));
        Populate<PartyMember>(ref _members, typeof(Party), node, "Members", "PartyMember");
        Populate<NPC>(ref _hires, typeof(Party), node, "Hires", "NPC");
    }

    void Init()
    {
        Instance = this;

        EnemyEntity.OnEnemyDeath += EnemyDeath;
        EnemyEntity.OnEnemyPickup += PickupEnemy;
        ChestEntity.OnInspectChest += InspectChest;
    }

    public override XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("Party");
        element.AppendChild(XmlHelper.Attribute(doc, "LocationID", CurrentLocationID));
        element.AppendChild(XmlHelper.Attribute(doc, "Reputation", CurrentReputation));
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

    void UpdateFunds(int goldDifference, int balanceDifference)
    {
        _currentGold += goldDifference;
        _currentBalance += balanceDifference;
        OnFundsChanged?.Invoke(_currentGold, _currentBalance);
    }

    void UpdateFood(int foodDifference)
    {
        _currentFood += foodDifference;
        OnFoodChanged?.Invoke(_currentFood);
    }

    public void SetActiveMember(PartyMember member)
    {
        ActiveMember = member;
        OnMemberChanged?.Invoke(member);
    }

    public void VisitLocation(string locationID)
    {
        _visitedLocations.Add(locationID);
    }

    public void Deposit(int quantity)
    {
        if (quantity > CurrentGold)
            quantity = CurrentGold;

        UpdateFunds(-quantity, quantity);
        ActiveMember.Vitals.Express(GameConstants.EXPRESSION_UNSURE, GameConstants.EXPRESSION_UNSURE_DURATION);
    }

    public void Withdraw(int quantity)
    {
        if (CurrentBalance == 0)
            return;

        if (quantity > CurrentBalance)
            quantity = CurrentBalance;

        UpdateFunds(quantity, -quantity);
        ActiveMember.Vitals.Express(GameConstants.EXPRESSION_HAPPY, GameConstants.EXPRESSION_HAPPY_DURATION);
    }

    void EnemyDeath(Enemy enemy)
    {
        foreach(var member in Members)
        {
            if (member.IsConcious())
                member.Profile.EarnXP(enemy.Data.Experience);
        }

        if (!_monthlyKills.Contains(enemy.Data.ID))
            _monthlyKills.Add(enemy.Data.ID);
    }

    void PickupEnemy(Enemy enemy)
    {
        CollectGold(enemy.Data.Gold.Roll(), true);
    }

    void InspectChest(Chest chest)
    {
        bool canOpen = true;
        if (chest.Data.LockLevel > 0)
        {
            canOpen = TryDisarm(chest.Trap);
            if (canOpen)
            {
                ActiveMember.Vitals.Express(GameConstants.EXPRESSION_HAPPY, GameConstants.EXPRESSION_HAPPY_DURATION);
            }
        }

        if (canOpen)
        {
            PartyController.Instance.SetControlState(ControlState.MenuLock);
            SoundManager.Instance.PlayUISound("Chest");
            HUD.Instance.OpenChest(chest);
        }
    }

    public bool HasKilledEnemyThisMonth(string enemyID)
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

        UpdateFunds(amount - paid, 0);

        if(paid == 0)
            HUD.Instance.SendInfoMessage("You found " + amount + " gold!", 2.0f);
        else
            HUD.Instance.SendInfoMessage("You found " + amount + " gold (followers take " + paid + ")!", 2.0f);

        SoundManager.Instance.PlayUISound("Coins");
    }

    public void FillFoodTo(int quantity)
    {
        if (quantity > _currentFood)
        {
            UpdateFood(quantity - _currentFood);
        }
    }

    public bool TryHire(NPC npc)
    {
        if(Hires.Count == 2)
        {
            HUD.Instance.SendInfoMessage("I cannot join you, your party is full", 2.0f);
            return false;
        }

        bool success = TryPay(npc.Cost);
        if (success)
        {
            Hires.Add(npc);
            OnHiresChanged?.Invoke(Hires);
        }
        return success;
    }

    public bool DismissHire(NPC npc)
    {
        if (!Hires.Contains(npc))
            return false;

        Hires.Remove(npc);
        OnHiresChanged?.Invoke(Hires);
        return true;
    }

    public bool TryEat(int cost)
    {
        if (CurrentFood < cost)
        {
            HUD.Instance.SendInfoMessage("You don't have enough food to rest", 2.0f);
            return false;
        }
        UpdateFood(-cost);
        return true;
    }

    public bool TryPay(int cost)
    {
        if (CurrentGold < cost) {
            HUD.Instance.SendInfoMessage("You don't have enough gold...", 2.0f);
            return false;
        }

        UpdateFunds(-cost, 0);
        return true;
    }

    public bool TryIdentify(InventoryItem item)
    {
        if (item.IsIdentified)
            return false;

        Armor armor = item.Data as Armor;

        int level = ActiveMember.Skillset.GetSkillLevel("Identify");
        foreach (var hire in Hires)
        {
            if (hire.Profession == Profession.Scholar)
                level = 999;
        }

        bool success = item.TryIdentify(level);
        if (success)
        {
            ActiveMember.Vitals.Express(GameConstants.EXPRESSION_HAPPY, GameConstants.EXPRESSION_HAPPY_DURATION);
        }
        return success;
    }

    public bool TryRepair(InventoryItem item)
    {
        if (!item.IsBroken)
            return false;

        Armor armor = item.Data as Armor;

        int level = ActiveMember.Skillset.GetSkillLevel("Repair");
        foreach (var hire in Hires)
        {
            if (hire.Profession == Profession.Smith && item.Data is Weapon)
                level = 999;
            else if (hire.Profession == Profession.Armorer)
            {
                if(armor != null && armor.IsArmorItem())
                    level = 999;
            }
            else if (hire.Profession == Profession.Alchemist)
            {
                if (armor != null && armor.IsMagicItem())
                    level = 999;
            }
        }

        bool success = item.TryRepair(level);
        if(success)
        {
            ActiveMember.Vitals.Express(GameConstants.EXPRESSION_HAPPY, GameConstants.EXPRESSION_HAPPY_DURATION);
        }

        return success;
    }

    public bool TryDisarm(Trap trap)
    {
        if (trap.IsDisarmed)
            return true;

        int level = ActiveMember.Skillset.GetSkillLevel("Disarm");
        foreach(var hire in Hires)
        {
            if (hire.Profession == Profession.Tinker)
                level += 4;
            else if (hire.Profession == Profession.Locksmith)
                level += 6;
            else if (hire.Profession == Profession.Burglar)
                level += 8;
        }

        AttackResult result;
        bool success = trap.Disarm(level, out result);

        if(!success)
        {
            foreach(var member in Members)
            {
                member.OnDamaged(result);
            }
        } 
        else
        {
            ActiveMember.Vitals.Express(GameConstants.EXPRESSION_HAPPY, GameConstants.EXPRESSION_HAPPY_DURATION);
        }

        return success;
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
            if (member.Vitals.Condition == PartyMemberState.Concious)
                return false;
        }

        return true;
    }

    public bool EnemyAttack(EnemyData data)
    {
        List<PartyMember> validTargets = Members.FindAll(x => x.Vitals.Condition == PartyMemberState.Concious);
        PartyMember member = validTargets[Random.Range(0, validTargets.Count)];
        bool success = member.OnEnemyAttack(data);

        if (success)
        {
            if (!member.IsConcious())
                PartyController.Instance.Knockout(member);

            if (IsDead())
                GameController.Instance.TriggerDeath();

        }
        return success;
    }
}