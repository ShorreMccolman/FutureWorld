using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffSpellA: SpellBehaviour
{
    [SerializeField] StatusEffectOption Option;

    public override float GetRecovery(InventorySkill skill) =>
                    skill.Proficiency == SkillProficiency.Master ? 120 : 140;

    public override bool IsTargetValid(CombatEntity target)
    {
        return target is PartyMember;
    }

    protected override void OnCast(CombatEntity caster, int power, SkillProficiency proficiency)
    {
        if (proficiency != SkillProficiency.Master)
        {
            PartyController.Instance.QueueSpell(this, caster, power, proficiency);
        }
        else
        {
            foreach (var member in Party.Instance.Members)
                BuffEntity(member, power, proficiency);
        }
    }

    public override void CastTarget(CombatEntity target)
    {
        BuffEntity(target, _potency, _proficiency);
        Destroy(this.gameObject);
    }

    void BuffEntity(CombatEntity entity, int power, SkillProficiency proficiency)
    {
        PartyMember member = entity as PartyMember;

        int potency = 10;
        if (proficiency == SkillProficiency.Novice)
            potency += 2 * power;
        else
            potency += 3 * power;

        member.Status.AddCondition(Option, potency, 60 * 60 * power);
    }
}
