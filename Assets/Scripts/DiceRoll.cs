using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DiceRoll
{
    public int Rolls;
    public int Sides;

    public DiceRoll(int sides, int rolls)
    {
        Sides = sides;
        Rolls = rolls;
    }

    public int Roll()
    {
        int value = 0;
        for (int i = 0; i < Rolls; i++)
        {
            value += Random.Range(1, Sides + 1);
        }
        return value;
    }

    public int RollHigh()
    {
        int value = 0;
        for (int i = 0; i < Rolls; i++)
        {
            value += Sides;
        }
        return value;
    }

    public int RollLow()
    {
        int value = 0;

        for (int i = 0; i < Rolls; i++)
        {
            value += 1;
        }
        return value;
    }
}
