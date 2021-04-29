using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour {

    [SerializeField] Text SkillNameLabel;
    [SerializeField] Text SkillLevelLabel;

    InventorySkill _skill;
    SkillsMenu _menu;

    public void Setup(InventorySkill skill, SkillsMenu menu)
    {
        _skill = skill;
        _menu = menu;

        string label = skill.Skill.DisplayName;
        if (skill.Proficiency != SkillProficiency.Novice)
            label += " (" + GameConstants.LabelForProficiency[skill.Proficiency] + ")";
  
        SkillNameLabel.text = label;
        SkillLevelLabel.text = skill.Level.ToString();
    }

	public void Select()
    {
        bool success = PartyController.Instance.ActiveMember.Profile.TryUpgradeSkill(_skill);
        if(success)
        {
            HUD.Instance.ExpressSelectedMember(GameConstants.EXPRESSION_HAPPY, GameConstants.EXPRESSION_HAPPY_DURATION);
            _menu.UpdateDisplay();
        } else
        {
            HUD.Instance.SendInfoMessage("You don't have enough skill points!", 2.0f);
        }
    }

    public void OnHover()
    {
        int difference;
        if (PartyController.Instance.ActiveMember.Profile.CanUpgradeSkill(_skill, out difference))
        {
            SkillNameLabel.color = Color.blue;
            SkillLevelLabel.color = Color.blue;
        }
        else
        {
            HUD.Instance.SendInfoMessage("You need " + difference + " more skill points to advance here");
        }
    }

    public void OffHover()
    {
        SkillNameLabel.color = Color.white;
        SkillLevelLabel.color = Color.white;
    }
}
