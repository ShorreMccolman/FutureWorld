using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;

public class VitalStats : EffectiveStats
{
    public int TotalHP => _dict[CharacterStat.TotalHP].BaseValue;
    public int TotalSP => _dict[CharacterStat.TotalSP].BaseValue;
    public int ArmorClass => Mathf.Max(0,_dict[CharacterStat.ArmorClass].BaseValue);
    public int Attack => _dict[CharacterStat.Attack].BaseValue;
    public int DamageUpper => _dict[CharacterStat.DamageUpper].BaseValue;
    public int DamageLower => _dict[CharacterStat.DamageLower].BaseValue;
    public int Recovery => _dict[CharacterStat.Recovery].BaseValue;
    public int RangedAttack => _dict[CharacterStat.RangedAttack].BaseValue;
    public int RangedDamageUpper => _dict[CharacterStat.RangedDamageUpper].BaseValue;
    public int RangedDamageLower => _dict[CharacterStat.RangedDamageLower].BaseValue;
    public int RangedRecovery => _dict[CharacterStat.RangedRecovery].BaseValue;

    public int EffectiveTotalHP => Evaluate(CharacterStat.TotalHP);
    public int EffectiveTotalSP => Evaluate(CharacterStat.TotalSP);
    public int EffectiveArmorClass => Mathf.Max(0,Evaluate(CharacterStat.ArmorClass));
    public int EffectiveAttack => Evaluate(CharacterStat.Attack);
    public int EffectiveDamageUpper => Evaluate(CharacterStat.DamageUpper);
    public int EffectiveDamageLower => Evaluate(CharacterStat.DamageLower);
    public int EffectiveRecovery => Evaluate(CharacterStat.Recovery);
    public int EffectiveRangedAttack => Evaluate(CharacterStat.RangedAttack);
    public int EffectiveRangedDamageUpper => Evaluate(CharacterStat.RangedDamageUpper);
    public int EffectiveRangedDamageLower => Evaluate(CharacterStat.RangedDamageLower);
    public int EffectiveRangedRecovery => Evaluate(CharacterStat.RangedRecovery);

    public VitalStats(int totalHP, int totalSP, int ac, int attack, int upper, int lower, int recovery,
                                                        int rattack, int rupper, int rlower, int rrecovery)
    {
        _dict = new Dictionary<CharacterStat, EffectiveStat>()
        {
            {CharacterStat.TotalHP, new EffectiveStat(CharacterStat.TotalHP, totalHP) },
            {CharacterStat.TotalSP, new EffectiveStat(CharacterStat.TotalSP, totalSP) },
            {CharacterStat.ArmorClass, new EffectiveStat(CharacterStat.ArmorClass, ac) },
            {CharacterStat.Attack, new EffectiveStat(CharacterStat.Attack, attack) },
            {CharacterStat.DamageUpper, new EffectiveStat(CharacterStat.DamageUpper, upper) },
            {CharacterStat.DamageLower, new EffectiveStat(CharacterStat.DamageLower, lower) },
            {CharacterStat.Recovery, new EffectiveStat(CharacterStat.Recovery, recovery) },
            {CharacterStat.RangedAttack, new EffectiveStat(CharacterStat.RangedAttack, rattack) },
            {CharacterStat.RangedDamageUpper, new EffectiveStat(CharacterStat.RangedDamageUpper, rupper) },
            {CharacterStat.RangedDamageLower, new EffectiveStat(CharacterStat.RangedDamageLower, rlower) },
            {CharacterStat.RangedRecovery, new EffectiveStat(CharacterStat.RangedRecovery, rrecovery) }
        };
    }
}

public class Vitals : GameStateEntity
{
    public string EffectiveExpression { get; private set; }

    VitalStats _stats;
    public VitalStats Stats => _stats;

    public PartyMemberState Condition { get; private set; }
    public int CurrentHP { get; private set; }
    public int CurrentSP { get; private set; }
    public float Cooldown { get; private set; }

    string _expression;
    float _timeToNeutral;
    double _timeToIdle;
    bool _isIdleing;

    Profile _profile;
    Equipment _equipment;
    Skillset _skillset;
    Status _status;

    public event System.Action<string> OnExpressionChange;
    public event System.Action<bool, PartyMemberState> OnConditionChange;
    public event System.Action<int, int> OnHealthChange;
    public event System.Action<int, int> OnManaChange;
    public event System.Action OnVitalsChanged;

    public Vitals(GameStateEntity parent, CharacterData data, Status status, Profile profile, Equipment equipment, Skillset skillset) : base(parent)
    {
        _profile = profile;
        _equipment = equipment;
        _skillset = skillset;
        _status = status;

        UpdateEffectiveVitals();

        _status.OnStatusChanged += UpdateEffectiveExpression;
        _profile.OnStatsChanged += UpdateEffectiveVitals;
        _skillset.OnSkillsChanged += UpdateEffectiveVitals;

        Condition = PartyMemberState.Concious;
        CurrentHP = Stats.EffectiveTotalHP;
        CurrentSP = Stats.EffectiveTotalSP;
        ApplyCooldown(Stats.EffectiveRecovery);

        _expression = GameConstants.EXPRESSION_NEUTRAL;
        _timeToNeutral = 0;
        _timeToIdle = Random.Range(5.0f, 15.0f);
    }

    public Vitals(GameStateEntity parent, XmlNode node, Status status, Profile profile, Equipment equipment, Skillset skillset) : base(parent, node)
    {
        _profile = profile;
        _equipment = equipment;
        _skillset = skillset;
        _status = status;

        _status.OnStatusChanged += UpdateEffectiveExpression;
        _profile.OnStatsChanged += UpdateEffectiveVitals;
        _skillset.OnSkillsChanged += UpdateEffectiveVitals;

        XmlNode vitalsNode = node.SelectSingleNode("Vitals");
        Condition = (PartyMemberState)int.Parse(vitalsNode.SelectSingleNode("Condition").InnerText);
        CurrentHP = int.Parse(vitalsNode.SelectSingleNode("CurrentHP").InnerText);
        CurrentSP = int.Parse(vitalsNode.SelectSingleNode("CurrentMP").InnerText);
        _timeToNeutral = float.Parse(vitalsNode.SelectSingleNode("TTN").InnerText);
        Cooldown = float.Parse(vitalsNode.SelectSingleNode("Cooldown").InnerText);
        _expression = vitalsNode.SelectSingleNode("Expression").InnerText;

        UpdateEffectiveVitals();
    }

    public override XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("Vitals");

        element.AppendChild(XmlHelper.Attribute(doc, "Condition", (int)Condition));
        element.AppendChild(XmlHelper.Attribute(doc, "CurrentHP", CurrentHP));
        element.AppendChild(XmlHelper.Attribute(doc, "CurrentMP", CurrentSP));
        element.AppendChild(XmlHelper.Attribute(doc, "TTN", _timeToNeutral));
        element.AppendChild(XmlHelper.Attribute(doc, "Cooldown", Cooldown));
        element.AppendChild(XmlHelper.Attribute(doc, "Expression", _expression));
        element.AppendChild(base.ToXml(doc));
        return element;
    }

    public void UpdateEffectiveVitals()
    {
        int hp = _profile.MaxHitPoints;
        int sp = _profile.MaxSpellPoints;
        int ac = _profile.BaseArmorClass + _equipment.GetArmorClass(_skillset);
        int attack = _profile.BaseAttack + _equipment.GetAttack(_skillset);
        int upper = Mathf.Max(_profile.BaseDamage + _equipment.GetDamageUpper(_skillset), 1);
        int lower = Mathf.Max(_profile.BaseDamage + _equipment.GetDamageLower(_skillset), 1);
        int rec = Mathf.Max(30, _equipment.GetRecovery(_skillset) - _profile.BaseArmorClass);
        int rattack = _profile.BaseRangedAttack + _equipment.GetRangedAttack(_skillset);
        int rupper = Mathf.Max(_profile.BaseRangedDamage + _equipment.GetRangedDamageUpper(_skillset), 0);
        int rlower = Mathf.Max(_profile.BaseRangedDamage + _equipment.GetRangedDamageLower(_skillset), 0);
        int rrec = Mathf.Max(1, _equipment.GetRangedRecovery(_skillset) - _profile.BaseArmorClass);

        _stats = new VitalStats(hp, sp, ac, attack, upper, lower, rec, rattack, rupper, rlower, rrec);
        _equipment.ModifyStats(_stats);
        _skillset.ModifyStats(_stats);

        OnVitalsChanged?.Invoke();
    }

    public void UpdateEffectiveExpression()
    {
        string old = EffectiveExpression;

        if (Condition == PartyMemberState.Unconcious)
            EffectiveExpression = GameConstants.EXPRESSION_UNCONCIOUS;
        else if (Condition == PartyMemberState.Dead)
            EffectiveExpression = GameConstants.EXPRESSION_DEAD;
        else
        {
            string expression = _expression;
            _status.OverrideExpression(ref expression);
            EffectiveExpression = expression;
        }
        if(old != EffectiveExpression)
            OnExpressionChange?.Invoke(EffectiveExpression);
    }

    public bool IsReady()
    {
        return Cooldown <= 0;
    }

    public void ForceFullHeal()
    {
        UpdateHP(Stats.EffectiveTotalHP);
        UpdateSP(Stats.EffectiveTotalSP);
    }

    public void Restore(bool restoreAll = false)
    {
        UpdateHP(_status.ModifyRestHP(Stats.EffectiveTotalHP));
        UpdateSP(_status.ModifyRestSP(Stats.EffectiveTotalSP));

        if (restoreAll && (Condition == PartyMemberState.Dead || Condition == PartyMemberState.Eradicated))
            ChangeCondition(PartyMemberState.Concious);
    }

    public void Express(string expression, float duration)
    {
        _expression = expression;
        _timeToNeutral = duration;
        _isIdleing = false;
        UpdateEffectiveExpression();
    }

    public bool TickExpression(float delta)
    {
        _timeToNeutral -= delta;

        if (_timeToNeutral <= 0 && !_isIdleing)
        {
            _expression = GameConstants.EXPRESSION_NEUTRAL;
            UpdateEffectiveExpression();
            _timeToIdle = Random.Range(5.0f, 15.0f);
            _isIdleing = true;
            return true;
        }

        if (_timeToNeutral < -_timeToIdle)
        {
            Express(GameConstants.EXPRESSION_IDLE, 0.5f);
            return true;
        }

        return false;
    }

    public void UpdateHP(int hp)
    {
        if (hp == CurrentHP)
            return;

        if(hp > 0 && Condition == PartyMemberState.Unconcious)
        {
            ChangeCondition(PartyMemberState.Concious);
        }
        else if (hp <= 0 && Condition == PartyMemberState.Concious) 
        {
            if (hp > -Stats.EffectiveTotalHP)
                ChangeCondition(PartyMemberState.Unconcious);
            else
                ChangeCondition(PartyMemberState.Dead);
        }

        CurrentHP = Mathf.Min(hp, Stats.EffectiveTotalHP);
        OnHealthChange?.Invoke(CurrentHP, Stats.EffectiveTotalHP);
    }

    public void UpdateSP(int sp)
    {
        if (sp == CurrentSP)
            return;

        CurrentSP = Mathf.Min(sp, Stats.EffectiveTotalSP);
        OnManaChange?.Invoke(CurrentSP, Stats.EffectiveTotalSP);
    }

    public void ApplyCooldown(float cooldown)
    {
        Cooldown = cooldown;
    }

    public bool TickCooldown(float delta)
    {
        bool wasOn = Cooldown > 0;
        if (!wasOn)
            return false;

        Cooldown -= delta;
        if (Cooldown < 0)
            Cooldown = 0;

        bool isReady = Cooldown <= 0;
        if(isReady)
        {
            OnConditionChange?.Invoke(true, Condition);
        }

        return isReady;
    }

    public void GainHealthPoints(int amount)
    {
        UpdateHP(CurrentHP + amount);
    }

    public void GainSpellPoints(int amount)
    {
        UpdateSP(CurrentSP + amount);
    }

    public void TakeDamage(int damage)
    {
        UpdateHP(CurrentHP - damage);
    }

    void ChangeCondition(PartyMemberState state)
    {
        if (Condition == state)
            return;

        Condition = state;
        OnConditionChange?.Invoke(IsReady(), state);
        UpdateEffectiveExpression();
    }
}
