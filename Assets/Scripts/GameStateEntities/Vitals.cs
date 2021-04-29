using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;

public class Vitals : GameStateEntity
{
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
            return Mathf.Max(value,1);
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
            if (!_equipment.HasRangedWeapon())
                return 0;

            return _profile.BaseRangedAttack(_equipment.AccuracyBonus()) + _equipment.GetRangedAttack(_skillset);
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

    public float TimeToNeutral { get; private set; }
    public float Cooldown { get; private set; }
    public string Expression { get; private set; }

    double _timeToIdle;
    bool _isIdleing;

    Profile _profile;
    Equipment _equipment;
    Skillset _skillset;
    Status _status;

    public Vitals(GameStateEntity parent, CharacterData data, Status status, Profile profile, Equipment equipment, Skillset skillset) : base(parent)
    {
        _profile = profile;
        _equipment = equipment;
        _skillset = skillset;
        _status = status;

        Condition = PartyMemberState.Good;
        CurrentHP = EffectiveTotalHP;
        CurrentMP = EffectiveTotalMP;
        ApplyCooldown(Recovery);

        Expression = GameConstants.EXPRESSION_NEUTRAL;
        TimeToNeutral = 0;
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
        TimeToNeutral = int.Parse(vitalsNode.SelectSingleNode("TTN").InnerText);
        Cooldown = int.Parse(vitalsNode.SelectSingleNode("Cooldown").InnerText);
        Expression = vitalsNode.SelectSingleNode("Expression").InnerText;
    }

    public override XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("Vitals");

        element.AppendChild(XmlHelper.Attribute(doc, "Condition", (int)Condition));
        element.AppendChild(XmlHelper.Attribute(doc, "CurrentHP", CurrentHP));
        element.AppendChild(XmlHelper.Attribute(doc, "CurrentMP", CurrentMP));
        element.AppendChild(XmlHelper.Attribute(doc, "TTN", TimeToNeutral));
        element.AppendChild(XmlHelper.Attribute(doc, "Cooldown", Cooldown));
        element.AppendChild(XmlHelper.Attribute(doc, "Expression", Expression));
        element.AppendChild(base.ToXml(doc));
        return element;
    }

    public float GetHealthRatio()
    {
        return (float)CurrentHP / (float)EffectiveTotalHP;
    }

    public float GetManaRatio()
    {
        return (float)CurrentMP / (float)EffectiveTotalMP;
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

    public void Rest()
    {
        CurrentHP = EffectiveTotalHP;
        CurrentMP = EffectiveTotalMP;
    }

    public void SetExpression(string expression, float duration)
    {
        TimeToNeutral = duration;
        Expression = expression;
        _isIdleing = false;
    }

    public void ApplyCooldown(float cooldown)
    {
        Cooldown = cooldown;
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

    public bool TickExpression(float delta)
    {
        TimeToNeutral -= delta;

        if (TimeToNeutral <= 0 && !_isIdleing)
        {
            Expression = GameConstants.EXPRESSION_NEUTRAL;
            _timeToIdle = Random.Range(5.0f, 15.0f);
            _isIdleing = true;
            return true;
        }

        if (TimeToNeutral < -_timeToIdle)
        {
            SetExpression(GameConstants.EXPRESSION_IDLE, 0.5f);
            return true;
        }

        return false;
    }

    public void GainHealthPoints(int amount)
    {
        CurrentHP += amount;
        if (CurrentHP > EffectiveTotalHP)
            CurrentHP = EffectiveTotalHP;
    }

    public void GainSpellPoints(int amount)
    {
        CurrentMP += amount;
        if (CurrentMP > EffectiveTotalMP)
            CurrentMP = EffectiveTotalMP;
    }

    public void TakeDamage(int damage)
    {
        CurrentHP -= damage;

        if (CurrentHP <= -EffectiveTotalHP)
            Condition = PartyMemberState.Dead;
        else if (CurrentHP <= 0)
            Condition = PartyMemberState.Unconcious;
    }
}
