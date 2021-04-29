using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ProjectileDBObject : ScriptableObject
{
    [SerializeField] ProjectileData[] Projectiles;
    public ProjectileData[] GetProjectiles() { return Projectiles; }
}
