using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchLight : SpellBehaviour
{
    public override float GetRecovery(SkillProficiency proficiency) => 60f;

    protected override void OnCast(CombatEntity caster, int power, SkillProficiency proficiency)
    {
        Party.Instance.TorchLight(power * 60 * 60, proficiency);
    }
}
