using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Poison:  Reduces might, endurance, speed and accuracy to 75%
 *          Reduces max HP and MP after rest to 50%
 * 
 * Disease: Reduces might, endurance, speed and accuracy to 60%
 *          Reduces max HP and MP after rest to 50%
 * 
 * Weak:    Reduces all outgoing damage to 50%
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 */

public enum StatusEffectOption
{
    Sleep,
    Rested,

    Poison,
    Disease,
    Weak,

    BoostedAC,
    BoostedStats,
    BoostedResistance,

    BoostedMight,
    BoostedPersonality,
    BoostedIntellect,
    BoostedEndurance,
    BoostedAccuracy,
    BoostedSpeed,
    BoostedLuck
}


[System.Serializable]
public class StatusEffect
{
    public StatusEffectOption Option;
    public string DisplayName;
    public bool NeedsHealing;
    public bool TicksUp;
    public string ExpressionOverride;
    public int OverridePriority;

    public StatusEffect(StatusEffectOption option, string name, string expression, int priority, bool needsHealing, bool ticksUp)
    {
        Option = option;
        DisplayName = name;
        ExpressionOverride = expression;
        OverridePriority = priority;
        NeedsHealing = needsHealing;
        TicksUp = ticksUp;
    }

    public static List<StatusEffect> Effects = new List<StatusEffect>()
    {
        { new StatusEffect(StatusEffectOption.Sleep, "Sleep", GameConstants.EXPRESSION_SLEEP, 100, true, false) },
        { new StatusEffect(StatusEffectOption.Rested, "Rested", null, 0, false, false) },
        { new StatusEffect(StatusEffectOption.Disease, "Disease", GameConstants.EXPRESSION_DISEASE, 99, true, true) },
        { new StatusEffect(StatusEffectOption.Poison, "Poison", GameConstants.EXPRESSION_POISON, 98, true, true) },
        { new StatusEffect(StatusEffectOption.Weak, "Weak", GameConstants.EXPRESSION_WEAK, 97, true, true) },
        { new StatusEffect(StatusEffectOption.BoostedAC, "BoostedAC", null, 0, false, false) },
        { new StatusEffect(StatusEffectOption.BoostedStats, "BoostedStats", null, 0, false, false) },
        { new StatusEffect(StatusEffectOption.BoostedResistance, "BoostedResist", null, 0, false, false) },

        { new StatusEffect(StatusEffectOption.BoostedMight, "BoostedMight", null, 0, false, false) },
        { new StatusEffect(StatusEffectOption.BoostedIntellect, "BoostedIntellect", null, 0, false, false) },
        { new StatusEffect(StatusEffectOption.BoostedPersonality, "BoostedPersonality", null, 0, false, false) },
        { new StatusEffect(StatusEffectOption.BoostedAccuracy, "BoostedAccuracy", null, 0, false, false) },
        { new StatusEffect(StatusEffectOption.BoostedSpeed, "BoostedSpeed", null, 0, false, false) },
        { new StatusEffect(StatusEffectOption.BoostedEndurance, "BoostedEndurance", null, 0, false, false) },
        { new StatusEffect(StatusEffectOption.BoostedLuck, "BoostedLuck", null, 0, false, false) }

    };
}

public class StatusEffectDatabase
{
    public static StatusEffectDatabase Instance;

    Dictionary<StatusEffectOption, StatusEffect> _seDict;

    public StatusEffectDatabase()
    {
        Instance = this;

        _seDict = new Dictionary<StatusEffectOption, StatusEffect>();

        foreach (var se in StatusEffect.Effects)
        {
            _seDict.Add(se.Option, se);
        }
    }

    public StatusEffect GetEffect(StatusEffectOption option)
    {
        if (!_seDict.ContainsKey(option))
        {
            return null;
        }

        return _seDict[option];
    }

    public StatusCondition GetStatusCondition(StatusEffectOption option, Status status, float duration)
    {
        if (!_seDict.ContainsKey(option))
        {
            return null;
        }

        return new StatusCondition(status, _seDict[option], duration);
    }

    public StatusCondition GetStatusCondition(StatusEffectOption option, Status status, int potency, float duration)
    {
        if (!_seDict.ContainsKey(option))
        {
            return null;
        }

        return new StatusCondition(status, _seDict[option], potency, duration);
    }
}