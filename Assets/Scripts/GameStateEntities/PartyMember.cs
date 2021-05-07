﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;

public class PartyMember : GameStateEntity, CombatEntity {

    public Profile Profile { get; private set; }
    public Inventory Inventory { get; private set; }
    public Equipment Equipment { get; private set; }
    public Skillset Skillset { get; private set; }
    public SpellLog SpellLog { get; private set; }
    public History History { get; private set; }
    public Status Status { get; private set; }
    public Vitals Vitals { get; private set; }

    public PartyMember(GameStateEntity parent, CharacterData data) : base(parent)
    {
        Status = new Status(this);
        Equipment = new Equipment(this, data);
        Inventory = new Inventory(this, data, Equipment);
        Skillset = new Skillset(this, data);
        SpellLog = new SpellLog(this, Skillset, data);
        History = new History(this, data);
        Profile = new Profile(this, Status, Equipment, data);
        Vitals = new Vitals(this, data, Status, Profile, Equipment, Skillset);
    }

    public PartyMember(GameStateEntity parent, XmlNode node) : base(parent, node)
    {
        Status = new Status(this, node);
        Inventory = new Inventory(this, node);
        Equipment = new Equipment(this, node);
        Skillset = new Skillset(this, node);
        SpellLog = new SpellLog(this, Skillset, node);
        History = new History(this, node);
        Profile = new Profile(this, Status, Equipment, node);
        Vitals = new Vitals(this, node, Status, Profile, Equipment, Skillset);
    }

    public override XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("PartyMember");
        element.AppendChild(Profile.ToXml(doc));
        element.AppendChild(Vitals.ToXml(doc));
        element.AppendChild(Inventory.ToXml(doc));
        element.AppendChild(Equipment.ToXml(doc));
        element.AppendChild(Skillset.ToXml(doc));
        element.AppendChild(SpellLog.ToXml(doc));
        element.AppendChild(Status.ToXml(doc));
        element.AppendChild(History.ToXml(doc));
        element.AppendChild(base.ToXml(doc));
        return element;
    }

    public string EffectiveStatusCondition()
    {
        switch(Vitals.Condition)
        {
            case PartyMemberState.Good:
                return Status.GetStatusCondition();
            case PartyMemberState.Unconcious:
                return "Unconcious";
            case PartyMemberState.Dead:
                return "Dead";
            case PartyMemberState.Eradicated:
                return "Eradicated";
        }
        return "";
    }

    public List<Skill> DetermineLearnableSkills(List<string> available)
    {
        List<Skill> skills = new List<Skill>();

        foreach(var skill in available)
        {
            if (GameConstants.InvalidSkillsByCharacterClass[Profile.Class].Contains(skill))
                continue;
            if (Skillset.KnowsSkill(skill))
                continue;

            skills.Add(SkillDatabase.Instance.GetSkill(skill));
        }

        return skills;
    }

    public float GetCooldown()
    {
        return Vitals.Cooldown;
    }

    public void Rest(float duration)
    {
        Vitals.Rest();
        Status.AddCondition(StatusEffectOption.Sleep, duration * 60);
        Status.RemoveCondition(StatusEffectOption.Weak);
    }

    public bool TryConsume(InventoryItem item)
    {
        Consumable consumable = item.Data as Consumable;
        if(consumable != null)
        {
            switch(consumable.ConsumeEffect)
            {
                case ConsumeEffect.RestoreHP:
                    Vitals.GainHealthPoints(consumable.Potency);
                    return true;
                case ConsumeEffect.RestoreMP:
                    Vitals.GainSpellPoints(consumable.Potency);
                    return true;
                case ConsumeEffect.BoostAC:
                    Status.AddCondition(StatusEffectOption.BoostedAC, 60.0f);
                    return true;
                case ConsumeEffect.BoostResistance:
                    Status.AddCondition(StatusEffectOption.BoostedResistance, 60.0f);
                    return true;
                case ConsumeEffect.BoostStats:
                    Status.AddCondition(StatusEffectOption.BoostedStats, 60.0f);
                    return true;
                case ConsumeEffect.CurePoison:
                    Status.RemoveCondition(StatusEffectOption.Poison);
                    return true;
                case ConsumeEffect.Poison:
                    Status.AddCondition(StatusEffectOption.Poison, 0);
                    return true;
                case ConsumeEffect.SkillPoints:
                    Profile.AddSkillPoints(consumable.Potency);
                    return true;
            }

            return false;
        }

        Magic spell = item.Data as Magic;
        if (spell != null)
        {
            if(Skillset.KnowsSkill(spell.Type.ToString()))
            {
                return SpellLog.LearnSpell(spell.Type, spell.SpellNumber);
            }

            return false;
        }

        return false;
    }

    public bool TryIdentify(InventoryItem item)
    {
        if (!Skillset.KnowsSkill("Identify"))
            return false;

        InventorySkill skill = Skillset.GetSkillByID("Identify");
        int level = skill.Level;
        if (skill.Proficiency == SkillProficiency.Expert)
            level *= 2;
        else if (skill.Proficiency == SkillProficiency.Master)
            level *= 3;

        return item.TryIdentify(level);
    }

    public bool TryRepair(InventoryItem item)
    {
        if (!Skillset.KnowsSkill("Repair"))
            return false;

        InventorySkill skill = Skillset.GetSkillByID("Repair");
        int level = skill.Level;
        if (skill.Proficiency == SkillProficiency.Expert)
            level *= 2;
        else if (skill.Proficiency == SkillProficiency.Master)
            level *= 3;

        return item.TryRepair(level);
    }

    public bool TryHit(Enemy enemy, out int damage)
    {
        bool hits = CombatHelper.ShouldHit(Vitals.EffectiveAttack, enemy.Data.ArmorClass);

        int dmg = 0;
        if(hits)
        {
            dmg = Equipment.RollDamage(Skillset);
            float chanceOfReduction = 1 - 30 / (30 + enemy.Data.Resistances.Physical);
            dmg = CombatHelper.ReduceDamage(dmg, chanceOfReduction);
        }

        damage = Status.ModifyFinalDamage(dmg);

        if (hits)
        {
            string msg = Profile.CharacterName + " hit " + enemy.Data.DisplayName + " for " + damage + " damage.";
            if(enemy.CurrentHP - damage <= 0)
                msg = Profile.CharacterName + " inflicts " + damage + " damage killing " + enemy.Data.DisplayName + ".";

            HUD.Instance.SendInfoMessage(msg, 2.0f);
            HUD.Instance.ExpressMember(this, GameConstants.EXPRESSION_HAPPY, GameConstants.EXPRESSION_HAPPY_DURATION);
        }
        else
        {
            HUD.Instance.ExpressMember(this, GameConstants.EXPRESSION_UNSURE, GameConstants.EXPRESSION_UNSURE_DURATION);
        }

        Vitals.ApplyCooldown(Vitals.Recovery);

        return hits;
    }

    public bool TryShoot(EnemyEntity target, out Projectile projectile)
    {
        bool result = false;
        projectile = null;

        if(Equipment.HasRangedWeapon())
        {
            projectile = ProjectileDatabase.Instance.GetProjectile("generic");
            projectile.SetDamage(Profile.CharacterName, Vitals.EffectiveRangedAttack, Equipment.RollRangedDamage(Skillset));

            Vitals.ApplyCooldown(Vitals.RangedRecovery);
            HUD.Instance.ExpressMember(this, GameConstants.EXPRESSION_UNSURE, GameConstants.EXPRESSION_UNSURE_DURATION);
        }
        else
        {
            Vitals.ApplyCooldown(Vitals.Recovery);
        }
        return result;
    }

    public bool OnEnemyAttack(EnemyData enemy)
    {
        float chanceToHit = (5 + (float)enemy.Level * 2) / (10 + (float)enemy.Level * 2 + (float)Vitals.EffectiveArmorClass);
        float rand = Random.Range(0f, 1f);

        bool hits = rand <= chanceToHit;
        if (hits)
        {
            int damage = enemy.AttackData.DamageRoll.Roll();
            float chanceOfReduction = 1 - 30 / (30 + Profile.EffectiveResistance(enemy.AttackData.Type));
            damage = CombatHelper.ReduceDamage(damage, chanceOfReduction);

            Vitals.TakeDamage(damage);
            //string msg = enemy.DisplayName + " hit " + Profile.CharacterName + " for " + damage + " damage.";
            //HUD.Instance.SendInfoMessage(msg, 2.0f);
            HUD.Instance.ExpressMember(this, GameConstants.EXPRESSION_HIT, GameConstants.EXPRESSION_HIT_DURATION);
        }

        return hits;
    }
}
