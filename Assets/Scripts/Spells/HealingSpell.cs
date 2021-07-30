using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingSpell : SpellBehaviour
{
    [SerializeField] int NoviceRecovery;
    [SerializeField] int ExpertRecovery;
    [SerializeField] int MasterRecovery;

    [SerializeField] DiceRoll Roll;
    [SerializeField] int NoviceBaseValue;
    [SerializeField] int ExpertBaseValue;
    [SerializeField] int MasterBaseValue;

    [SerializeField] bool RollsPerSkillPoint;
    [SerializeField] bool SkillReducesRecovery;

    public override float GetRecovery(InventorySkill skill) =>
                    SkillReducesRecovery ? 140f - skill.Level :
                    skill.Proficiency == SkillProficiency.Novice ? NoviceRecovery :
                    skill.Proficiency == SkillProficiency.Expert ? ExpertRecovery : MasterRecovery;

    public override bool IsTargetValid(CombatEntity target)
    {
        if (target == null)
            return false;

        return target is PartyMember;
    }

    protected override void OnCast(CombatEntity caster, int power, SkillProficiency proficiency)
    {
        PartyController.Instance.QueueSpell(this, caster, power, proficiency);
    }

    public override void CastTarget(CombatEntity target)
    {
        PartyMember member = target as PartyMember;

        int power = 0;
        switch(_proficiency)
        {
            case SkillProficiency.Novice:
                power = NoviceBaseValue;
                break;
            case SkillProficiency.Expert:
                power = ExpertBaseValue;
                break;
            case SkillProficiency.Master:
                power = MasterBaseValue;
                break;
        }

        if (RollsPerSkillPoint)
        {
            power += Roll.MultiRoll(_potency);
        }
        else
            power += Roll.Roll();

        member.Vitals.GainHealthPoints(power);
        Destroy(this.gameObject);
    }
}
