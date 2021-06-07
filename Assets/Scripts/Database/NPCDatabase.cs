using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Profession
{
    Smith,
    Armorer,
    Alchemist,
    Scholar,
    Guide,
    Tracker,
    Pathfinder,
    Sailor,
    Navigator,
    Healer
}

[System.Serializable]
public struct NPCData
{
    public string ID;
    public string ProfessionName;
    public Profession Profession;

    public int Weight;
    public int Cost;
    public int Rate;

    public string ActionText;
    public string JoinText;

    public static NPCData Default()
    {
        return new NPCData();
    }
}

public class NPCDatabase
{
    public static NPCDatabase Instance;

    Dictionary<string, NPCData> _npcDict;

    public NPCDatabase()
    {
        Instance = this;

        _npcDict = new Dictionary<string, NPCData>();

        NPCDBObject[] DBObjects = Resources.LoadAll<NPCDBObject>("Database");
        if (DBObjects.Length == 0)
        {
            Debug.LogError("Failed to load NPC DB");
            return;
        }

        foreach (var db in DBObjects)
        {
            foreach(var npc in db.NPCs)
                _npcDict.Add(npc.ID, npc);
        }
    }

    public NPCData GetNPCData(string ID)
    {
        if (!_npcDict.ContainsKey(ID))
            return NPCData.Default();

        return _npcDict[ID];
    }

    public void CreateRandomNPC(Enemy enemy)
    {
        int max = 0;
        foreach (var npc in _npcDict.Values)
        {
            max += npc.Weight;
        }

        int roll = Random.Range(0, max);
        int current = 0;
        foreach (var data in _npcDict.Values)
        {
            int chance = data.Weight;
            if (roll >= current && roll < current + chance)
            {
                enemy.InitNPC(data);
                return;
            }

            current += chance;
        }
    }
}
