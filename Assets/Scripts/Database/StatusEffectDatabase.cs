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
    BoostedResistance
}


[System.Serializable]
public class StatusEffect
{
    public StatusEffectOption Option;
    public string DisplayName;
    public bool NeedsHealing;
    public bool TicksUp;

    public StatusEffect(StatusEffectOption option, string name, bool needsHealing, bool ticksUp)
    {
        Option = option;
        DisplayName = name;
        NeedsHealing = needsHealing;
        TicksUp = ticksUp;
    }

    public static List<StatusEffect> Effects = new List<StatusEffect>()
    {
        { new StatusEffect(StatusEffectOption.Sleep, "Sleep", true, false) },
        { new StatusEffect(StatusEffectOption.Rested, "Rested", false, false) },
        { new StatusEffect(StatusEffectOption.Poison, "Poison", true, true) },
        { new StatusEffect(StatusEffectOption.Disease, "Disease", true, true) },
        { new StatusEffect(StatusEffectOption.Weak, "Weak", true, true) },
        { new StatusEffect(StatusEffectOption.BoostedAC, "BoostedAC", false, false) },
        { new StatusEffect(StatusEffectOption.BoostedStats, "BoostedStats", false, false) },
        { new StatusEffect(StatusEffectOption.BoostedResistance, "BoostedResist", false, false) }
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
}