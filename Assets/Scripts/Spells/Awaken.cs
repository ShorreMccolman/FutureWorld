using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Awaken : SpellBehaviour
{
    public override float GetRecovery(InventorySkill skill) => 60f;

    protected override void OnCast(CombatEntity caster, int power, SkillProficiency proficiency)
    {
        foreach(var member in Party.Instance.Members)
        {
            member.Status.RemoveCondition(StatusEffectOption.Sleep);

            float expiry = power;
            switch(proficiency)
            {
                case SkillProficiency.Novice:
                    expiry *= 3 * 60;
                    break;
                case SkillProficiency.Expert:
                    expiry *= 60 * 60;
                    break;
                case SkillProficiency.Master:
                    expiry *= 24 * 60 * 60;
                    break;
            }

            member.Status.TryRemoveNegativeCondition(StatusEffectOption.MagicSleep, expiry);
        }
    }
}
