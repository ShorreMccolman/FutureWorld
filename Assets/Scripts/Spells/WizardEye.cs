using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WizardEye : SpellBehaviour
{
    public override float GetRecovery(SkillProficiency proficiency) => 60f;

    protected override void OnCast()
    {
        Party.Instance.WizardsEye(_potency * 60 * 60, _proficiency);
    }
}
