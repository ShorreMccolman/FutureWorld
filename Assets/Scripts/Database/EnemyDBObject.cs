using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EnemyFamilyData
{
    public string ID;

    public EnemyData Soldier;
    public EnemyData Commander;
    public EnemyData Captain;

    public EnemyData EnemyForRank(EnemyRank rank)
    {
        switch(rank)
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

[CreateAssetMenu]
public class EnemyDBObject : ScriptableObject
{
    public EnemyFamilyData Family;
}
