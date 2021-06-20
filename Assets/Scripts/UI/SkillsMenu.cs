using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillsMenu : CharacterMenu
{
    [SerializeField] Text Name;
    [SerializeField] Text Skillpoints;

    [SerializeField] Text WeaponLabel;
    [SerializeField] Text ArmorLabel;
    [SerializeField] Text MagicLabel;
    [SerializeField] Text MiscLabel;

    [SerializeField] GameObject ButtonTemplate;

    List<GameObject> buttonObjects = new List<GameObject>();

    PartyMember _member;

    public void UpdateDisplay()
    {
        Setup(_member);
    }

    public override void Setup(PartyMember member)
    {
        _member = member;

        foreach (var button in buttonObjects)
            Destroy(button);
        buttonObjects.Clear();

        Name.text = "Skills for " + member.Profile.CharacterName;
        Skillpoints.text = "Skill Points: " + member.Profile.SkillPoints;

        float lastOffset = 0;
        List<InventorySkill> skills = member.Skillset.GetSkillsByType(SkillType.Weapon);
        for(int i=0;i<skills.Count;i++)
        {
            lastOffset = i * 22 + 30;
            AddButton(skills[i], WeaponLabel.transform, lastOffset);
        }
        if(skills.Count == 0)
        {
            lastOffset = 30;
            AddButton(null, WeaponLabel.transform, lastOffset);
        }

        MagicLabel.transform.position = WeaponLabel.transform.position + Vector3.down * (lastOffset + 50);
        skills = member.Skillset.GetSkillsByType(SkillType.Magic);
        for (int i = 0; i < skills.Count; i++)
        {
            lastOffset = i * 22 + 30;
            AddButton(skills[i], MagicLabel.transform, lastOffset);
        }
        if (skills.Count == 0)
        {
            lastOffset = 30;
            AddButton(null, MagicLabel.transform, lastOffset);
        }

        skills = member.Skillset.GetSkillsByType(SkillType.Armor);
        for (int i = 0; i < skills.Count; i++)
        {
            lastOffset = i * 22 + 30;
            AddButton(skills[i], ArmorLabel.transform, lastOffset);
        }
        if (skills.Count == 0)
        {
            lastOffset = 30;
            AddButton(null, ArmorLabel.transform, lastOffset);
        }

        MiscLabel.transform.position = ArmorLabel.transform.position + Vector3.down * (lastOffset + 50);
        skills = member.Skillset.GetSkillsByType(SkillType.Misc);
        for (int i = 0; i < skills.Count; i++)
        {
            lastOffset = i * 22 + 30;
            AddButton(skills[i], MiscLabel.transform, lastOffset);
        }
        if (skills.Count == 0)
        {
            lastOffset = 30;
            AddButton(null, MiscLabel.transform, lastOffset);
        }
    }

    void AddButton(InventorySkill skill, Transform anchor, float offset)
    {
        GameObject obj = Instantiate(ButtonTemplate, anchor.parent);
        obj.transform.position = anchor.position + Vector3.down * offset;
        buttonObjects.Add(obj);

        SkillButton btn = obj.GetComponent<SkillButton>();
        btn.Setup(_member, skill, this);
    }
}
