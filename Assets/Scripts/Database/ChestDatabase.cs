using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChestData
{
    public string ID;
    public string MouseoverName;
    public SpawnQuantities Quantities;
    public int TrapLevel;
    public int LockLevel;
    public GameObject Prefab;
}

public class ChestDatabase
{
    public static ChestDatabase Instance;

    Dictionary<string, ChestData> _dict;

    public ChestDatabase()
    {
        Instance = this;

        _dict = new Dictionary<string, ChestData>();

        ChestDBObject[] DBObjects = Resources.LoadAll<ChestDBObject>("Database/Chests");
        if (DBObjects.Length == 0)
        {
            Debug.LogError("Failed to load Chest DB");
            return;
        }

        foreach (var db in DBObjects)
        {
            _dict.Add(db.Data.ID, db.Data);
        }
    }

    public ChestData GetChestData(string ID)
    {
        if (!_dict.ContainsKey(ID))
        {
            Debug.LogError("Could not find chest with ID " + ID);
            return null;
        }

        return _dict[ID];
    }
}
