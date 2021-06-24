using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class Enchantment : GameStateEntity
{
    public string ID { get; private set; }
    public EnchantmentData Data { get; private set; }

    List<int> _effectStrengths;

    public string SpecialText
    {
        get
        {
            if (!Data.Description.Contains("*"))
                return Data.Description;

            string num = _effectStrengths[0].ToString();
            return Data.Description.Replace("*", num);
        }
    }

    public Enchantment(GameStateEntity parent, TreasureLevel level, EnchantmentData data) : base(parent)
    {
        ID = data.ID;
        Data = data;

        if(data.Type == EnchantmentType.Standard)
        {
            int[] range = GameConstants.StandardEnchantmentRangeForItemlevel[level];
            _effectStrengths = new List<int>() { Random.Range(range[0], range[1] + 1) };
        }
        else
        {
            _effectStrengths = new List<int>();
            foreach(var effect in data.Effects)
            {
                if(effect.StrengthRange.Length > 0)
                    _effectStrengths.AddRange(effect.StrengthRange);
            }
        }
    }

    public Enchantment(GameStateEntity parent, XmlNode node) : base(parent, node)
    {
        ID = node.SelectSingleNode("ID").InnerText;

        _effectStrengths = new List<int>();
        XmlNodeList itemListNode = node.SelectNodes("Strength");
        for (int i = 0; i < itemListNode.Count; i++)
        {
            _effectStrengths.Add(int.Parse(itemListNode.Item(i).InnerText));
        }
        Data = ItemDatabase.Instance.EnchantDB.GetEnchantmentData(ID);
    }

    public override XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("Enchantment");
        element.AppendChild(XmlHelper.Attribute(doc, "ID", ID));
        foreach (var effect in _effectStrengths)
        {
            element.AppendChild(XmlHelper.Attribute(doc, "Strength", effect));
        }
        element.AppendChild(base.ToXml(doc));
        return element;
    }

    public int StrengthOfOption(EnchantmentEffectOption option)
    {
        for(int i=0;i<Data.Effects.Count;i++)
        {
            if(Data.Effects[i].Option == option)
            {
                return _effectStrengths[i];
            }
        }
        return 0;
    }

    public int ModifiedValue(int value)
    {
        int newValue = value;
        if (Data.Type == EnchantmentType.Standard)
        {
            newValue += 100 * _effectStrengths[0];
        }
        else
        {
            if (Data.ValueMulitplier != 0)
                newValue *= Data.ValueMulitplier;
            else
                newValue += Data.ValueBonus;
        }
        return newValue;
    }

    public void ModifyStats(EffectiveStats stats)
    {
        foreach(var pair in Data.Effects)
        {
            switch (pair.Option)
            {
                case EnchantmentEffectOption.AllResistances:
                    stats.GetStat(CharacterStat.FireResist).AddToBonus(StrengthOfOption(pair.Option));
                    stats.GetStat(CharacterStat.ElecResist).AddToBonus(StrengthOfOption(pair.Option));
                    stats.GetStat(CharacterStat.ColdResist).AddToBonus(StrengthOfOption(pair.Option));
                    stats.GetStat(CharacterStat.PoisonResist).AddToBonus(StrengthOfOption(pair.Option));
                    stats.GetStat(CharacterStat.MagicResist).AddToBonus(StrengthOfOption(pair.Option));
                    break;
                case EnchantmentEffectOption.AllStats:
                    stats.GetStat(CharacterStat.Might).AddToBonus(StrengthOfOption(pair.Option));
                    stats.GetStat(CharacterStat.Intellect).AddToBonus(StrengthOfOption(pair.Option));
                    stats.GetStat(CharacterStat.Personality).AddToBonus(StrengthOfOption(pair.Option));
                    stats.GetStat(CharacterStat.Endurance).AddToBonus(StrengthOfOption(pair.Option));
                    stats.GetStat(CharacterStat.Speed).AddToBonus(StrengthOfOption(pair.Option));
                    stats.GetStat(CharacterStat.Accuracy).AddToBonus(StrengthOfOption(pair.Option));
                    stats.GetStat(CharacterStat.Luck).AddToBonus(StrengthOfOption(pair.Option));
                    break;
                case EnchantmentEffectOption.BonusHP:
                    stats.GetStat(CharacterStat.TotalHP).AddToBonus(StrengthOfOption(pair.Option));
                    break;
                case EnchantmentEffectOption.BonusSP:
                    stats.GetStat(CharacterStat.TotalSP).AddToBonus(StrengthOfOption(pair.Option));
                    break;
                case EnchantmentEffectOption.BonusMight:
                    stats.GetStat(CharacterStat.Might).AddToBonus(StrengthOfOption(pair.Option));
                    break;
                case EnchantmentEffectOption.BonusEndurance:
                    stats.GetStat(CharacterStat.Endurance).AddToBonus(StrengthOfOption(pair.Option));
                    break;
                case EnchantmentEffectOption.BonusSpeed:
                    stats.GetStat(CharacterStat.Speed).AddToBonus(StrengthOfOption(pair.Option));
                    break;
                case EnchantmentEffectOption.BonusAccuracy:
                    stats.GetStat(CharacterStat.Accuracy).AddToBonus(StrengthOfOption(pair.Option));
                    break;
                case EnchantmentEffectOption.BonusPersonality:
                    stats.GetStat(CharacterStat.Personality).AddToBonus(StrengthOfOption(pair.Option));
                    break;
                case EnchantmentEffectOption.BonusIntellect:
                    stats.GetStat(CharacterStat.Intellect).AddToBonus(StrengthOfOption(pair.Option));
                    break;
                case EnchantmentEffectOption.BonusLuck:
                    stats.GetStat(CharacterStat.Luck).AddToBonus(StrengthOfOption(pair.Option));
                    break;
                case EnchantmentEffectOption.BonusAC:
                    stats.GetStat(CharacterStat.ArmorClass).AddToBonus(StrengthOfOption(pair.Option));
                    break;
                case EnchantmentEffectOption.ResistanceFire:
                    stats.GetStat(CharacterStat.FireResist).AddToBonus(StrengthOfOption(pair.Option));
                    break;
                case EnchantmentEffectOption.ResistanceCold:
                    stats.GetStat(CharacterStat.ColdResist).AddToBonus(StrengthOfOption(pair.Option));
                    break;
                case EnchantmentEffectOption.ResistanceElec:
                    stats.GetStat(CharacterStat.ElecResist).AddToBonus(StrengthOfOption(pair.Option));
                    break;
                case EnchantmentEffectOption.ResistancePoison:
                    stats.GetStat(CharacterStat.PoisonResist).AddToBonus(StrengthOfOption(pair.Option));
                    break;
            }
        }
    }
}
