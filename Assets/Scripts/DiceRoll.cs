using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DiceRoll
{
    public int Rolls;
    public int Sides;
    public int Baseline;

    public DiceRoll(int sides, int rolls, int baseline = 0)
    {
        Sides = sides;
        Rolls = rolls;
        Baseline = baseline;
    }

    public int MultiRoll(int multiple)
    {
        int value = Baseline;
        for (int j = 0; j < multiple; j++)
        {
            for (int i = 0; i < Rolls; i++)
            {
                value += Random.Range(1, Sides + 1);
            }
        }
        return value;
    }

    public int Roll()
    {
        int value = Baseline;
        for (int i = 0; i < Rolls; i++)
        {
            value += Random.Range(1, Sides + 1);
        }
        return value;
    }

    public int RollHigh()
    {
        int value = Baseline;
        for (int i = 0; i < Rolls; i++)
        {
            value += Sides;
        }
        return value;
    }

    public int RollLow()
    {
        int value = Baseline;
        for (int i = 0; i < Rolls; i++)
        {
            value += 1;
        }
        return value;
    }
}
