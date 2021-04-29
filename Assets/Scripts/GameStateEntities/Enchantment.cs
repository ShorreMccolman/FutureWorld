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
}
