using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyMoveType
{
    Short,
    Medium,
    Long
}

public enum EnemyAIType
{
    Normal,
    Wimp,
    Aggressive,
    Suicidal,
}

[System.Serializable]
public class EnemyCombatData
{
    public bool Flies;
    public EnemyMoveType MoveType;
    public EnemyAIType AIType;
    public int Speed;
    public int Recovery;
}

public enum EnemyRank
{
    Soldier,
    Commander,
    Captain
}

[System.Serializable]
public class EnemyData
{
    public string ID;
    public string DisplayName;
    public bool isFemale;

    public bool IsHostile;

    public int Level;
    public int HitPoints;
    public int ArmorClass;

    public int Experience;
    public DiceRoll Gold;

    public int DropLevel;
    [Range(0, 100)] public int DropRate;
    public string DropTag;

    public AttackData AttackData;
    public Resistances Resistances;
    public EnemyCombatData CombatData;

    public string Spell;

    public GameObject model;
}

[System.Serializable]
public struct EnemyFamilyData
{
    public string ID;

    public bool IsNPC;

    public EnemyData Soldier;
    public EnemyData Commander;
    public EnemyData Captain;

    public EnemyData EnemyForRank(EnemyRank rank)
    {
        switch (rank)
        {
            default:
            case EnemyRank.Soldier:
                return Soldier;
            case EnemyRank.Commander:
                return Commander;
            case EnemyRank.Captain:
                return Captain;
        }
    }
}

public class EnemyDatabase
{
    public static EnemyDatabase Instance;

    Dictionary<string, EnemyFamilyData> _familyDict;
    Dictionary<string, EnemyData> _npcDict;

    public EnemyDatabase()
    {
        Instance = this;

        _familyDict = new Dictionary<string, EnemyFamilyData>();
        _npcDict = new Dictionary<string, EnemyData>();

        EnemyDBObject[] DBObjects = Resources.LoadAll<EnemyDBObject>("Database/Enemies");
        if (DBObjects.Length == 0)
        {
            Debug.LogError("Failed to load EnemyDB");
            return;
        }

        foreach (var db in DBObjects)
        {
            if (db.Family.IsNPC)
            {
                _npcDict.Add(db.Family.Soldier.ID, db.Family.Soldier);
                _npcDict.Add(db.Family.Commander.ID, db.Family.Commander);
                _npcDict.Add(db.Family.Captain.ID, db.Family.Captain);
            }
            else
                _familyDict.Add(db.Family.ID, db.Family);
        }
    }

    public EnemyData GetNPCEnemyData()
    {
        int rand = Random.Range(0, _npcDict.Count);

        int count = 0;
        foreach (var value in _npcDict.Values)
        {
            if (count == rand)
            {
                return value;
            }
            count++;
        }

        return null;
    }

    public Enemy GetRandomEnemy()
    {
        int rand = Random.Range(0, _familyDict.Count);
        int rand0 = Random.Range(0, 3);

        int count = 0;
        foreach(var value in _familyDict.Values)
        {
            if (count == rand)
            { 
                return new Enemy(value.EnemyForRank((EnemyRank)rand0));
            }
            count++;
        }

        return null;
    }

    public EnemyData GetEnemyData(string enemyID)
    {
        foreach(var fam in _familyDict.Values)
        {
            if (fam.Soldier.ID == enemyID)
                return fam.Soldier;
            if (fam.Captain.ID == enemyID)
                return fam.Captain;
            if (fam.Commander.ID == enemyID)
                return fam.Commander;
        }

        foreach(var npc in _npcDict.Values)
        {
            if (npc.ID == enemyID)
                return npc;
        }

        Debug.LogError("Could not find enemy with ID " + enemyID);
        return null;
    }

    public Enemy GetEnemy(string familyID, EnemyRank rank)
    {
        if(!_familyDict.ContainsKey(familyID))
        {
            Debug.LogError("Could not find family with ID " + familyID);
            return null;
        }

        EnemyFamilyData family = _familyDict[familyID];
        switch(rank)
        {
            default:
            case EnemyRank.Soldier:
                return new Enemy(family.Soldier);
            case EnemyRank.Captain:
                return new Enemy(family.Captain);
            case EnemyRank.Commander:
                return new Enemy(family.Commander);
        }
    }

    public EnemyFamilyData GetFamily(string familyID)
    {
        if (!_familyDict.ContainsKey(familyID))
        {
            Debug.LogError("Could not find family with ID " + familyID);
            return new EnemyFamilyData();
        }

        return _familyDict[familyID];
    }
}
