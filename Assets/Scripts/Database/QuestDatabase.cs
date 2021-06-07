using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestLine
{
    UNASSIGNED = 0,
    NotQuest = 1,


    Letter = 100,
    Candelabra = 101,

    SpiderQueen = 102,
    MissingDaughter = 103,

    ShadowGuild = 104,
    Goblinwatch = 105
}

[System.Serializable]
public class QuestData
{
    public QuestLine ID;
    public string[] LogDescriptions;
    public string AwardDescription;
}

public class QuestDatabase
{
    public static QuestDatabase Instance;

    Dictionary<QuestLine, QuestData> _questDict;

    public QuestDatabase()
    {
        Instance = this;

        _questDict = new Dictionary<QuestLine, QuestData>();

        QuestDBObject[] questDBObjects = Resources.LoadAll<QuestDBObject>("Database");
        if (questDBObjects == null)
        {
            Debug.LogError("Failed to load any Quest DB");
            return;
        }

        foreach (var obj in questDBObjects)
        {
            foreach (var quest in obj.GetQuests())
            {
                _questDict.Add(quest.ID, quest);
            }
        }
    }

    public QuestData GetQuest(QuestLine ID)
    {
        if (!_questDict.ContainsKey(ID))
            return null;

        return _questDict[ID];
    }
}
