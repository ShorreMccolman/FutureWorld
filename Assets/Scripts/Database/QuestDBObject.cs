using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class QuestDBObject : ScriptableObject
{
    public QuestData[] Quests;
    public QuestData[] GetQuests() { return Quests; }
}