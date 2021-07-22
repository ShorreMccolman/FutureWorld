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

    [SerializeField] DiceRoll Roll;

    [SerializeField] int ProficiencyDiscount;
    [SerializeField] bool UsesSkillForHit;

    public override float GetRecovery(SkillProficiency proficiency) => 
        proficiency == SkillProficiency.Novice ? NoviceRecovery : 
        proficiency == SkillProficiency.Expert ? ExpertRecovery : MasterRecovery;

    public override int AdjustCost(int cost, InventorySkill skill)
    {
        int adjusted = cost;
        switch(skill.Proficiency)
        {
            case SkillProficiency.Expert:
                adjusted -= ProficiencyDiscount;
                break;
            case SkillProficiency.Master:
                adjusted -= ProficiencyDiscount * 2;
                break;
        }
        return adjusted;
    }

    public override bool IsTargetValid(CombatEntity target)
    {
        return target is Enemy;
    }

    protected override void OnCast()
    {
        Projectile projectile = ProjectileDatabase.Instance.GetProjectile(ProjectileID);
        int attack = int.MaxValue;
        if (UsesSkillForHit)
            attack = _potency;
        projectile.SetDamage(_caster.GetName(), attack, Roll.Roll());

        PartyController.Instance.CastProjectile(projectile);

        //TimeManagement.Instance.FreezeTime();
        //PartyController.Instance.QueueSpell(this);
    }

    //public override void CastFinal(CombatEntity target)
    //{
    //    TimeManagement.Instance.UnfreezeTime();
    //    Destroy(this.gameObject);
    //}
}
