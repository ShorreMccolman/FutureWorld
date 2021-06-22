using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;

public class Vitals : GameStateEntity
{
    public string EffectiveExpression
    {
        get
        {
            if (Condition == PartyMemberState.Unconcious)
                return GameConstants.EXPRESSION_UNCONCIOUS;
            else if (Condition == PartyMemberState.Dead)
                return GameConstants.EXPRESSION_DEAD;
            else
            {
                string expression;
                if (_status.OverrideExpression(out expression))
                    return expression;
            }
            return Expression;
        }
    }

    public int EffectiveTotalHP
    {
        get
        {
            return _profile.HitPoints() + _equipment.GetBonusHP() + _skillset.GetBonusHP(_profile.Class);
        }
    }
    public int EffectiveTotalMP
    {
        get
        {
            return _profile.SpellPoints() + _equipment.GetBonusMP() + _skillset.GetBonusMP(_profile.Class);
        }
    }
    public int EffectiveArmorClass
    {
        get
        {
            int ac = _profile.BaseArmorClass() + _equipment.GetArmorClass(_skillset);
            return Mathf.Max(ac, 0);
        }
    }
    public int EffectiveAttack
    {
        get
        {
            return _profile.BaseAttack() + _equipment.GetAttack(_skillset);
        }
    }
    public int EffectiveDamageLower
    {
        get
        {
            int value = _profile.BaseDamage() + _equipment.GetDamageLower(_skillset);
            return Mathf.Max(value, 1);
        }
    }
    public int EffectiveDamageUpper
    {
        get
        {
            int value = _profile.BaseDamage() + _equipment.GetDamageUpper(_skillset);
            return Mathf.Max(value, 1);
        }
    }
    public int EffectiveRangedAttack
    {
        get
        {
            return _profile.BaseRangedAttack() + _equipment.GetRangedAttack(_skillset);
        }
    }
    public int EffectiveRangedDamageLower
    {
        get
        {
            int value = _profile.BaseRangedDamage() + _equipment.GetRangedDamageLower(_skillset);
            return Mathf.Max(value, 0);
        }
    }
    public int EffectiveRangedDamageUpper
    {
        get
        {
            int value = _profile.BaseRangedDamage() + _equipment.GetRangedDamageUpper(_skillset);
            return Mathf.Max(value, 0);
        }
    }

    public int Recovery
    {
        get
        {
            int value = _equipment.GetRecovery(_skillset);
            value -= _profile.BaseArmorClass();
            return Mathf.Max(30, value);
        }
    }

    public int RangedRecovery
    {
        get
        {
            int value = _equipment.GetRangedRecovery(_skillset);
            value -= _profile.BaseArmorClass();
            return Mathf.Max(1, value);
        }
    }


    public PartyMemberState Condition { get; private set; }
    public int CurrentHP { get; private set; }
    public int CurrentMP { get; private set; }
    public float Cooldown { get; private set; }
    public string Expression { get; private set; }

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

    public Vitals(GameStateEntity parent, CharacterData data, Status status, Profile profile, Equipment equipment, Skillset skillset) : base(parent)
    {
        _profile = profile;
        _equipment = equipment;
        _skillset = skillset;
        _status = status;

        _status.OnStatusChange += ChangeStatus;

        Condition = PartyMemberState.Concious;
        CurrentHP = EffectiveTotalHP;
        CurrentMP = EffectiveTotalMP;
        ApplyCooldown(Recovery);

        Expression = GameConstants.EXPRESSION_NEUTRAL;
        _timeToNeutral = 0;
        _timeToIdle = Random.Range(5.0f, 15.0f);
    }

    public Vitals(GameStateEntity parent, XmlNode node, Status status, Profile profile, Equipment equipment, Skillset skillset) : base(parent, node)
    {
        _profile = profile;
        _equipment = equipment;
        _skillset = skillset;
        _status = status;

        XmlNode vitalsNode = node.SelectSingleNode("Vitals");
        Condition = (PartyMemberState)int.Parse(vitalsNode.SelectSingleNode("Condition").InnerText);
        CurrentHP = int.Parse(vitalsNode.SelectSingleNode("CurrentHP").InnerText);
        CurrentMP = int.Parse(vitalsNode.SelectSingleNode("CurrentMP").InnerText);
        _timeToNeutral = float.Parse(vitalsNode.SelectSingleNode("TTN").InnerText);
        Cooldown = float.Parse(vitalsNode.SelectSingleNode("Cooldown").InnerText);
        Expression = vitalsNode.SelectSingleNode("Expression").InnerText;
    }

    public override XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("Vitals");

        element.AppendChild(XmlHelper.Attribute(doc, "Condition", (int)Condition));
        element.AppendChild(XmlHelper.Attribute(doc, "CurrentHP", CurrentHP));
        element.AppendChild(XmlHelper.Attribute(doc, "CurrentMP", CurrentMP));
        element.AppendChild(XmlHelper.Attribute(doc, "TTN", _timeToNeutral));
        element.AppendChild(XmlHelper.Attribute(doc, "Cooldown", Cooldown));
        element.AppendChild(XmlHelper.Attribute(doc, "Expression", Expression));
        element.AppendChild(base.ToXml(doc));
        return element;
    }

    public int MaxRestHP()
    {
        return _status.ModifyRestHP(EffectiveTotalHP);
    }

    public int MaxRestMP()
    {
        return _status.ModifyRestMP(EffectiveTotalHP);
    }

    public bool IsReady()
    {
        return Cooldown <= 0;
    }

    public void ForceFullHeal()
    {
        CurrentHP = EffectiveTotalHP;
        CurrentMP = EffectiveTotalMP;

        OnHealthChange?.Invoke(CurrentHP, EffectiveTotalHP);
        OnManaChange?.Invoke(CurrentMP, EffectiveTotalMP);
    }

    public void Restore(bool restoreAll = false)
    {
        if(restoreAll)
        {
            CurrentHP = EffectiveTotalHP;
            CurrentMP = EffectiveTotalMP;
        } 
        else
        {
            CurrentHP = MaxRestHP();
            CurrentMP = MaxRestMP();
        }
        OnHealthChange?.Invoke(CurrentHP, EffectiveTotalHP);
        OnManaChange?.Invoke(CurrentMP, EffectiveTotalMP);

        if (Condition == PartyMemberState.Unconcious)
            ChangeCondition(PartyMemberState.Concious);
        else if (restoreAll && (Condition == PartyMemberState.Dead || Condition == PartyMemberState.Eradicated))
            ChangeCondition(PartyMemberState.Concious);
    }

    public void Express(string expression, float duration)
    {
        _timeToNeutral = duration;
        Expression = expression;
        _isIdleing = false;
        OnExpressionChange?.Invoke(EffectiveExpression);
    }

    void ChangeStatus(List<StatusCondition> conditions)
    {
        OnExpressionChange?.Invoke(EffectiveExpression);
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

    public bool TickExpression(float delta)
    {
        _timeToNeutral -= delta;

        if (_timeToNeutral <= 0 && !_isIdleing)
        {
            Expression = GameConstants.EXPRESSION_NEUTRAL;
            _timeToIdle = Random.Range(5.0f, 15.0f);
            _isIdleing = true;
            OnExpressionChange?.Invoke(EffectiveExpression);
            return true;
        }

        if (_timeToNeutral < -_timeToIdle)
        {
            Express(GameConstants.EXPRESSION_IDLE, 0.5f);
            return true;
        }

        return false;
    }

    public void GainHealthPoints(int amount)
    {
        CurrentHP += amount;
        if (CurrentHP > EffectiveTotalHP)
            CurrentHP = EffectiveTotalHP;
        OnHealthChange?.Invoke(CurrentHP, EffectiveTotalHP);

        if (Condition == PartyMemberState.Unconcious && CurrentHP > 0)
            ChangeCondition(PartyMemberState.Concious);
    }

    public void GainSpellPoints(int amount)
    {
        CurrentMP += amount;
        if (CurrentMP > EffectiveTotalMP)
            CurrentMP = EffectiveTotalMP;
        OnManaChange?.Invoke(CurrentMP, EffectiveTotalMP);
    }

    public void TakeDamage(int damage)
    {
        CurrentHP -= damage;
        OnHealthChange?.Invoke(CurrentHP, EffectiveTotalHP);

        if (CurrentHP <= -EffectiveTotalHP)
            ChangeCondition(PartyMemberState.Dead);
        else if (CurrentHP <= 0)
            ChangeCondition(PartyMemberState.Unconcious);
    }

    void ChangeCondition(PartyMemberState state)
    {
        Condition = state;
        OnConditionChange?.Invoke(IsReady(), state);
        OnExpressionChange?.Invoke(EffectiveExpression);
    }
}
