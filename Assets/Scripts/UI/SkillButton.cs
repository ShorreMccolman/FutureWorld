using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillButton : MonoBehaviour, IPopable, IInfoMessenger, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Text SkillNameLabel;
    [SerializeField] Text SkillLevelLabel;

    InventorySkill _skill;
    SkillsMenu _menu;

    public void Setup(PartyMember member, InventorySkill skill, SkillsMenu menu)
    {
        _skill = skill;
        _menu = menu;

        if(skill == null)
        {
            SkillNameLabel.text = "None";
            SkillLevelLabel.text = "";

            SkillNameLabel.color = Color.white;
            SkillLevelLabel.color = Color.white;
        } 
        else
        {
            string label = skill.Data.DisplayName;
            if (skill.Proficiency != SkillProficiency.Novice)
                label += " (" + GameConstants.LabelForProficiency[skill.Proficiency] + ")";

            SkillNameLabel.text = label;
            SkillLevelLabel.text = skill.Level.ToString();

            int diff;
            if(member.Profile.CanUpgradeSkill(skill, out diff))
            {
                SkillNameLabel.color = Color.blue;
                SkillLevelLabel.color = Color.blue;
            } 
            else
            {
                SkillNameLabel.color = Color.white;
                SkillLevelLabel.color = Color.white;
            }
        }
    }

	public void Select()
    {
        if (_skill == null)
            return;

        bool success = Party.Instance.ActiveMember.Profile.TryUpgradeSkill(_skill);
        if(success)
        {
            Party.Instance.ActiveMember.Vitals.Express(GameConstants.EXPRESSION_HAPPY, GameConstants.EXPRESSION_HAPPY_DURATION);
            _menu.UpdateDisplay();
        } else
        {
            InfoMessageReceiver.Send("You don't have enough skill points!", 2.0f);
        }

        int difference;
        if (Party.Instance.ActiveMember.Profile.CanUpgradeSkill(_skill, out difference))
        {
            SkillNameLabel.color = Color.green;
            SkillLevelLabel.color = Color.green;
        }
        else
        {
            SkillNameLabel.color = Color.red;
            SkillLevelLabel.color = Color.red;
        }
    }

    public void ShowPopup()
    {
        if (_skill == null)
            return;

        ProfilePopupInfo info = ProfilePopupInfoDatabase.Instance.GetInfo(_skill.ID);
        string title = info.Title;
        string body = info.Body + "\n";
        body += "\nNovice: " + info.Extra[0];
        body += "\nExpert:  " + info.Extra[1];
        body += "\nMaster: " + info.Extra[2];
        HUD.Instance.Popups.ShowText(title, body, 16, TextAnchor.UpperLeft);
    }

    public string GetInfoMessage()
    {
        if (_skill == null)
            return "";

        string result;
        int difference;
        if (Party.Instance.ActiveMember.Profile.CanUpgradeSkill(_skill, out difference))
        {
            result = "Clicking here will spend " + (_skill.Level + 1) + " skill points";
        }
        else
        {
            result = "You need " + difference + " more skill points to advance here";
        }
        return result;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_skill == null)
            return;

        int difference;
        if (Party.Instance.ActiveMember.Profile.CanUpgradeSkill(_skill, out difference))
        {
            SkillNameLabel.color = Color.green;
            SkillLevelLabel.color = Color.green;
        }
        else
        {
            SkillNameLabel.color = Color.red;
            SkillLevelLabel.color = Color.red;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_skill == null)
            return;

        int difference;
        if (Party.Instance.ActiveMember.Profile.CanUpgradeSkill(_skill, out difference))
        {
            SkillNameLabel.color = Color.blue;
            SkillLevelLabel.color = Color.blue;
        }
        else
        {
            SkillNameLabel.color = Color.white;
            SkillLevelLabel.color = Color.white;
        }       
    }

}
