using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtectionSpell : SpellBehaviour
{
    [SerializeField] StatusEffectOption EffectOption;

    public override float GetRecovery(SkillProficiency proficiency) => 120f;

    protected override void OnCast(CombatEntity caster, int power, SkillProficiency proficiency)
    {
        float duration = proficiency == SkillProficiency.Novice ? power : proficiency == SkillProficiency.Expert ? power * 2 : power * 3;

        duration *= 60 * 60;

        Party.Instance.Status.AddCondition(EffectOption, duration);
        foreach (var member in Party.Instance.Members)
            member.Status.AddCondition(EffectOption, duration);
    }
}
