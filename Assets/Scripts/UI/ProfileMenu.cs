using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileMenu : CharacterMenu
{
    [SerializeField] Text Name;
    [SerializeField] Text Skillpoints;

    [SerializeField] Text Might;
    [SerializeField] Text Intellect;
    [SerializeField] Text Personality;
    [SerializeField] Text Endurance;
    [SerializeField] Text Accuracy;
    [SerializeField] Text Speed;
    [SerializeField] Text Luck;

    [SerializeField] Text Hitpoints;
    [SerializeField] Text Spellpoints;
    [SerializeField] Text Armorclass;

    [SerializeField] Text Condition;
    [SerializeField] Text QuickSpell;

    [SerializeField] Text Age;
    [SerializeField] Text Level;
    [SerializeField] Text Experience;

    [SerializeField] Text Attack;
    [SerializeField] Text Damage;
    [SerializeField] Text Shoot;
    [SerializeField] Text ShootDamage;

    [SerializeField] Text Fire;
    [SerializeField] Text Electricity;
    [SerializeField] Text Cold;
    [SerializeField] Text Poison;
    [SerializeField] Text Magic;

    public override void Setup(PartyMember member)
    {
        Name.text = member.Profile.FullName;
        Skillpoints.text = "Skill Points: " + member.Profile.SkillPoints;

        Might.text = Label(member.Profile.EffectiveMight, member.Profile.Might);
        Intellect.text = Label(member.Profile.EffectiveIntellect, member.Profile.Intellect);
        Personality.text = Label(member.Profile.EffectivePersonality, member.Profile.Personality);
        Endurance.text = Label(member.Profile.EffectiveEndurance, member.Profile.Endurance);
        Accuracy.text = Label(member.Profile.EffectiveAccuracy, member.Profile.Accuracy);
        Speed.text = Label(member.Profile.EffectiveSpeed, member.Profile.Speed);
        Luck.text = Label(member.Profile.EffectiveLuck, member.Profile.Luck);

        Hitpoints.text = Label(member.Vitals.CurrentHP,member.Vitals.EffectiveTotalHP);
        Spellpoints.text = Label(member.Vitals.CurrentMP,member.Vitals.EffectiveTotalMP);
        Armorclass.text = Label(member.Vitals.EffectiveArmorClass);

        Condition.text = member.EffectiveStatusCondition();
        QuickSpell.text = member.Profile.QuickSpell;

        Age.text = Label(member.Profile.Age);
        Level.text = Label(member.Profile.Level);
        Experience.text = member.Profile.Experience.ToString();
        if (member.Profile.CanTrainLevel())
            Experience.color = Color.green;
        else
            Experience.color = Color.white;

        Attack.text = (member.Vitals.EffectiveAttack >= 0 ? "+ " : "- ") + Mathf.Abs(member.Vitals.EffectiveAttack);
        string label = member.Vitals.EffectiveDamageLower.ToString();
        if(member.Vitals.EffectiveDamageLower != member.Vitals.EffectiveDamageUpper)
            label += " - " + member.Vitals.EffectiveDamageUpper;
        Damage.text = label;
        Shoot.text = (member.Vitals.EffectiveRangedAttack >= 0 ? "+ " : "- ") + Mathf.Abs(member.Vitals.EffectiveRangedAttack);
        if(!member.Equipment.HasRangedWeapon())
        {
            label = "N/A";
        } 
        else
        {
            label = member.Vitals.EffectiveRangedDamageLower.ToString();
            if (member.Vitals.EffectiveRangedDamageLower != member.Vitals.EffectiveRangedDamageUpper)
                label += " - " + member.Vitals.EffectiveRangedDamageUpper;
        }
        ShootDamage.text = label;

        Fire.text = Label(member.Profile.Resistances.Fire);
        Electricity.text = Label(member.Profile.Resistances.Electricity);
        Cold.text = Label(member.Profile.Resistances.Cold);
        Poison.text = Label(member.Profile.Resistances.Poison);
        Magic.text = Label(member.Profile.Resistances.Magic);
    }

    string Label(int current)
    {
        return Label(current, current);
    }

    string Label(int current, int normal)
    {
        string color = "white";
        if (current < normal)
            color = "red";
        else if (current > normal)
            color = "green";

        string colorLabel = "<color=" + color + ">";

        return colorLabel + current + "</color> / " + normal;
    }
}
