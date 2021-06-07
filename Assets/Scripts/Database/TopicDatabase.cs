using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TopicData
{
    public string Header;
    public string Body;

    public TopicData(string header, string body)
    {
        Header = header;
        Body = body;
    }

    public static TopicData Default()
    {
        return new TopicData("UNDEFINED", "Could not find topic");
    }
}

[System.Serializable]
public struct TopicSet
{
    public string ID;
    public bool IsWeekly;
    public TopicData[] Info;

    public TopicData GetDailyTopic(int dayOfWeek)
    {
        if (!IsWeekly || dayOfWeek > 6)
        {
            return GetRandomTopic();
        }
        else
        {
            return Info[dayOfWeek];
        }
    }

    public TopicData GetRandomTopic()
    {
        int rand = Random.Range(0, Info.Length);
        return Info[rand];
    }
}

public class TopicDatabase
{
    public static TopicDatabase Instance;

    Dictionary<string, TopicSet> _topicDict;

    public TopicDatabase()
    {
        Instance = this;

        _topicDict = new Dictionary<string, TopicSet>();

        TopicDBObject[] DBObjects = Resources.LoadAll<TopicDBObject>("Database");
        if (DBObjects.Length == 0)
        {
            Debug.LogError("Failed to load TopicDB");
            return;
        }

        foreach (var db in DBObjects)
        {
            foreach(var topic in db.Topics)
                _topicDict.Add(topic.ID, topic);
        }
    }

    public TopicData GetTopicData(string ID, int index)
    {
        if (_topicDict.ContainsKey(ID))
        {
            TopicSet set = _topicDict[ID];
            return _topicDict[ID].Info[index];
        }

        return TopicData.Default();
    }

    public TopicData GetTopic(string ID, int dayOfWeek = 0)
    {
        if(_topicDict.ContainsKey(ID))
        {
            TopicSet set = _topicDict[ID];
            if(set.IsWeekly)
            {
                return _topicDict[ID].Info[dayOfWeek];
            }

            int rand = Random.Range(0, _topicDict[ID].Info.Length);
            return _topicDict[ID].Info[rand];
        }

        return TopicData.Default();
    }

    public Topic GetNewsTopic(GameStateEntity parent)
    {
        string ID = "news";
        if (_topicDict.ContainsKey(ID))
        {
            TopicSet set = _topicDict[ID];
            int rand = Random.Range(0, set.Info.Length);
            return new Topic(set.Info[rand], ID, rand, parent);
        }

        return null;
    }

    public List<Topic> GetNewsTopics(int quantity, GameStateEntity parent)
    {
        List<Topic> topics = new List<Topic>();

        string ID = "news";
        if (_topicDict.ContainsKey(ID))
        {
            TopicSet set = _topicDict[ID];

            List<int> options = new List<int>();
            for(int i=0;i<set.Info.Length;i++)
            {
                options.Add(i);
            }

            for(int i=0;i<quantity;i++)
            {
                int rand = Random.Range(0, options.Count);
                topics.Add(new Topic(set.Info[options[rand]], ID, options[rand], parent));
                options.Remove(options[rand]);
            }
        }

        return topics;
    }
}
