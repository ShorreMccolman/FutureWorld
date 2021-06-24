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

        Might.text = Label(member.Profile.Stats.EffectiveMight, member.Profile.Stats.Might);
        Intellect.text = Label(member.Profile.Stats.EffectiveIntellect, member.Profile.Stats.Intellect);
        Personality.text = Label(member.Profile.Stats.EffectivePersonality, member.Profile.Stats.Personality);
        Endurance.text = Label(member.Profile.Stats.EffectiveEndurance, member.Profile.Stats.Endurance);
        Accuracy.text = Label(member.Profile.Stats.EffectiveAccuracy, member.Profile.Stats.Accuracy);
        Speed.text = Label(member.Profile.Stats.EffectiveSpeed, member.Profile.Stats.Speed);
        Luck.text = Label(member.Profile.Stats.EffectiveLuck, member.Profile.Stats.Luck);

        Hitpoints.text = Label(member.Vitals.CurrentHP,member.Vitals.Stats.EffectiveTotalHP);
        Spellpoints.text = Label(member.Vitals.CurrentSP,member.Vitals.Stats.EffectiveTotalSP);
        Armorclass.text = Label(member.Vitals.Stats.EffectiveArmorClass, member.Vitals.Stats.ArmorClass);

        Condition.text = member.EffectiveStatusCondition();
        QuickSpell.text = member.Profile.QuickSpell;

        Age.text = Label(member.Profile.Age);
        Level.text = Label(member.Profile.Level);
        Experience.text = member.Profile.Experience.ToString();
        if (member.Profile.CanTrainLevel())
            Experience.color = Color.green;
        else
            Experience.color = Color.white;

        Attack.text = (member.Vitals.Stats.EffectiveAttack >= 0 ? "+ " : "- ") + Mathf.Abs(member.Vitals.Stats.EffectiveAttack);
        string label = member.Vitals.Stats.EffectiveDamageLower.ToString();
        if(member.Vitals.Stats.EffectiveDamageLower != member.Vitals.Stats.EffectiveDamageUpper)
            label += " - " + member.Vitals.Stats.EffectiveDamageUpper;
        Damage.text = label;
        Shoot.text = (member.Vitals.Stats.EffectiveRangedAttack >= 0 ? "+ " : "- ") + Mathf.Abs(member.Vitals.Stats.EffectiveRangedAttack);
        if(!member.Equipment.HasRangedWeapon())
        {
            label = "N/A";
        } 
        else
        {
            label = member.Vitals.Stats.EffectiveRangedDamageLower.ToString();
            if (member.Vitals.Stats.EffectiveRangedDamageLower != member.Vitals.Stats.EffectiveRangedDamageUpper)
                label += " - " + member.Vitals.Stats.EffectiveRangedDamageUpper;
        }
        ShootDamage.text = label;

        Fire.text = Label(member.Profile.Resistances.EffectiveFire, member.Profile.Resistances.Fire);
        Electricity.text = Label(member.Profile.Resistances.EffectiveElec, member.Profile.Resistances.Elec);
        Cold.text = Label(member.Profile.Resistances.EffectiveCold, member.Profile.Resistances.Cold);
        Poison.text = Label(member.Profile.Resistances.EffectivePoison, member.Profile.Resistances.Poison);
        Magic.text = Label(member.Profile.Resistances.EffectiveMagic, member.Profile.Resistances.Magic);
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
