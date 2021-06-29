using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class EnemySpawn : Spawn
{
    [SerializeField] public float Range = 3.0f;
    [SerializeField] public string EnemyFamilyID;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, Range);
    }

    public override void Populate()
    {
        EnemyFamilyData data = EnemyDatabase.Instance.GetFamily(EnemyFamilyID);

        int numberOfEnemies = data.MinAppearance;
        for(int i=data.MinAppearance;i<data.MaxAppearance;i++)
        {
            float rand = Random.Range(0, 1f);
            if (rand <= 0.33f)
                numberOfEnemies++;
        }

        List<EnemyData> spawns = new List<EnemyData>();
        for (int i = 0; i < numberOfEnemies; i++)
        {
            EnemyRank rank = GameConstants.RandomRank();
            spawns.Add(data.EnemyForRank(rank));
        }

        List<Vector3> positions = EvenlySpacedPositionsAtDistance(transform.position, Range, 0.5f, spawns.Count);
        for (int i=0;i<positions.Count;i++)
        {
            DropController.Instance.SpawnEnemy(spawns[i], positions[i]);
        }
    }
}
