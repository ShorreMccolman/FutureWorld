using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CombatHelper
{
    public static bool ShouldHit(int attack, int armorClass)
    {
        if (attack == int.MaxValue)
            return true;

        float chanceToHit = (float)(15 + 2 * attack) / (float)(30 + 2 * attack + armorClass);
        float rand = Random.Range(0f, 1f);
        return rand <= chanceToHit;
    }

    public static int ReduceDamage(int damage, float chanceOfReduction)
    {
        for(int i=0;i<4;i++)
        {
            float rand = Random.Range(0f, 1f);
            if (rand <= chanceOfReduction)
            {
                damage = Mathf.CeilToInt( damage * 0.5f);
            }
            else
                break;
        }
        return damage;
    }
}
