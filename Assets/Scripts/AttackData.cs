using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
    None,
    Fire,
    Electricity,
    Cold,
    Poison,
    Magic,
    Physical
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
