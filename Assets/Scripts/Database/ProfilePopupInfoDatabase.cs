using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProfilePopupInfo
{
    public string Tag;
    public string Title;
    public string Body;
    public string[] Extra;
}

public class ProfilePopupInfoDatabase
{
    public static ProfilePopupInfoDatabase Instance;

    Dictionary<string, ProfilePopupInfo> _infoDict;

    public ProfilePopupInfoDatabase()
    {
        Instance = this;

        _infoDict = new Dictionary<string, ProfilePopupInfo>();

        ProfilePopupInfoDBObject[] DBObjects = Resources.LoadAll<ProfilePopupInfoDBObject>("Database");
        if (DBObjects == null)
        {
            Debug.LogError("Failed to load any Profile popup info DB");
            return;
        }

        foreach (var obj in DBObjects)
        {
            foreach(var info in obj.Info)
            {
                _infoDict.Add(info.Tag, info);
            }
        }
    }

    public ProfilePopupInfo GetInfo(string tag)
    {
        if(!_infoDict.ContainsKey(tag))
        {
            Debug.LogError("Could not find info with tag " + tag);
            return null;
        }

        return _infoDict[tag];
    }
}
