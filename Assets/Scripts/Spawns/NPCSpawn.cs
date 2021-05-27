using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawn : Spawn
{
    [SerializeField] public float Range = 3.0f;

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(84f / 256f, 165f / 256f, 45f / 256f);
        Gizmos.DrawSphere(transform.position, Range);
    }

    public override void Populate()
    {
        EnemyData data = EnemyDatabase.Instance.GetNPCEnemyData("Peasant");
        DropController.Instance.SpawnNPC(data, transform.position);
    }
}
