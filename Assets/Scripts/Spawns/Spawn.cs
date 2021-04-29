using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spawn : MonoBehaviour
{
    public abstract void Populate();

    protected Vector3 PositionInRange(Vector3 origin, float range)
    {
        float radius = Random.Range(0, range);
        float angle = Random.Range(0, 2 * Mathf.PI);

        return origin + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
    }

    protected List<Vector3> EvenlySpacedPositionsAtDistance(Vector3 origin, float radius, float variance, int enemyCount)
    {
        List<Vector3> results = new List<Vector3>();

        float initialAngle = Random.Range(0, 2 * Mathf.PI);

        bool hasCenterSpawn = /*enemyCount % 2 != 0;*/  Random.Range(0f,1f) < 0.33f;
        int count = hasCenterSpawn ? (enemyCount - 1) : enemyCount;
        float arc = 2 * Mathf.PI / (float)count;

        if (hasCenterSpawn)
            results.Add(origin);

        for(int i=0;i<count;i++)
        {
            float angle = initialAngle + arc * i + Random.Range(-variance * arc, variance * arc);
            float distance = Random.Range(radius / 2, radius);
            results.Add(origin + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * distance);
        }

        return results;
    }

    public void Terminate()
    {
        Destroy(gameObject);
    }
}
