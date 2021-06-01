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
                case InteractableEffect.TemporaryStat:
                    break;
                case InteractableEffect.PermanentStat:
                    break;
            }

            HUD.Instance.UpdateDisplay();
        }
        
        return canUse;
    }
}