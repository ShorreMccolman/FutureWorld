using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WizardEye : SpellBehaviour
{
    public override float GetRecovery(SkillProficiency proficiency) => 60f;

    protected override void OnCast(CombatEntity caster, int power, SkillProficiency proficiency)
    {
        StatusEffectOption option = proficiency == SkillProficiency.Novice ? StatusEffectOption.WizardEye : 
                                    proficiency == SkillProficiency.Expert ? StatusEffectOption.WizardEyeExpert : 
                                                                             StatusEffectOption.WizardEyeMaster;

        Party.Instance.Status.AddCondition(option, power * 60 * 60);
    }
}
