using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffSpellB : SpellBehaviour
{
    [SerializeField] StatusEffectOption Option;

    public override float GetRecovery(InventorySkill skill) => 
        skill.Proficiency == SkillProficiency.Novice ? 140f : 100f;

    public override bool IsTargetValid(CombatEntity target)
    {
        return target is PartyMember;
    }

    protected override void OnCast(CombatEntity caster, int power, SkillProficiency proficiency)
    {
        if (proficiency == SkillProficiency.Novice)
        {
            PartyController.Instance.QueueSpell(this, caster, power, proficiency);
        }
        else
        {
            foreach(var member in Party.Instance.Members)
                BlessEntity(member, power, proficiency);
        }
    }

    public override void CastTarget(CombatEntity target)
    {
        BlessEntity(target, _potency, _proficiency);
        Destroy(this.gameObject);
    }

    void BlessEntity(CombatEntity entity, int power, SkillProficiency proficiency)
    {
        PartyMember member = entity as PartyMember;
        member.Status.AddCondition(Option, 5 + power, 60 * 60 + power * 60 * (proficiency == SkillProficiency.Novice ? 3 : 15));
    }
}
