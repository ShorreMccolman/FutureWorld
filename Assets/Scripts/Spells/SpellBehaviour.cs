using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpellBehaviour : MonoBehaviour
{
    public virtual bool IsRanged => false;
    public virtual bool TargetsFriendly => false;

    public virtual int AdjustCost(int cost, InventorySkill skill) { return cost; }

    public virtual void Cast(InventorySkill skill) { }
}
