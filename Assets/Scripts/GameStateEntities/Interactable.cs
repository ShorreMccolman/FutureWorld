using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class Interactable : GameStateEntity
{
    int _timesUsed;

    public InteractableData Data { get; protected set; }

    public CharacterStat Stat { get; protected set; }

    public Interactable(InteractableData data) : base(null)
    {
        Data = data;
        _timesUsed = 0;
        if (data.Stat == CharacterStat.Random)
            Stat = (CharacterStat)Random.Range(0, 8);
        else
            Stat = data.Stat;
    }

    public Interactable(XmlNode node) : base(null, node)
    {
        Data = InteractableDatabase.Instance.GetInteractableData(node.SelectSingleNode("ID").InnerText);
        _timesUsed = int.Parse(node.SelectSingleNode("TimesUsed").InnerText);
        Stat = (CharacterStat)int.Parse(node.SelectSingleNode("Stat").InnerText);
    }

    public override XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("Interactable");
        element.AppendChild(XmlHelper.Attribute(doc, "ID", Data.ID));
        element.AppendChild(XmlHelper.Attribute(doc, "TimesUsed", _timesUsed));
        element.AppendChild(XmlHelper.Attribute(doc, "Stat", (int)Stat));
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
                        canUse = false;
                    else
                    {
                        member.Status.AddCondition(Data.Option, Data.Potency, Data.Duration * 60);
                        HUD.Instance.ExpressMember(member, GameConstants.EXPRESSION_HAPPY, GameConstants.EXPRESSION_HAPPY_DURATION);
                        HUD.Instance.SendInfoMessage(MessageForOption(Data.Option, Data.Potency), 2.0f);
                    }
                    break;
                case InteractableEffect.PermanentStat:
                    if (Stat == CharacterStat.None)
                        canUse = false;
                    else
                    {
                        member.Profile.AddStatPoints(Stat, Data.Potency);
                        HUD.Instance.ExpressMember(member, GameConstants.EXPRESSION_HAPPY, GameConstants.EXPRESSION_HAPPY_DURATION);
                        HUD.Instance.SendInfoMessage("+" + Data.Potency + " " + Stat.ToString() + " permanent.", 2.0f);
                        Stat = CharacterStat.None;
                    }
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