using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;


public class Residency : GameStateEntity
{
    public string DisplayName { get { return _data.DisplayName; } }

    private List<Resident> _residents;
    public List<Resident> Residents { get { return _residents; } }

    ResidencyDBObject _data;
    public ResidencyDBObject Data { get { return _data; } }

    public Residency(ResidencyDBObject dbObject) : base(null)
    {
        _data = dbObject;

        _residents = new List<Resident>();
        foreach(var res in dbObject.Residents)
        {
            Resident resident = new Resident(this, res);
            Residents.Add(resident);
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
        element.AppendChild(XmlHelper.Attribute(doc, "Residents", Residents));
        element.AppendChild(base.ToXml(doc));

        return element;
    }
}
