using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    BoostedLuck,

    BoostedFireR,
    BoostedElecR,
    BoostedColdR,
    BoostedPoisR,
    BoostedMagicR,

    MagicSleep,

    WizardEye,
    WizardEyeExpert,
    WizardEyeMaster,
    TorchLight,
    TorchLightExpert,
    TorchLightMaster,
    Bless,
}

[System.Serializable]
public class StatusEffect
{
    public string DisplayName;
    public StatusEffectOption Option;

    public string ExpressionOverride;
    public int OverridePriority;

    public bool EffectsParty;
    public bool NeedsHealing;
    public bool TicksUp;

    public bool IsActiveSpell;

    public List<StatusEffectOption> OverridedEffects = new List<StatusEffectOption>();
}

public class StatusEffectDatabase
{
    public static StatusEffectDatabase Instance;

    Dictionary<StatusEffectOption, StatusEffect> _seDict;

    public StatusEffectDatabase()
    {
        Instance = this;

        StatusEffectDBObject[] dbs = Resources.LoadAll<StatusEffectDBObject>("Database");

        _seDict = new Dictionary<StatusEffectOption, StatusEffect>();

        foreach (var db in dbs)
        {
            foreach (var se in db.Effects)
            {
                _seDict.Add(se.Option, se);
            }
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