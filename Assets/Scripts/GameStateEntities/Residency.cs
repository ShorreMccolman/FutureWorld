using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;


public class Residency : GameStateEntity
{
    public string DisplayName => _data.DisplayName;

    private List<Resident> _residents;
    public List<Resident> Residents => _residents;

    ResidencyDBObject _data;

    public static event System.Action<Residency> OnResidenceEntered;

    public Residency(ResidencyDBObject dbObject) : base(null)
    {
        _data = dbObject;

        _residents = new List<Resident>();
        foreach(var res in dbObject.Residents)
        {
            Resident resident = new Resident(this, res);
            _residents.Add(resident);
        }
    }

    public Residency(XmlNode node) : base(null, node)
    {
        _data = ResidencyDatabase.Instance.GetResidency(node.SelectSingleNode("ID").InnerText);
        Populate<Resident>(ref _residents, typeof(Residency), node, "Residents", "Resident");

        for(int i=0;i<_residents.Count;i++)
        {
            _residents[i].RestoreDataAfterLoad(_data.Residents[i]);
        }
    }

    public override XmlNode ToXml(XmlDocument doc)
    {
        XmlNode element = doc.CreateElement("Residency");
        element.AppendChild(XmlHelper.Attribute(doc, "ID", _data.ID));
        element.AppendChild(XmlHelper.Attribute(doc, "Residents", _residents));
        element.AppendChild(base.ToXml(doc));

        return element;
    }

    public void TryEnterResidence()
    {
        if (_data.Hours.IsStoreOpen(TimeManagement.Instance.GetCurrentHour()))
        {
            SoundManager.Instance.PlayUISound("Open");
            OnResidenceEntered?.Invoke(this);
        }
        else
        {
            InfoMessageReceiver.Send("This place is open from " + _data.Hours.GetStoreHoursString(), 2.0f);
            Party.Instance.ActiveMember.Vitals.Express(GameConstants.EXPRESSION_SAD, GameConstants.EXPRESSION_SAD_DURATION);
        }
    }
}
