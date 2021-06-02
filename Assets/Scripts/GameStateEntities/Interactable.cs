using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class Interactable : GameStateEntity
{
    int _timesUsed;

    public InteractableData Data { get; protected set; }

    public Interactable(InteractableData data) : base(null)
    {
        Data = data;
        _timesUsed = 0;
    }

    public Interactable(XmlNode node) : base(null, node)
    {
        Data = InteractableDatabase.Instance.GetInteractableData(node.SelectSingleNode("ID").InnerText);
        _timesUsed = int.Parse(node.SelectSingleNode("TimesUsed").InnerText);
    }

    public override XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("Interactable");
        element.AppendChild(XmlHelper.Attribute(doc, "ID", Data.ID));
        element.AppendChild(XmlHelper.Attribute(doc, "TimesUsed", _timesUsed));
        element.AppendChild(base.ToXml(doc));
        return element;
    }


    public bool TryInteraction(PartyMember member)
    {
        float update;
        bool refresh = TimeManagement.Instance.ShouldRefresh(Data.Period, LastUpdate, out update);
        LastUpdate = update;
        if (refresh)
            _timesUsed = 0;

        bool canUse = Data.NumberOfUses == 0 || _timesUsed < Data.NumberOfUses;

        if(canUse)
        {
            _timesUsed++;
            switch(Data.Effect)
            {
                case InteractableEffect.Charity:
                    break;
                case InteractableEffect.RestoreHP:
                    member.Vitals.GainHealthPoints(Data.Potency);
                    HUD.Instance.SendInfoMessage("+" + Data.Potency + " Hit points restored.", 2.0f);
                    break;
                case InteractableEffect.RestoreMP:
                    member.Vitals.GainSpellPoints(Data.Potency);
                    HUD.Instance.SendInfoMessage("+" + Data.Potency + " Spell points restored.", 2.0f);
                    break;
                case InteractableEffect.StatusEffect:
                    if (member.Status.HasCondition(Data.Option))
                    {
                        canUse = false;
                    }
                    else
                    {
                        member.Status.AddCondition(Data.Option, Data.Potency, Data.Duration * 60);
                        HUD.Instance.ExpressMember(member, GameConstants.EXPRESSION_HAPPY, GameConstants.EXPRESSION_HAPPY_DURATION);
                        HUD.Instance.SendInfoMessage(MessageForOption(Data.Option, Data.Potency), 2.0f);
                    }
                    break;
                case InteractableEffect.PermanentStat:
                    member.Profile.AddStatPoints(Data.Stat, Data.Potency);
                    HUD.Instance.ExpressMember(member, GameConstants.EXPRESSION_HAPPY, GameConstants.EXPRESSION_HAPPY_DURATION);
                    HUD.Instance.SendInfoMessage("+" + Data.Potency + " " + Data.Stat.ToString() + " permanent.", 2.0f);
                    break;
            }

            HUD.Instance.UpdateDisplay();
        }
        
        return canUse;
    }

    public string MessageForOption(StatusEffectOption option, int potency)
    {
        string result = "";
        switch (option)
        {
            case StatusEffectOption.BoostedMight:
                result = "+" + potency + " Might temporary.";
                break;
            case StatusEffectOption.BoostedEndurance:
                result = "+" + potency + " Endurance temporary.";
                break;
            case StatusEffectOption.BoostedAccuracy:
                result = "+" + potency + " Accuracy temporary.";
                break;
            case StatusEffectOption.BoostedSpeed:
                result = "+" + potency + " Speed temporary.";
                break;
            case StatusEffectOption.BoostedIntellect:
                result = "+" + potency + " Intellect temporary.";
                break;
            case StatusEffectOption.BoostedPersonality:
                result = "+" + potency + " Personality temporary.";
                break;
            case StatusEffectOption.BoostedLuck:
                result = "+" + potency + " Luck temporary.";
                break;
        }
        return result;
    }
}