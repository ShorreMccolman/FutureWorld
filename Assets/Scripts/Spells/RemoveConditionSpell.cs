using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveConditionSpell : SpellBehaviour
{
    [SerializeField] StatusEffectOption Option;
    [SerializeField] StatusEffectOption Secondary;
    [SerializeField] StatusEffectOption SideEffect;

    public override float GetRecovery(SkillProficiency proficiency) => 140f;

    public override bool IsTargetValid(CombatEntity target)
    {
        return target is PartyMember;
    }

    protected override void OnCast(CombatEntity caster, int power, SkillProficiency proficiency)
    {
        PartyController.Instance.QueueSpell(this, caster, power, proficiency);
    }

    public override void CastTarget(CombatEntity target)
    {
        CureEntity(target, _potency, _proficiency);
        Destroy(this.gameObject);
    }

    void CureEntity(CombatEntity entity, int power, SkillProficiency proficiency)
    {
        float duration = 60 * 60;
        switch(proficiency)
        {
            case SkillProficiency.Novice:
                duration *= power * 3;
                break;
            case SkillProficiency.Expert:
                duration *= power * 60;
                break;
            case SkillProficiency.Master:
                duration *= power * 60 * 24;
                break;
        }

        PartyMember member = entity as PartyMember;
        member.Status.TryRemoveNegativeCondition(Option, duration);
        if(Secondary != StatusEffectOption.None)
            member.Status.TryRemoveNegativeCondition(Secondary, duration);
        if (SideEffect != StatusEffectOption.None)
            member.Status.AddCondition(SideEffect, 0f);
    }
}
