using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class Status : GameStateEntity
{
    List<StatusCondition> _conditions;

    public event System.Action<int, bool, bool> OnStatusIconChanged;
    public event System.Action OnStatusChanged;

    public static event System.Action<bool, SkillProficiency> OnWizardEyeChanged;
    public static event System.Action<bool, SkillProficiency> OnTorchChanged;

    public Status(GameStateEntity parent, bool needsRest = true) : base(parent)
    {
        _conditions = new List<StatusCondition>();
        if(needsRest)
            AddCondition(StatusEffectOption.Rested, GameConstants.REST_DURATION, false);
    }

    public Status(GameStateEntity parent, XmlNode node) : base(parent, node)
    {
        XmlNode statusNode = node.SelectSingleNode("Status");
        Populate<StatusCondition>(ref _conditions, typeof(Status), node, "Conditions", "StatusCondition");
    }

    public override XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("Status");
        element.AppendChild(XmlHelper.Attribute(doc, "Conditions", _conditions));
        element.AppendChild(base.ToXml(doc));
        return element;
    }

    public string GetStatusCondition()
    {
        string status = "Good";

        int currentPriority = 0;
        foreach (var condition in _conditions)
        {
            if (condition.Effect.OverridePriority > currentPriority)
            {
                status = condition.Effect.DisplayName;
                currentPriority = condition.Effect.OverridePriority;
            }
        }

        return status;
    }

    public string GetActiveSpells(bool partySpells = false)
    {
        string text = "";
        foreach(var condition in _conditions)
        {
            if(condition.Effect.IsActiveSpell && ((partySpells && condition.Effect.EffectsParty) || (!partySpells && !condition.Effect.EffectsParty)))
            {
                float mins = condition.Duration / 60;
                float seconds = condition.Duration % 60;
                text += condition.Effect.DisplayName + "    " + (int)mins + " mins " + (int)seconds + " seconds\n";
            }
        }
        if (string.IsNullOrEmpty(text))
            text = "None";

        return text;
    }

    public void OverrideExpression(ref string expression)
    {
        int currentPriority = 0;
        foreach(var condition in _conditions)
        {
            if(condition.Effect.OverridePriority > currentPriority)
            {
                expression = condition.Effect.ExpressionOverride;
                currentPriority = condition.Effect.OverridePriority;
            }   
        }
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
            RemoveCondition(option, false);
        AddCondition(StatusEffectOption.Rested, GameConstants.REST_DURATION);
    }

    public void CompleteCondition(StatusCondition condition)
    {
        condition.OnConditionComplete -= CompleteCondition;
        condition.Terminate();
        _conditions.Remove(condition);
        switch(condition.Effect.Option)
        {
            default:
                break;
            case StatusEffectOption.Sleep:
                AddCondition(StatusEffectOption.Rested, GameConstants.REST_DURATION);
                break;
            case StatusEffectOption.Rested:
                AddCondition(StatusEffectOption.Weak, 0);
                break;

            case StatusEffectOption.WizardEye:
            case StatusEffectOption.WizardEyeExpert:
            case StatusEffectOption.WizardEyeMaster:
                OnWizardEyeChanged?.Invoke(false, SkillProficiency.Novice);
                break;

            case StatusEffectOption.TorchLight:
            case StatusEffectOption.TorchLightExpert:
            case StatusEffectOption.TorchLightMaster:
                OnTorchChanged?.Invoke(false, SkillProficiency.Novice);
                break;
        }

        if (condition.Effect.IconSlot != -1)
            OnStatusIconChanged?.Invoke(condition.Effect.IconSlot, condition.Effect.EffectsParty, false);
        OnStatusChanged?.Invoke();
    }

    public void AddCondition(StatusEffectOption option, int potency, float duration, bool update = true)
    {
        for (int i = 0; i < _conditions.Count; i++)
        {
            if (_conditions[i].Option == option)
            {
                _conditions[i] = StatusEffectDatabase.Instance.GetStatusCondition(option, this, potency, duration);
                return;
            }
        }

        StatusCondition condition = StatusEffectDatabase.Instance.GetStatusCondition(option, this, potency, duration);
        condition.OnConditionComplete += CompleteCondition;
        _conditions.Add(condition);
        OnAddCondition(option);
        if(condition.Effect.IconSlot != -1)
            OnStatusIconChanged?.Invoke(condition.Effect.IconSlot, condition.Effect.EffectsParty, true);
        if (update)
            OnStatusChanged?.Invoke();
    }

    public void AddCondition(StatusEffectOption option, float duration, bool update = true)
    {
        for(int i=0;i<_conditions.Count;i++)
        {
            if (_conditions[i].Option == option)
            {
                _conditions[i] = StatusEffectDatabase.Instance.GetStatusCondition(option, this, duration);
                return;
            }
        }
        StatusCondition condition = StatusEffectDatabase.Instance.GetStatusCondition(option, this, duration);
        foreach (var effect in condition.Effect.OverridedEffects)
            RemoveCondition(effect, false);

        condition.OnConditionComplete += CompleteCondition;
        _conditions.Add(condition);
        OnAddCondition(option);
        if (condition.Effect.IconSlot != -1)
            OnStatusIconChanged?.Invoke(condition.Effect.IconSlot, condition.Effect.EffectsParty, true);
        if (update)
            OnStatusChanged?.Invoke();
    }

    void OnAddCondition(StatusEffectOption option)
    {
        switch (option)
        {
            default:
                break;

            case StatusEffectOption.WizardEye:
                OnWizardEyeChanged?.Invoke(true, SkillProficiency.Novice);
                break;
            case StatusEffectOption.WizardEyeExpert:
                OnWizardEyeChanged?.Invoke(true, SkillProficiency.Expert);
                break;
            case StatusEffectOption.WizardEyeMaster:
                OnWizardEyeChanged?.Invoke(true, SkillProficiency.Master);
                break;

            case StatusEffectOption.TorchLight:
                OnTorchChanged?.Invoke(true, SkillProficiency.Novice);
                break;
            case StatusEffectOption.TorchLightExpert:
                OnTorchChanged?.Invoke(true, SkillProficiency.Expert);
                break;
            case StatusEffectOption.TorchLightMaster:
                OnTorchChanged?.Invoke(true, SkillProficiency.Master);
                break;
        }
    }

    public void RemoveCondition(StatusEffectOption option, bool update = true)
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

        if (index >= 0)
        {
            _conditions[index].OnConditionComplete -= CompleteCondition;
            _conditions[index].Terminate();
            _conditions.RemoveAt(index);
            if (_conditions[index].Effect.IconSlot != -1)
                OnStatusIconChanged?.Invoke(_conditions[index].Effect.IconSlot, _conditions[index].Effect.EffectsParty, false);
            if (update)
                OnStatusChanged?.Invoke();
        }
    }

    public bool TryRemoveNegativeCondition(StatusEffectOption option, float expiry)
    {
        if (!HasCondition(option))
            return false;

        StatusCondition condition = _conditions.Find(x => x.Option == option);
        if(condition.Duration <= expiry)
        {
            RemoveCondition(option);
            return true;
        }

        return false;
    }

    public void SwapConditions(StatusEffectOption remove, StatusEffectOption add, float duration, bool update = true)
    {
        RemoveCondition(remove, false);
        AddCondition(add, duration, update);
    }

    public void ModifyStats(EffectiveStats stats)
    {
        foreach(var condition in _conditions)
        {
            condition.ModifyStats(stats);
        }
    }

    public int ModifyRestHP(int stat)
    {
        foreach (var condition in _conditions)
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

    public int ModifyRestSP(int stat)
    {
        foreach (var condition in _conditions)
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
        foreach (var condition in _conditions)
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
