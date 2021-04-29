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
    public string DisplayName;

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

public class EnemyDatabase
{
    public static EnemyDatabase Instance;

    Dictionary<string, EnemyFamilyData> _familyDict;

    public EnemyDatabase()
    {
        Instance = this;

        _familyDict = new Dictionary<string, EnemyFamilyData>();

        EnemyDBObject[] DBObjects = Resources.LoadAll<EnemyDBObject>("Database/Enemies");
        if (DBObjects.Length == 0)
        {
            Debug.LogError("Failed to load EnemyDB");
            return;
        }

        foreach (var db in DBObjects)
        {
            _familyDict.Add(db.Family.ID, db.Family);
        }
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
