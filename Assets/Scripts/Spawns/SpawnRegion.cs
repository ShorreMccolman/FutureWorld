using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRegion : MonoBehaviour
{
    ItemSpawn[] _itemSpawns;
    EnemySpawn[] _enemySpawns;

    void Awake()
    {
        _itemSpawns = GetComponentsInChildren<ItemSpawn>();
        _enemySpawns = GetComponentsInChildren<EnemySpawn>();
    }

    public virtual void Populate(ItemDatabase itemDB, EnemyDatabase enemyDB)
    {
        //// Items
        //Queue<InventoryItem> spawnItems = itemDB.ItemQueueByBudget(Budget, Levels, _itemSpawns.Length * 3);

        //List<ItemSpawn> spawns = new List<ItemSpawn>();
        //foreach (var spawn in _itemSpawns)
        //    spawns.Add(spawn);

        //while(spawnItems.Count > 0)
        //{
        //    int randomSpawn = Random.Range(0, spawns.Count);
        //    ItemSpawn spawn = spawns[randomSpawn];

        //    InventoryItem item = spawnItems.Dequeue();
        //    spawn.AddItem(item);
        //    if(spawn.IsFull)
        //    {
        //        spawns.Remove(spawn);
        //    }
        //}

        //foreach (var spawn in _itemSpawns)
        //    spawn.Populate();

        //// Enemies
        //foreach(var spawn in _enemySpawns)
        //{
        //    EnemyFamilyData data = enemyDB.GetFamily(spawn.EnemyFamilyID);
        //    spawn.SetFamily(data);
        //    spawn.Populate();
        //}
    }

    public void Terminate()
    {
        for(int i=0;i<_itemSpawns.Length;i++)
        {
            Destroy(_itemSpawns[i].gameObject);
        }
        for (int i = 0; i < _enemySpawns.Length; i++)
        {
            Destroy(_enemySpawns[i].gameObject);
        }
        Destroy(gameObject);
    }
}
