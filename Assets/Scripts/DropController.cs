﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;

public class DropController : MonoBehaviour {
    public static DropController Instance { get; private set; }
    void Awake() { Instance = this; }

    [SerializeField] GameObject ItemDropEntityObject;
    [SerializeField] GameObject ChestEntityObject;
    [SerializeField] GameObject EnemyEntityObject;
    [SerializeField] GameObject NPCEntityObject;
    [SerializeField] GameObject ResidenceEntityObject;
    [SerializeField] GameObject MerchantEntityObject;
    [SerializeField] GameObject ProjectileEntityObject;

    public void LoadDrops(XmlNodeList nodes)
    {
        for(int i=0;i<nodes.Count;i++)
        {
            XmlNode node = nodes.Item(i);

            GameObject obj = Instantiate(ItemDropEntityObject);

            ItemDropEntity ent = obj.GetComponent<ItemDropEntity>();
            ItemDrop drop = new ItemDrop(node);
            ent.Setup(drop);
            drop.CreateEntity(obj);
        }
    }

    public void DropItem(InventoryItem item, Vector3 position)
    {
        GameObject obj = Instantiate(ItemDropEntityObject);

        ItemDropEntity ent = obj.GetComponent<ItemDropEntity>();
        ItemDrop drop = new ItemDrop(item);
        ent.Setup(drop);
        drop.CreateEntity(obj, position);
    }

    public void DropItem(InventoryItem item, Transform location)
    {
        GameObject obj = Instantiate(ItemDropEntityObject);

        ItemDropEntity ent = obj.GetComponent<ItemDropEntity>();
        ItemDrop drop = new ItemDrop(item);
        ent.Setup(drop);
        drop.CreateEntity(obj, location);
    }

    public void LoadChests(XmlNodeList nodes)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            XmlNode node = nodes.Item(i);

            GameObject obj = Instantiate(ChestEntityObject);

            ChestEntity ent = obj.GetComponent<ChestEntity>();
            Chest chest = new Chest(node);
            ent.Setup(chest);
            chest.CreateEntity(obj);
        }
    }

    public void SpawnChest(SpawnQuantities quantities, Transform location)
    {
        GameObject obj = Instantiate(ChestEntityObject);

        ChestEntity ent = obj.GetComponent<ChestEntity>();
        Chest chest = new Chest(quantities);
        ent.Setup(chest);
        chest.CreateEntity(obj, location);
    }

    public void LoadProjectiles(XmlNodeList nodes)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            XmlNode node = nodes.Item(i);

            GameObject obj = Instantiate(ProjectileEntityObject);

            ProjectileEntity ent = obj.GetComponent<ProjectileEntity>();
            Projectile proj = new Projectile(node);
            ent.Setup(proj);
            proj.CreateEntity(obj);
        }
    }

    public void SpawnProjectile(Transform location, Projectile proj)
    {
        GameObject obj = Instantiate(ProjectileEntityObject);

        ProjectileEntity ent = obj.GetComponent<ProjectileEntity>();
        ent.Setup(proj);

        proj.CreateEntity(obj, location);
    }

    public void LoadEnemies(XmlNodeList nodes)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            XmlNode node = nodes.Item(i);

            Enemy enemy = new Enemy(node);

            if(enemy.NPC == null)
            {
                GameObject obj = Instantiate(EnemyEntityObject);
                EnemyEntity ent = obj.GetComponent<EnemyEntity>();
                ent.Setup(enemy);
                enemy.CreateEntity(obj);
            } 
            else
            {
                GameObject obj = Instantiate(NPCEntityObject);
                NPCEntity ent = obj.GetComponent<NPCEntity>();
                ent.Setup(enemy);
                enemy.CreateEntity(obj);
            }
        }
    }

    public void SpawnNPC(EnemyData data, Vector3 position)
    {
        GameObject obj = Instantiate(NPCEntityObject);

        NPCEntity ent = obj.GetComponent<NPCEntity>();
        Enemy enemy = new Enemy(data);
        NPCDatabase.Instance.CreateRandomNPC(enemy);
        ent.Setup(enemy, enemy.NPC);
        enemy.CreateEntity(obj, position);
    }

    public void SpawnEnemy(EnemyData data, Vector3 position)
    {
        GameObject obj = Instantiate(EnemyEntityObject);

        EnemyEntity ent = obj.GetComponent<EnemyEntity>();
        Enemy enemy = new Enemy(data);
        ent.Setup(enemy);
        enemy.CreateEntity(obj, position);
    }

    public void LoadResidences(XmlNodeList nodes)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            XmlNode node = nodes.Item(i);

            GameObject obj = Instantiate(ResidenceEntityObject);

            ResidencyEntity ent = obj.GetComponent<ResidencyEntity>();
            Residency residence = new Residency(node);
            ent.Setup(residence);
            residence.CreateEntity(obj);
        }
    }

    public void SpawnResidence(ResidencyDBObject data, Transform position)
    {
        GameObject obj = Instantiate(ResidenceEntityObject);

        ResidencyEntity ent = obj.GetComponent<ResidencyEntity>();
        Residency residency = new Residency(data);
        ent.Setup(residency);
        residency.CreateEntity(obj, position);
    }

    public void LoadMerchants(XmlNodeList nodes)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            XmlNode node = nodes.Item(i);

            GameObject obj = Instantiate(MerchantEntityObject);

            MerchantEntity ent = obj.GetComponent<MerchantEntity>();
            Merchant merchant = new Merchant(node);
            ent.Setup(merchant);
            merchant.CreateEntity(obj);
        }
    }

    public void SpawnMerchants(string ID, Transform position)
    {
        GameObject obj = Instantiate(MerchantEntityObject);

        MerchantEntity ent = obj.GetComponent<MerchantEntity>();
        Merchant merchant = new Merchant(MerchantDatabase.Instance.GetMerchant(ID));
        ent.Setup(merchant);
        merchant.CreateEntity(obj, position);
    }
}
