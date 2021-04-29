using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSkillSelectButton : CharacterCreatorButton {

    [SerializeField] CharacterCreator creator;

    [SerializeField] Text SkillLabel;
    
    public string SkillName { get; private set; }

    public void Select()
    {
        creator.SelectCharacterSkill(this);
    }

    public void UpdateUI(string skillName, bool hasSkill)
    {
        SkillName = skillName;
        SkillLabel.text = string.IsNullOrEmpty(skillName) ? "None" : skillName;
        SkillLabel.color = hasSkill ? Color.blue : Color.black;
    }
}
