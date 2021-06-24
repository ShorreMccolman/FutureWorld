using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EffectiveStat
{
    public CharacterStat Stat;
    public int BaseValue;
    public int Bonus;
    public float Multiplier;

    public EffectiveStat(CharacterStat stat, int value)
    {
        Stat = stat;
        BaseValue = value;
        Bonus = 0;
        Multiplier = 1f;
    }

    public void Flush()
    {
        Bonus = 0;
        Multiplier = 1f;
    }

    public void IncreaseBase(int value)
    {
        BaseValue += value;
    }

    public void AddToBonus(int value)
    {
        Bonus += value;
    }

    public void ReduceMultiplier(float value)
    {
        Multiplier = Mathf.Min(value, Multiplier);
    }

    public int Evaluate()
    {
        return Mathf.FloorToInt(Multiplier * (BaseValue + Bonus));
    }
}

public abstract class EffectiveStats
{
    protected Dictionary<CharacterStat, EffectiveStat> _dict;

    public void Flush()
    {
        foreach (var stat in _dict.Values)
        {
            stat.Flush();
        }
    }

    public EffectiveStat GetStat(CharacterStat stat)
    {
        if (!_dict.ContainsKey(stat))
            return new EffectiveStat(CharacterStat.None, 0);

        return _dict[stat];
    }

    public int Evaluate(CharacterStat stat)
    {
        if (!_dict.ContainsKey(stat))
            return 0;

        return _dict[stat].Evaluate();
    }
}