using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WizardEye : SpellBehaviour
{
    public override void Cast(InventorySkill skill)
    {
        Party.Instance.WizardsEye(skill.Level * 60 * 60, skill.Proficiency);
    }
}
