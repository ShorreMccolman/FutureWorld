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

    [SerializeField] int ExpertCost;
    [SerializeField] int MasterCost;

    [SerializeField] DiceRoll Roll;
    [SerializeField] bool UsesSkillForHit;

    public override float GetRecovery(SkillProficiency proficiency) => 
        proficiency == SkillProficiency.Novice ? NoviceRecovery : 
        proficiency == SkillProficiency.Expert ? ExpertRecovery : MasterRecovery;

    public override int AdjustCost(int cost, InventorySkill skill)
    {
        switch(skill.Proficiency)
        {
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
        projectile.SetDamage(name, attack, Roll.Roll());
        return projectile;
    }
}
