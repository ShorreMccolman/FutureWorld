using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchLight : SpellBehaviour
{
    public override float GetRecovery(InventorySkill skill) => 60f;

    protected override void OnCast(CombatEntity caster, int power, SkillProficiency proficiency)
    {
        StatusEffectOption option = proficiency == SkillProficiency.Novice ? StatusEffectOption.TorchLight :
                            proficiency == SkillProficiency.Expert ? StatusEffectOption.TorchLightExpert :
                                                                     StatusEffectOption.TorchLightMaster;

        Party.Instance.Status.AddCondition(option, power * 60 * 60);
    }
}
