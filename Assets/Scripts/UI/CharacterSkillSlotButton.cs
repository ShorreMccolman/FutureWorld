using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSkillSlotButton : MonoBehaviour {

    [SerializeField] CharacterCreatorUI creatorUI;

    [SerializeField] Text SkillLabel;

    public string SkillName { get; private set; }

    Color blue = new Color(0, 235f, 255f);

    public void Select()
    {
        if (string.IsNullOrEmpty(SkillName))
            return;

        creatorUI.RemoveCharacterSkill(this);
    }

    public void UpdateUI(string skillName)
    {
        SkillName = skillName;
        SkillLabel.text = string.IsNullOrEmpty(skillName) ? "None" : skillName;
        SkillLabel.color = string.IsNullOrEmpty(skillName) ? blue : Color.white;
    }
}
