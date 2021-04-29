using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResidencyDatabase
{
    public static ResidencyDatabase Instance;

    Dictionary<string, ResidencyDBObject> _residencyDict;

    public ResidencyDatabase()
    {
        Instance = this;

        _residencyDict = new Dictionary<string, ResidencyDBObject>();

        ResidencyDBObject[] resDBObjects = Resources.LoadAll<ResidencyDBObject>("Database/Residencies");
        if (resDBObjects.Length == 0)
        {
            Debug.LogError("Failed to load ResidentDB");
            return;
        }

        foreach (var db in resDBObjects)
        {
            _residencyDict.Add(db.ID, db);
        }
    }

    public ResidencyDBObject GetResidency(string ID)
    {
        if (!_residencyDict.ContainsKey(ID))
            return null;

        return _residencyDict[ID];
    }
}
