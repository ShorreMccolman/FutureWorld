using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchLight : SpellBehaviour
{
    public override float GetRecovery(SkillProficiency proficiency) => 60f;

    protected override void OnCast()
    {
        Party.Instance.TorchLight(_potency * 60 * 60, _proficiency);
    }
}
