using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WizardEye : SpellBehaviour
{
    public override float GetRecovery(SkillProficiency proficiency) => 60f;

    protected override void OnCast(CombatEntity caster, int power, SkillProficiency proficiency)
    {
        Party.Instance.WizardsEye(power * 60 * 60, proficiency);
    }
}
