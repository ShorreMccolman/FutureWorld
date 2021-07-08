using System.Collections;
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
    
    public bool AwaitingTurn { get; private set; }

    public PartyMember(GameStateEntity parent, CharacterData data) : base(parent)
    {
        Status = new Status(this);
        Equipment = new Equipment(this, data);
        Inventory = new Inventory(this, data, Equipment);
        Skillset = new Skillset(this, data);
        SpellLog = new SpellLog(this, Skillset, data);
        History = new History(this, data);
        Profile = new Profile(this, Status, Equipment, Skillset, data);
        Vitals = new Vitals(this, data, Status, Profile, Equipment, Skillset);
        TurnController.OnTurnBasedToggled += ToggleCombat;
    }

    public PartyMember(GameStateEntity parent, XmlNode node) : base(parent, node)
    {
        Status = new Status(this, node);
        Inventory = new Inventory(this, node);
        Equipment = new Equipment(this, node);
        Skillset = new Skillset(this, node);
        SpellLog = new SpellLog(this, Skillset, node);
        History = new History(this, node);
        Profile = new Profile(this, Status, Equipment, Skillset, node);
        Vitals = new Vitals(this, node, Status, Profile, Equipment, Skillset);
        TurnController.OnTurnBasedToggled += ToggleCombat;
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
            case PartyMemberState.Concious:
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

    public bool IsAlive()
    {
        if (Vitals.Condition != PartyMemberState.Concious)
            return false;

        if (Status.HasCondition(StatusEffectOption.Sleep))
            return false;

        return true;
    }

    public string GetName()
    {
        return Profile.CharacterName;
    }

    public void ToggleCombat(bool enable)
    {
        if (!IsAlive())
            return;

        if (enable)
            Vitals.ApplyCooldown(Vitals.Stats.EffectiveRecovery);
        else
            Vitals.ApplyCooldown(0.1f);
    }

    public void ActivateTurn()
    {
        Party.Instance.SetActiveMember(this);
        AwaitingTurn = true;
    }

    public bool WaitForTurn()
    {
        return AwaitingTurn;
    }

    public void FullHeal()
    {
        Status.HealConditions();
        Vitals.Restore(true);
    }

    public int PriceOfHealing()
    {
        int value = 0;
        if(Vitals.Condition == PartyMemberState.Eradicated)
        {
            value = 500;
        } 
        else if (Vitals.Condition == PartyMemberState.Dead)
        {
            value = 200;
        }
        else
        {
            if(Status.HasNegativeCondition())
            {
                value = 30;
            } 
            else if (Vitals.CurrentHP < Vitals.Stats.EffectiveTotalHP || Vitals.CurrentSP < Vitals.Stats.EffectiveTotalSP)
            {
                value = 10;
            }
        }
        return value;
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
        return Mathf.Max(0,Vitals.Cooldown);
    }

    public void Rest(float duration)
    {
        Vitals.Restore();
        if (duration > 0)
            Status.AddCondition(StatusEffectOption.Sleep, duration * 60);
        else
            Status.AddCondition(StatusEffectOption.Rested, GameConstants.REST_DURATION);
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
    public bool TryHit(Enemy enemy, out int damage)
    {
        bool hits = CombatHelper.ShouldHit(Vitals.Stats.EffectiveAttack, enemy.Data.ArmorClass);

        int dmg = 0;
        if(hits)
        {
            dmg = Equipment.RollDamage(Skillset);
            float chanceOfReduction = 1 - 30 / (float)(30 + enemy.Data.Resistances.Physical);
            dmg = CombatHelper.ReduceDamage(dmg, chanceOfReduction);
        }

        damage = Status.ModifyFinalDamage(dmg);

        if (hits)
        {
            string msg = Profile.CharacterName + " hit " + enemy.Data.DisplayName + " for " + damage + " damage.";
            if(enemy.CurrentHP - damage <= 0)
                msg = Profile.CharacterName + " inflicts " + damage + " damage killing " + enemy.Data.DisplayName + ".";
            InfoMessageReceiver.Send(msg, 2.0f);

            Vitals.Express(GameConstants.EXPRESSION_HAPPY, GameConstants.EXPRESSION_HAPPY_DURATION);
            SoundManager.Instance.PlayUISound("Hit");
        }
        else
        {
            Vitals.Express(GameConstants.EXPRESSION_UNSURE, GameConstants.EXPRESSION_UNSURE_DURATION);
            SoundManager.Instance.PlayUISound("Swing");
        }

        Vitals.ApplyCooldown(Vitals.Stats.EffectiveRecovery);
        AwaitingTurn = false;
        return hits;
    }

    public bool TryShoot(EnemyEntity target, out Projectile projectile)
    {
        bool result = false;
        projectile = null;

        if(Equipment.HasRangedWeapon())
        {
            projectile = ProjectileDatabase.Instance.GetProjectile("generic");
            projectile.SetDamage(Profile.CharacterName, Vitals.Stats.EffectiveRangedAttack, Equipment.RollRangedDamage(Skillset));

            Vitals.ApplyCooldown(Vitals.Stats.EffectiveRangedRecovery);
            Vitals.Express(GameConstants.EXPRESSION_UNSURE, GameConstants.EXPRESSION_UNSURE_DURATION);
            SoundManager.Instance.PlayUISound("Arrow");
        }
        else
        {
            Vitals.ApplyCooldown(Vitals.Stats.EffectiveRecovery);
            SoundManager.Instance.PlayUISound("Swing");
        }
        AwaitingTurn = false;
        return result;
    }

    public bool OnEnemyAttack(EnemyData enemy)
    {
        float chanceToHit = (5 + (float)enemy.Level * 2) / (10 + (float)enemy.Level * 2 + (float)Vitals.Stats.EffectiveArmorClass);
        float rand = Random.Range(0f, 1f);

        bool hits = rand <= chanceToHit;
        if (hits)
        {
            int damage = enemy.AttackData.DamageRoll.Roll();
            float chanceOfReduction = 1 - 30 / (30 + (Profile.Resistances.ResistanceForAttackType(enemy.AttackData.Type) + Profile.BaseResistance));
            damage = CombatHelper.ReduceDamage(damage, chanceOfReduction);

            Vitals.TakeDamage(damage);
            Vitals.Express(GameConstants.EXPRESSION_HIT, GameConstants.EXPRESSION_HIT_DURATION);
        }

        return hits;
    }

    public bool OnDamaged(AttackResult attack)
    {
        float chanceOfReduction = 1 - 30 / (30 + Profile.Resistances.ResistanceForAttackType(attack.Type) + Profile.BaseResistance);
        int damage = CombatHelper.ReduceDamage(attack.Value, chanceOfReduction);

        Vitals.TakeDamage(damage);
        Vitals.Express(GameConstants.EXPRESSION_HIT, GameConstants.EXPRESSION_HIT_DURATION);
        return true;
    }
}
