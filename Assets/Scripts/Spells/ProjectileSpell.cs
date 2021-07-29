using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProjectileSpell : SpellBehaviour
{
    [SerializeField] string ProjectileID;
    [SerializeField] int NoviceRecovery;
    [SerializeField] int ExpertRecovery;
    [SerializeField] int MasterRecovery;

    [SerializeField] bool UseAdjustedCosts;
    [SerializeField] int NoviceCost;
    [SerializeField] int ExpertCost;
    [SerializeField] int MasterCost;

    [SerializeField] DiceRoll Roll;
    [SerializeField] bool UsesSkillForHit;
    [SerializeField] bool RollsPerSkillPoint;

    public override float GetRecovery(InventorySkill skill) => 
        skill.Proficiency == SkillProficiency.Novice ? NoviceRecovery : 
        skill.Proficiency == SkillProficiency.Expert ? ExpertRecovery : MasterRecovery;

    public override int AdjustCost(int cost, InventorySkill skill)
    {
        if (!UseAdjustedCosts)
            return cost;

        switch(skill.Proficiency)
        {
            case SkillProficiency.Novice:
                return NoviceCost;
            case SkillProficiency.Expert:
                return ExpertCost;
            case SkillProficiency.Master:
                return MasterCost;
        }
        return cost;
    }

    public override bool IsTargetValid(CombatEntity target)
    {
        if (target == null)
            return false;

        return target is Enemy;
    }

    protected override void OnCast(CombatEntity caster, int power, SkillProficiency proficiency)
    {
        if (TurnController.Instance.IsTurnBasedEnabled)
        {
            PartyController.Instance.QueueSpell(this, caster, power, proficiency);
        }
        else
        {
            PartyController.Instance.CastProjectile(MakeProjectile(DisplayName));
        }
    }

    public override void CastTarget(CombatEntity target)
    {
        PartyController.Instance.CastProjectile(MakeProjectile(DisplayName), target.GetEntity());
        Destroy(this.gameObject);
    }

    Projectile MakeProjectile(string name)
    {
        Projectile projectile = ProjectileDatabase.Instance.GetProjectile(ProjectileID);
        int attack = int.MaxValue;
        if (UsesSkillForHit)
            attack = _potency;

        int damage = 0;
        if (RollsPerSkillPoint)
        {
            for(int i=0;i<_potency;i++)
            {
                damage += Roll.Roll();
            }
        }
        else
            damage = Roll.Roll();

        projectile.SetDamage(name, attack, damage);
        return projectile;
    }
}
