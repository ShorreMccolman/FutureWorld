using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class SpriteHandler
{
    static Dictionary<string, Sprite> _spriteDict = new Dictionary<string, Sprite>();
    static Dictionary<string, Sprite[]> _tempDict = new Dictionary<string, Sprite[]>();

    public static Sprite FetchSprite(string location, string ID)
    {
        string path = location + "/" + ID;

        if (_spriteDict.ContainsKey(path))
        {
            return _spriteDict[path];
        }
        else
        {
            Sprite sprite = Resources.Load<Sprite>(path);
            if(sprite == null)
            {
                Debug.LogError("Could not find sprite at path " + path);
                return null;
            }

            _spriteDict.Add(path, sprite);
            return sprite;
        }
    }

    public static Sprite[] FetchTemp(string path)
    {
        if (_tempDict.ContainsKey(path))
        {
            return _tempDict[path];
        }
        else
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>(path);
            if (sprites == null)
            {
                Debug.LogError("Could not find sprites at path " + path);
                return null;
            }

            _tempDict.Add(path, sprites);
            return sprites;
        }
    }

    public static void PurgeTemp()
    {
        _tempDict.Clear();
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }
}
