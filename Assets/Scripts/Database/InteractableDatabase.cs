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

public enum InteractableType
{
    Fountain,
    Barrel,
    Well
}

[System.Serializable]
public class InteractableData
{
    public string ID;
    public GameObject Prefab;

    public int Potency;
    public float Duration;
    public int NumberOfUses;

    public InteractableEffect Effect;
    public StatusEffectOption Option;
    public CharacterStat Stat;
    public InteractableType Type;
    public RefreshPeriod Period;
}

public class InteractableDatabase
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
