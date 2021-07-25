using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum StatusEffectOption
{
    None = 0,
    Sleep = 1,
    Rested = 2,

    Weak = 10,
    Fear = 11,
    Poison = 12,
    Disease = 13,
    Paralysis = 14,
    Curse = 15,
    MagicSleep = 16,
    MagicWeak = 17,
    Stoned = 18,
    Insane = 19,
    Dead = 20,
    Incinerated = 21,

    BoostedAC = 100,
    BoostedStats = 101,
    BoostedResistance = 102,
    BoostedMight = 103,
    BoostedPersonality = 104,
    BoostedIntellect = 105,
    BoostedEndurance = 106,
    BoostedAccuracy = 107,
    BoostedSpeed = 108,
    BoostedLuck = 109,
    BoostedFireR = 110,
    BoostedElecR = 111,
    BoostedColdR = 112,
    BoostedPoisR = 113,
    BoostedMagicR = 114,

    WizardEye = 200,
    WizardEyeExpert = 202,
    WizardEyeMaster = 203,
    TorchLight = 204,
    TorchLightExpert = 205,
    TorchLightMaster = 206,
    Bless = 207,
    Haste = 208,
    Feather = 209,
    MagicShield = 210,
    Jumping = 211,
    Flying = 212,
    WaterWalking = 213,
    StoneSkin = 214,
    Guardian = 215,
    Heroism = 216,
    Lucky = 217,
    Meditation = 218,
    Precision = 219,
    Speed = 220,
    Power = 221,
    ProtFire = 222,
    ProtElec = 223,
    ProtCold = 224,
    ProtPois = 225,
    ProtMagic = 226,
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
    public bool IsTemporary;

    public bool IsActiveSpell;
    public int IconSlot = -1;

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