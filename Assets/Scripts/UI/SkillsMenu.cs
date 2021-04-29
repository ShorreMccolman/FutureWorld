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

        Vector3 lastPos = WeaponLabel.transform.position;
        List<InventorySkill> skills = member.Skillset.GetSkillsByType(SkillType.Weapon);
        for(int i=0;i<skills.Count;i++)
        {
            GameObject obj = Instantiate(ButtonTemplate, WeaponLabel.transform.parent);
            lastPos = WeaponLabel.transform.position + Vector3.down * (i * 22 + 30);
            obj.transform.position = lastPos;
            buttonObjects.Add(obj);

            SkillButton btn = obj.GetComponent<SkillButton>();
            btn.Setup(skills[i], this);
        }

        MagicLabel.transform.position = lastPos + Vector3.down * 50;
        skills = member.Skillset.GetSkillsByType(SkillType.Magic);
        for (int i = 0; i < skills.Count; i++)
        {
            GameObject obj = Instantiate(ButtonTemplate, MagicLabel.transform.parent);
            lastPos = MagicLabel.transform.position + Vector3.down * (i * 22 + 30);
            obj.transform.position = lastPos;
            buttonObjects.Add(obj);

            SkillButton btn = obj.GetComponent<SkillButton>();
            btn.Setup(skills[i], this);
        }

        lastPos = ArmorLabel.transform.position;
        skills = member.Skillset.GetSkillsByType(SkillType.Armor);
        for (int i = 0; i < skills.Count; i++)
        {
            GameObject obj = Instantiate(ButtonTemplate, ArmorLabel.transform.parent);
            lastPos = ArmorLabel.transform.position + Vector3.down * (i * 22 + 30);
            obj.transform.position = lastPos;
            buttonObjects.Add(obj);

            SkillButton btn = obj.GetComponent<SkillButton>();
            btn.Setup(skills[i], this);
        }

        MiscLabel.transform.position = lastPos + Vector3.down * 50;
        skills = member.Skillset.GetSkillsByType(SkillType.Misc);
        for (int i = 0; i < skills.Count; i++)
        {
            GameObject obj = Instantiate(ButtonTemplate, MiscLabel.transform.parent);
            lastPos = MiscLabel.transform.position + Vector3.down * (i * 22 + 30);
            obj.transform.position = lastPos;
            buttonObjects.Add(obj);

            SkillButton btn = obj.GetComponent<SkillButton>();
            btn.Setup(skills[i], this);
        }
    }
}
