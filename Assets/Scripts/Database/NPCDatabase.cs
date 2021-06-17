using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Profession
{
    Smith = 1,
    Armorer = 2,
    Alchemist = 3,
    Scholar = 4,
    Guide = 5,
    Tracker = 6,
    Pathfinder = 7,
    Sailor = 8,
    Navigator = 9,
    Healer = 10,
    ExpertHealer = 11,
    MasterHealer = 12,
    Teacher = 13,
    Instructor = 14,
    ArmsMaster = 15,
    WeaponsMaster = 16,
    Apprentice = 17,
    Mystic = 18,
    SpellMaster = 19,
    Trader = 20,
    Merchant = 21,
    Scout = 22,
    Counselor = 23,
    Barrister = 24,
    Tinker = 25,
    Locksmith = 26,
    Fool = 27,
    ChimneySweep = 28,
    Porter = 29,
    QuarterMaster = 30,

    Burglar = 51,
    Peasant = 52
}

[System.Serializable]
public class NPCData
{
    public string ID;
    public string ProfessionName;
    public Profession Profession;

    public int Weight;
    public int Cost;
    public int Rate;

    public string ActionText;
    public string JoinText;
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
            return null;

        return _npcDict[ID];
    }

    public Enemy CreateRandomNPC(EnemyData enemyData)
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
                return new Enemy(enemyData, data);
            }

            current += chance;
        }
        return null;
    }
}
