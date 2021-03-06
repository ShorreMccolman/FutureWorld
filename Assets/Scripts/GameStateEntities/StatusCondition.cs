using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;


public class StatusCondition : GameStateEntity
{
    public StatusEffectOption Option { get; private set; }
    public StatusEffect Effect { get; private set; }
    public float Lifetime { get; private set; }
    public float Duration { get; private set; }
    public int Potency { get; private set; }

    public event System.Action<StatusCondition> OnConditionComplete;

    public StatusCondition(GameStateEntity parent, StatusEffect data, float duration) : base(parent)
    {
        Option = data.Option;
        Effect = data;
        Duration = duration;
        Lifetime = 0;
        Potency = 0;

        TimeManagement.OnTick += Tick;
    }

    public StatusCondition(GameStateEntity parent, StatusEffect data, int potency, float duration) : base(parent)
    {
        Option = data.Option;
        Effect = data;
        Duration = duration;
        Potency = potency;

        TimeManagement.OnTick += Tick;
    }

    public StatusCondition(GameStateEntity parent, XmlNode node) : base(parent, node)
    {
        XmlNode skillNode = node.SelectSingleNode("StatusCondition");
        Option = (StatusEffectOption)int.Parse(node.SelectSingleNode("Option").InnerText);
        Duration = float.Parse(node.SelectSingleNode("Duration").InnerText);
        Potency = int.Parse(node.SelectSingleNode("Potency").InnerText);
        Effect = StatusEffectDatabase.Instance.GetEffect(Option);

        TimeManagement.OnTick += Tick;
    }

    public override XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("StatusCondition");
        element.AppendChild(XmlHelper.Attribute(doc, "Option", (int)Option));
        element.AppendChild(XmlHelper.Attribute(doc, "Duration", Duration));
        element.AppendChild(XmlHelper.Attribute(doc, "Potency", Potency));
        element.AppendChild(base.ToXml(doc));
        return element;
    }

    public void Tick(float delta)
    {
        Lifetime += delta;

        if(Effect.IsTemporary)
        {
            Duration -= delta;
            if (Duration < 0)
            {
                OnConditionComplete?.Invoke(this);
            }
        }
    }

    public void Terminate()
    {
        TimeManagement.OnTick -= Tick;
    }

    public void ModifyStats(EffectiveStats stats)
    {
        switch (Option)
        {
            case StatusEffectOption.Poison:
                stats.GetStat(CharacterStat.Might).ReduceMultiplier(0.75f);
                stats.GetStat(CharacterStat.Endurance).ReduceMultiplier(0.75f);
                stats.GetStat(CharacterStat.Accuracy).ReduceMultiplier(0.75f);
                stats.GetStat(CharacterStat.Speed).ReduceMultiplier(0.75f);
                break;
            case StatusEffectOption.Disease:
                stats.GetStat(CharacterStat.Might).ReduceMultiplier(0.6f);
                stats.GetStat(CharacterStat.Endurance).ReduceMultiplier(0.6f);
                stats.GetStat(CharacterStat.Accuracy).ReduceMultiplier(0.6f);
                stats.GetStat(CharacterStat.Speed).ReduceMultiplier(0.6f);
                break;

            case StatusEffectOption.BoostedResistance:
                stats.GetStat(CharacterStat.FireResist).AddToBonus(Potency);
                stats.GetStat(CharacterStat.ElecResist).AddToBonus(Potency);
                stats.GetStat(CharacterStat.ColdResist).AddToBonus(Potency);
                stats.GetStat(CharacterStat.PoisonResist).AddToBonus(Potency);
                stats.GetStat(CharacterStat.MagicResist).AddToBonus(Potency);
                break;
            case StatusEffectOption.ProtFire:
            case StatusEffectOption.BoostedFireR:
                stats.GetStat(CharacterStat.FireResist).AddToBonus(Potency);
                break;
            case StatusEffectOption.ProtElec:
            case StatusEffectOption.BoostedElecR:
                stats.GetStat(CharacterStat.ElecResist).AddToBonus(Potency);
                break;
            case StatusEffectOption.ProtCold:
            case StatusEffectOption.BoostedColdR:
                stats.GetStat(CharacterStat.ColdResist).AddToBonus(Potency);
                break;
            case StatusEffectOption.ProtPois:
            case StatusEffectOption.BoostedPoisR:
                stats.GetStat(CharacterStat.PoisonResist).AddToBonus(Potency);
                break;
            case StatusEffectOption.ProtMagic:
            case StatusEffectOption.BoostedMagicR:
                stats.GetStat(CharacterStat.MagicResist).AddToBonus(Potency);
                break;
            case StatusEffectOption.BoostedAC:
                stats.GetStat(CharacterStat.ArmorClass).AddToBonus(Potency);
                break;
            case StatusEffectOption.BoostedStats:
                stats.GetStat(CharacterStat.Might).AddToBonus(Potency);
                stats.GetStat(CharacterStat.Intellect).AddToBonus(Potency);
                stats.GetStat(CharacterStat.Personality).AddToBonus(Potency);
                stats.GetStat(CharacterStat.Endurance).AddToBonus(Potency);
                stats.GetStat(CharacterStat.Speed).AddToBonus(Potency);
                stats.GetStat(CharacterStat.Accuracy).AddToBonus(Potency);
                stats.GetStat(CharacterStat.Luck).AddToBonus(Potency);
                break;
            case StatusEffectOption.BoostedMight:
                stats.GetStat(CharacterStat.Might).AddToBonus(Potency);
                break;
            case StatusEffectOption.BoostedIntellect:
                stats.GetStat(CharacterStat.Intellect).AddToBonus(Potency);
                break;
            case StatusEffectOption.BoostedPersonality:
                stats.GetStat(CharacterStat.Personality).AddToBonus(Potency);
                break;
            case StatusEffectOption.BoostedEndurance:
                stats.GetStat(CharacterStat.Endurance).AddToBonus(Potency);
                break;
            case StatusEffectOption.BoostedSpeed:
                stats.GetStat(CharacterStat.Speed).AddToBonus(Potency);
                break;
            case StatusEffectOption.BoostedAccuracy:
                stats.GetStat(CharacterStat.Accuracy).AddToBonus(Potency);
                break;
            case StatusEffectOption.BoostedLuck:
                stats.GetStat(CharacterStat.Luck).AddToBonus(Potency);
                break;

            case StatusEffectOption.Bless:
                stats.GetStat(CharacterStat.RangedAttack).AddToBonus(Potency);
                stats.GetStat(CharacterStat.Attack).AddToBonus(Potency);
                break;
        }
    }
}
