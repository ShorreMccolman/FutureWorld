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
        Name.text = member.Profile.CharacterName + " the " + member.Profile.Class.ToString();
        Skillpoints.text = "Skill Points: " + member.Profile.SkillPoints;

        Might.text = Label(member.Profile.EffectiveMight);
        Intellect.text = Label(member.Profile.EffectiveIntellect);
        Personality.text = Label(member.Profile.EffectivePersonality);
        Endurance.text = Label(member.Profile.EffectiveEndurance);
        Accuracy.text = Label(member.Profile.EffectiveAccuracy);
        Speed.text = Label(member.Profile.EffectiveSpeed);
        Luck.text = Label(member.Profile.EffectiveLuck);

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
        string damage = member.Vitals.EffectiveDamageLower.ToString();
        if(member.Vitals.EffectiveDamageLower != member.Vitals.EffectiveDamageUpper)
            damage += " - " + member.Vitals.EffectiveDamageUpper;
        Damage.text = damage;
        Shoot.text = (member.Vitals.EffectiveRangedAttack >= 0 ? "+ " : "- ") + Mathf.Abs(member.Vitals.EffectiveRangedAttack);
        damage = member.Vitals.EffectiveRangedDamageLower.ToString();
        if (member.Vitals.EffectiveRangedDamageLower != member.Vitals.EffectiveRangedDamageUpper)
            damage += " - " + member.Vitals.EffectiveRangedDamageUpper;
        ShootDamage.text = damage;

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
        return current + " / " + normal;
    }
}
