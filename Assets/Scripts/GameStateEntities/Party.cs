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
    public QuestLog QuestLog { get; private set; }

    float _currentTime;
    public float CurrentTime => _currentTime;

    int _currentReputation;
    public int CurrentReputation => _currentReputation;

    int _currentFood;
    public int CurrentFood => _currentFood;

    int _currentGold;
    public int CurrentGold => _currentGold;

    int _currentBalance;
    public int CurrentBalance => _currentBalance;

    List<PartyMember> _members;
    public List<PartyMember> Members => _members;

    List<NPC> _hires;
    List<string> _visitedLocations;
    List<string> _monthlyKills;
    CombatPriority _priorityQueue;

    bool _memberLock;

    float _wizDuration;
    float _torchDuration;
    float _wWalkDuration;
    float _flyDuration;

    public static event System.Action<PartyMember> OnMemberChanged;
    public static event System.Action<int, int> OnFundsChanged;
    public static event System.Action<int> OnFoodChanged;
    public static event System.Action<List<NPC>> OnHiresChanged;
    public static event System.Action<bool, SkillProficiency> OnWizardEyeChanged;

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

        _currentReputation = 0;
        _currentFood = 7;
        _currentGold = 200;
        _currentBalance = 0;
        _currentTime = 0;
        _visitedLocations = new List<string>();
        _monthlyKills = new List<string>();
    }

    public Party(XmlNode node) : base(null, node)
    {
        Init();

        CurrentLocationID = node.SelectSingleNode("LocationID").InnerText;
        _currentReputation = int.Parse(node.SelectSingleNode("Reputation").InnerText);
        _currentFood = int.Parse(node.SelectSingleNode("Food").InnerText);
        _currentGold = int.Parse(node.SelectSingleNode("Gold").InnerText);
        _currentBalance = int.Parse(node.SelectSingleNode("Balance").InnerText);
        _currentTime = float.Parse(node.SelectSingleNode("Time").InnerText);
        QuestLog = new QuestLog(this, node.SelectSingleNode("QuestLog"));
        Populate<PartyMember>(ref _members, typeof(Party), node, "Members", "PartyMember");
        Populate<NPC>(ref _hires, typeof(Party), node, "Hires", "NPC");
    }

    void Init()
    {
        Instance = this;

        _priorityQueue = new CombatPriority();

        EnemyEntity.OnEnemyDeath += EnemyDeath;
        EnemyEntity.OnEnemyPickup += PickupEnemy;
        Vitals.OnMemberKnockout += MemberKnockout;
        Vitals.OnMemberReady += MemberReady;
        TimeManagement.OnTick += Tick;
        TurnController.OnTurnBasedToggled += TBToggled;
    }

    public override XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("Party");
        element.AppendChild(XmlHelper.Attribute(doc, "LocationID", CurrentLocationID));
        element.AppendChild(XmlHelper.Attribute(doc, "Reputation", _currentReputation));
        element.AppendChild(XmlHelper.Attribute(doc, "Food", _currentFood));
        element.AppendChild(XmlHelper.Attribute(doc, "Gold", _currentGold));
        element.AppendChild(XmlHelper.Attribute(doc, "Balance", _currentBalance));
        element.AppendChild(XmlHelper.Attribute(doc, "Time", _currentTime));
        element.AppendChild(XmlHelper.Attribute(doc, "Members", _members));
        if(_hires.Count > 0)
            element.AppendChild(XmlHelper.Attribute(doc, "Hires", _hires));
        element.AppendChild(QuestLog.ToXml(doc));
        element.AppendChild(base.ToXml(doc));

        return element;
    }

    public Vector3 DropPosition
    {
        get
        {
            return Entity.transform.position + Entity.transform.forward * 3f;
        }
    }

    public void Tick(float tick)
    {
        System.DateTime dt = TimeManagement.Instance.GetDT(_currentTime);
        _currentTime += tick;
        System.DateTime newDT = TimeManagement.Instance.GetDT(_currentTime);

        if(dt.Month < newDT.Month)
        {
            _monthlyKills.Clear();
        }
    }

    void TBToggled(bool enabled)
    {
        _priorityQueue.Clear();
    }

    void MemberReady(PartyMember member)
    {
        if (member.IsConcious())
        {
            if (!TurnController.Instance.IsTurnBasedEnabled)
            {
                if (ActiveMember == null)
                    SetActiveMember(member);
                else
                    _priorityQueue.Add(member);
            }
        }
    }

    public void MemberKnockout(PartyMember member)
    {
        bool alive = false;
        foreach (var m in Members)
            if (m.IsConcious())
                alive = true;

        if(alive)
            MemberUnready(member);
        else
            GameController.Instance.TriggerDeath();
    }

    public void MemberUnready(PartyMember member)
    {
        _priorityQueue.Flush(member);
        if (ActiveMember == member)
        {
            PartyMember next = _priorityQueue.GetIfReady() as PartyMember;
            SetActiveMember(next);
        }
    }

    public PartyMember GetAttacker()
    {
        PartyMember attacker = null;
        if (ActiveMember != null && ActiveMember.Vitals.IsReady())
            attacker = ActiveMember;
        else if (!TurnController.Instance.IsTurnBasedEnabled)
        {
            attacker = _priorityQueue.GetIfReady() as PartyMember;
            SetActiveMember(attacker);
        }

        return attacker;
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

    public void ToggleSelectedCharacter()
    {
        if (ActiveMember == null)
            return;

        int member = -1;
        for (int i = 0; i < 4; i++)
        {
            if (Members[i].Equals(ActiveMember))
            {
                member = i;
            }
        }

        int index = member;
        for (int i = 1; i < 4; i++)
        {
            int cur = (index + i) % 4;
            if (Members[cur].Vitals.IsReady() && Members[cur].IsConcious())
            {
                SetActiveMember(Members[cur]);
                return;
            }
        }
    }

    public void SetMemberLock(bool enable)
    {
        _memberLock = enable;
    }

    public void SetActiveMember(PartyMember member)
    {
        if (_memberLock)
            return;

        ActiveMember = member;
        OnMemberChanged?.Invoke(member);
    }

    public void VisitLocation(string locationID)
    {
        _visitedLocations.Add(locationID);
    }

    public void Deposit(int quantity)
    {
        if (quantity > _currentGold)
            quantity = _currentGold;

        UpdateFunds(-quantity, quantity);
        ActiveMember.Vitals.Express(GameConstants.EXPRESSION_UNSURE, GameConstants.EXPRESSION_UNSURE_DURATION);
    }

    public void Withdraw(int quantity)
    {
        if (CurrentBalance == 0)
            return;

        if (quantity > _currentBalance)
            quantity = _currentBalance;

        UpdateFunds(quantity, -quantity);
        ActiveMember.Vitals.Express(GameConstants.EXPRESSION_HAPPY, GameConstants.EXPRESSION_HAPPY_DURATION);
    }

    void EnemyDeath(Enemy enemy)
    {
        foreach(var member in _members)
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
            foreach (var hire in _hires)
            {
                paid += Mathf.CeilToInt(amount * hire.Rate * 0.01f);
            }
        }

        UpdateFunds(amount - paid, 0);

        if(paid == 0)
            InfoMessageReceiver.Send("You found " + amount + " gold!", 2.0f);
        else
            InfoMessageReceiver.Send("You found " + amount + " gold (followers take " + paid + ")!", 2.0f);

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
        if(_hires.Count == 2)
        {
            InfoMessageReceiver.Send("I cannot join you, your party is full", 2.0f);
            return false;
        }

        bool success = TryPay(npc.Cost);
        if (success)
        {
            _hires.Add(npc);
            OnHiresChanged?.Invoke(_hires);
        }
        return success;
    }

    public bool DismissHire(NPC npc)
    {
        if (!_hires.Contains(npc))
            return false;

        _hires.Remove(npc);
        OnHiresChanged?.Invoke(_hires);
        return true;
    }

    public bool TryEat(int cost)
    {
        if (CurrentFood < cost)
        {
            InfoMessageReceiver.Send("You don't have enough food to rest", 2.0f);
            return false;
        }
        UpdateFood(-cost);
        return true;
    }

    public bool TryPay(int cost)
    {
        if (CurrentGold < cost) {
            InfoMessageReceiver.Send("You don't have enough gold...", 2.0f);
            return false;
        }

        UpdateFunds(-cost, 0);
        return true;
    }

    public bool BarterSellItem(InventoryItem item, int price)
    {
        bool success = TryPay(-price);
        if (success)
        {
            success = ActiveMember.Inventory.RemoveItem(item);
        }

        return success;
    }

    public bool BarterIdentifyItem(InventoryItem item, int price)
    {
        bool success = TryPay(price);
        if (success)
        {
            success = item.TryIdentify(100000);
        }

        return success;
    }

    public bool BarterRepairItem(InventoryItem item, int price)
    {
        bool success = TryPay(price);
        if (success)
        {
            success = item.TryRepair(100000);
        }

        return success;
    }

    public bool TryIdentify(InventoryItem item)
    {
        if (item.IsIdentified)
            return false;

        Armor armor = item.Data as Armor;

        int level = ActiveMember.Skillset.GetSkillLevel("Identify");
        foreach (var hire in _hires)
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
        foreach (var hire in _hires)
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
        foreach(var hire in _hires)
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

    public bool PickupDrop(ItemDrop drop)
    {
        if (drop.Item.Data is Gold)
        {
            CollectGold(drop.Item);
            return true;
        }

        if (ActiveMember != null)
        {
            InventoryItem item = ActiveMember.Inventory.AddItem(drop.Item);
            if (item != null)
            {
                InfoMessageReceiver.Send("You found an item (" + item.Data.GetTypeDescription() + ")!", 2.0f);
                ActiveMember.Vitals.Express(GameConstants.EXPRESSION_HAPPY, GameConstants.EXPRESSION_HAPPY_DURATION);
                return true;
            }
        }

        bool success = HUD.Instance.TryHold(drop.Item);
        if(success)
            InfoMessageReceiver.Send("You found an item (" + drop.Item.Data.GetTypeDescription() + ")!", 2.0f);
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

    public Dictionary<string, float> GetPartySpells()
    {
        Dictionary<string, float> spells = new Dictionary<string, float>();
        if(_wizDuration > 0)
        {
            spells.Add("Wizard Eye", _wizDuration);
        }
        return spells;
    }

    public void WizardsEye(float duration, SkillProficiency proficiency)
    {
        _wizDuration = duration;
        TimeManagement.OnTick += TickEye;
        OnWizardEyeChanged?.Invoke(true, proficiency);
    }

    void TickEye(float tick)
    {
        _wizDuration -= tick;
        if(_wizDuration <= 0)
        {
            OnWizardEyeChanged?.Invoke(false, SkillProficiency.Novice);
            TimeManagement.OnTick -= TickEye;
        }
    }
}