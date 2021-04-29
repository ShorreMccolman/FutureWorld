using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Xml;
using System.Xml.Serialization;


public class GameStateManager : MonoBehaviour {

    public static GameStateManager Instance { get; private set; }
    void Awake()
    { Instance = this; }

    List<GameStateEntity> _parentEntities;

    void Start()
    {
        Init();
    }

    void Init()
    {
        DontDestroyOnLoad(this.gameObject);
        _parentEntities = new List<GameStateEntity>();
    }

    public void RegisterParentEntity(GameStateEntity entity)
    {
        if(!_parentEntities.Contains(entity))
        {
            _parentEntities.Add(entity);
        }
    }

    public void Unregister(GameStateEntity entity)
    {
        if(_parentEntities.Contains(entity))
        {
            _parentEntities.Remove(entity);
        }
    }

    public void SaveGameState(int slot)
    {
        XmlDocument document = new XmlDocument();
        XmlElement root = document.CreateElement("GameData");
        foreach(var entity in _parentEntities)
        {
            root.AppendChild(entity.ToXml(document));
        }
        document.AppendChild(root);

        FileManager.SaveFile("GSED_" + slot, "xml", "SaveData", document.OuterXml);
    }

    public XmlDocument LoadGameState(int slot)
    {
        XmlDocument document = new XmlDocument();
        document.LoadXml(FileManager.ReadFile("GSED_" + slot, "xml", "SaveData"));

        return document;
    }
}
