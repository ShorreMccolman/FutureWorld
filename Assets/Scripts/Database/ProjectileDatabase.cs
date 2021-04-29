﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProjectileData
{
    public string ID;
    public float Speed;
    public GameObject model;
}

public class ProjectileDatabase : MonoBehaviour
{
    public static ProjectileDatabase Instance;

    Dictionary<string, ProjectileData> _projDict;

    public ProjectileDatabase()
    {
        Instance = this;

        _projDict = new Dictionary<string, ProjectileData>();

        ProjectileDBObject[] DBObjects = Resources.LoadAll<ProjectileDBObject>("Database");
        if (DBObjects.Length == 0)
        {
            Debug.LogError("Failed to load ProjectileDB");
            return;
        }

        foreach (var db in DBObjects)
        {
            foreach(var proj in db.GetProjectiles())
                _projDict.Add(proj.ID, proj);
        }
    }

    public Projectile GetProjectile(string id)
    {
        if (!_projDict.ContainsKey(id))
        {
            Debug.LogError("Could not find projectile with ID " + id);
            return null;
        }

        return new Projectile( _projDict[id] );
    }
}
