using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class Status : GameStateEntity
{
    List<StatusCondition> _conditions;
    public List<StatusCondition> Conditions { get { return _conditions; } }

    public Status(GameStateEntity parent) : base(parent)
    {
        _conditions = new List<StatusCondition>();
        AddCondition(StatusEffectOption.Rested, GameConstants.REST_DURATION);
    }

    public Status(GameStateEntity parent, XmlNode node) : base(parent, node)
    {
        XmlNode statusNode = node.SelectSingleNode("Status");
        Populate<StatusCondition>(ref _conditions, typeof(Status), node, "Conditions", "StatusCondition");
    }

    public override XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("Status");
        element.AppendChild(XmlHelper.Attribute(doc, "Conditions", Conditions));
        element.AppendChild(base.ToXml(doc));
        return element;
    }

    public string GetStatusCondition()
    {
        if (HasCondition(StatusEffectOption.Disease))
        {
            return "Diseased";
        }
        else if (HasCondition(StatusEffectOption.Poison))
        {
            return "Poisoned";
        }
        else if (HasCondition(StatusEffectOption.Weak))
        {
            return "Weak";
        }
        return "Good";
    }

    public bool ExpressionOverride(out string expression)
    {
        if (HasCondition(StatusEffectOption.Sleep))
        {
            expression = GameConstants.EXPRESSION_SLEEP;
            return true;
        }
        else if (HasCondition(StatusEffectOption.Disease))
        {
            expression = GameConstants.EXPRESSION_DISEASE;
            return true;
        }
        else if (HasCondition(StatusEffectOption.Poison))
        {
            expression = GameConstants.EXPRESSION_POISON;
            return true;
        }
        else if(HasCondition(StatusEffectOption.Weak))
        {
            expression = GameConstants.EXPRESSION_WEAK;
            return true;
        }

        expression = "";
        return false;
    }

    public bool TickConditions(float delta)
    {
        List<StatusCondition> completed = new List<StatusCondition>();
        foreach(var condition in _conditions)
        {
            bool isComplete = condition.Tick(delta);
            if (isComplete)
                completed.Add(condition);
        }

        if (completed.Count == 0)
            return false;

        foreach (var condition in completed)
        {
            CompleteCondition(condition);
        }
        return true;
    }

    public bool HasCondition(StatusEffectOption option)
    {
        for (int i = 0; i < _conditions.Count; i++)
        {
            if (_conditions[i].Option == option)
            {
                return true;
            }
        }
        return false;
    }

    public bool HasNegativeCondition()
    {
        foreach(var condition in _conditions)
        {
            if (condition.Effect.NeedsHealing)
                return true;
        }
        return false;
    }

    public void HealConditions()
    {
        List<StatusEffectOption> options = new List<StatusEffectOption>();
        foreach(var condition in _conditions)
        {
            if(condition.Effect.NeedsHealing)
            {
                options.Add(condition.Option);
            }
        }
        foreach(var option in options)
            RemoveCondition(option);
        AddCondition(StatusEffectOption.Rested, GameConstants.REST_DURATION);
    }

    public void CompleteCondition(StatusCondition condition)
    {
        _conditions.Remove(condition);
        switch(condition.Effect.Option)
        {
            case StatusEffectOption.Sleep:
                RemoveCondition(StatusEffectOption.Weak);
                AddCondition(StatusEffectOption.Rested, GameConstants.REST_DURATION);
                return;
            case StatusEffectOption.Rested:
                RemoveCondition(StatusEffectOption.Rested);
                AddCondition(StatusEffectOption.Weak, 0);
                return;
        }
    }

    public void AddCondition(StatusEffectOption option, float duration)
    {
        for(int i=0;i<_conditions.Count;i++)
        {
            if (_conditions[i].Option == option)
            {
                _conditions[i] = StatusEffectDatabase.Instance.GetStatusCondition(option, this, duration);
                return;
            }
        }

        _conditions.Add(StatusEffectDatabase.Instance.GetStatusCondition(option, this, duration));
    }

    public void RemoveCondition(StatusEffectOption option)
    {
        int index = -1;
        for (int i = 0; i < _conditions.Count; i++)
        {
            if (_conditions[i].Option == option)
            {
                index = i;
                break;
            }
        }

        if(index >= 0)
            _conditions.RemoveAt(index);
    }

    public int ModifyMight(int stat)
    {
        foreach(var condition in Conditions)
        {
            switch(condition.Option)
            {
                case StatusEffectOption.Poison:
                    stat = Mathf.RoundToInt(stat * 0.75f);
                    break;
                case StatusEffectOption.Disease:
                    stat = Mathf.RoundToInt(stat * 0.60f);
                    break;
            }
        }

        return stat;
    }

    public int ModifyEndurance(int stat)
    {
        foreach (var condition in Conditions)
        {
            switch (condition.Option)
            {
                case StatusEffectOption.Poison:
                    stat = Mathf.RoundToInt(stat * 0.75f);
                    break;
                case StatusEffectOption.Disease:
                    stat = Mathf.RoundToInt(stat * 0.60f);
                    break;
            }
        }

        return stat;
    }

    public int ModifyIntellect(int stat)
    {
        foreach (var condition in Conditions)
        {
            switch (condition.Option)
            {
                default:
                    break;
            }
        }

        return stat;
    }

    public int ModifyPersonality(int stat)
    {
        foreach (var condition in Conditions)
        {
            switch (condition.Option)
            {
                default:
                    break;
            }
        }

        return stat;
    }

    public int ModifyAccuracy(int stat)
    {
        foreach (var condition in Conditions)
        {
            switch (condition.Option)
            {
                case StatusEffectOption.Poison:
                    stat = Mathf.RoundToInt(stat * 0.75f);
                    break;
                case StatusEffectOption.Disease:
                    stat = Mathf.RoundToInt(stat * 0.60f);
                    break;
            }
        }

        return stat;
    }

    public int ModifySpeed(int stat)
    {
        foreach (var condition in Conditions)
        {
            switch (condition.Option)
            {
                case StatusEffectOption.Poison:
                    stat = Mathf.RoundToInt(stat * 0.75f);
                    break;
                case StatusEffectOption.Disease:
                    stat = Mathf.RoundToInt(stat * 0.60f);
                    break;
            }
        }

        return stat;
    }

    public int ModifyLuck(int stat)
    {
        foreach (var condition in Conditions)
        {
            switch (condition.Option)
            {
                default:
                    break;
            }
        }

        return stat;
    }

    public int ModifyRestHP(int stat)
    {
        foreach (var condition in Conditions)
        {
            switch (condition.Option)
            {
                case StatusEffectOption.Poison:
                case StatusEffectOption.Disease:
                    stat = Mathf.RoundToInt(stat * 0.5f);
                    break;
            }
        }

        return stat;
    }

    public int ModifyResistance(AttackType type, int stat)
    {
        foreach (var condition in Conditions)
        {
            switch (condition.Option)
            {
                default:
                    break;
            }
        }

        return stat;
    }

    public int ModifyRestMP(int stat)
    {
        foreach (var condition in Conditions)
        {
            switch (condition.Option)
            {
                case StatusEffectOption.Poison:
                case StatusEffectOption.Disease:
                    stat = Mathf.RoundToInt(stat * 0.5f);
                    break;
            }
        }

        return stat;
    }

    public int ModifyFinalDamage(int stat)
    {
        foreach (var condition in Conditions)
        {
            switch (condition.Option)
            {
                case StatusEffectOption.Weak:
                    stat = Mathf.RoundToInt(stat * 0.5f);
                    break;
            }
        }

        return stat;
    }
}
