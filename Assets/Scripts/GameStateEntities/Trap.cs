using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public struct AttackResult
{
    public int Value;
    public AttackType Type;

    public AttackResult(int val, AttackType type)
    {
        Value = val;
        Type = type;
    }
}

public class Trap : GameStateEntity
{
    int _level;
    AttackType _attackType;

    bool _isDisarmed;
    public bool IsDisarmed { get { return _isDisarmed; } }

    DiceRoll Roll;

    public Trap(Chest parent) : base(parent)
    {
        _level = parent.Data.TrapLevel;
        Roll = new DiceRoll(6, 3 + _level);
        if (_level == 0)
            _isDisarmed = true;

        int rand = Random.Range(0, 4);
        if (rand == 0)
        {
            _attackType = AttackType.Fire;
        }
        else if (rand == 1)
        {
            _attackType = AttackType.Cold;
        }
        else if (rand == 2)
        {
            _attackType = AttackType.Electricity;
        }
        else if (rand == 3)
        {
            _attackType = AttackType.Poison;
        }
    }

    public Trap(XmlNode node, GameStateEntity parent) : base(parent, node)
    {
        _level = int.Parse(node.SelectSingleNode("Level").InnerText);
        _attackType = (AttackType)int.Parse(node.SelectSingleNode("Type").InnerText);
        _isDisarmed = bool.Parse(node.SelectSingleNode("Disarmed").InnerText);
        Roll = new DiceRoll(6, 3 + _level);
    }

    public override XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("Trap");
        element.AppendChild(XmlHelper.Attribute(doc, "Level", _level));
        element.AppendChild(XmlHelper.Attribute(doc, "Type", (int)_attackType));
        element.AppendChild(XmlHelper.Attribute(doc, "Disarmed", _isDisarmed));
        element.AppendChild(base.ToXml(doc));
        return element;
    }

    public bool Disarm(int level, out AttackResult attack)
    {
        attack = new AttackResult(Roll.Roll(), _attackType);
        _isDisarmed = true;
        return level >= _level;
    }
}
