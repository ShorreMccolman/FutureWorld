using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
    Physical,
    Fire,
    Electricity,
    Cold,
    Poison,
    Magic
}

[System.Serializable]
public class AttackData
{
    public AttackType Type;
    public DiceRoll DamageRoll;
    public int BaseDamage;
    public string OnHitEffect;

    public AttackType Missile;
}
