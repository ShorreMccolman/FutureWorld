using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RefreshPeriod
{
    Daily,
    Even,
    Weekly,
    Monthly,
    Yearly,
    YearToDate,
    Permanent
}

[System.Serializable]
public class InteractableData
{
    public string ID;
    public string Mouseover;
    public GameObject Prefab;

    public int Potency;
    public int NumberOfUses;

    public InteractableEffect Effect;
    public CharacterStat Stat;
    public RefreshPeriod Period;
}

public class InteractableDatabase : MonoBehaviour
{
    public static InteractableDatabase Instance;

    Dictionary<string, InteractableData> _interactDict;

    public InteractableDatabase()
    {
        Instance = this;

        _interactDict = new Dictionary<string, InteractableData>();

        InteractableDBObject[] DBObjects = Resources.LoadAll<InteractableDBObject>("Database/Interactables");
        if (DBObjects.Length == 0)
        {
            Debug.LogError("Failed to load InteractableDB");
            return;
        }

        foreach (var db in DBObjects)
        {
            _interactDict.Add(db.Data.ID, db.Data);
        }
    }

    public InteractableData GetInteractableData(string interactID)
    {
        if(!_interactDict.ContainsKey(interactID))
        {
            Debug.LogError("Could not find interactable with ID: " + interactID);
            return null;
        }

        return _interactDict[interactID];
    }
}
