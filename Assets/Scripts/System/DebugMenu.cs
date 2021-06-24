using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMenu : MonoBehaviour
{
    [SerializeField] InputField ChestInput;

    void Start()
    {
        gameObject.SetActive(false);
    }

    public void MasterSkills()
    {
        foreach(var member in Party.Instance.Members)
        {
            foreach(var skill in member.Skillset.Skills)
            {
                for(int i=0;i<10;i++)
                {
                    skill.Upgrade();
                }
                skill.IncreaseMastery();
                skill.IncreaseMastery();
            }
        }
    }

    public void TestChest()
    {
        string input = ChestInput.text;
        string[] split = input.Split('/');

        Chest chest = new Chest(new SpawnQuantities(split));
        ChestEntity.DebugInspect(chest);
    }

    public void AddToInventory()
    {
        string input = ChestInput.text;

        InventoryItem item = null;
        if (input.Contains(" "))
        {
            string[] split = input.Split(' ');
            item = ItemDatabase.Instance.GetInventoryItem(split[1], (TreasureLevel)int.Parse(split[0]), null);
        }
        else
        {
            item = ItemDatabase.Instance.GetInventoryItem(input);
        }
        item.TryIdentify(100000);
        Party.Instance.ActiveMember.Inventory.AddItem(item);
    }

    public void SpawnDrop()
    {
        string input = ChestInput.text;

        InventoryItem item = null;
        if (input.Contains(" "))
        {
            string[] split = input.Split(' ');
            item = ItemDatabase.Instance.GetInventoryItem(split[1], (TreasureLevel)int.Parse(split[0]), null);
        }
        else
        {
            item = ItemDatabase.Instance.GetInventoryItem(input);
        }
        item.TryIdentify(100000);
        DropController.Instance.DropItem(item, PartyController.Instance.Entity.DropPosition);
    }

    public void AddGold()
    {
        string input = ChestInput.text;
        int value = int.Parse(input);
        Party.Instance.CollectGold(value);
    }

    public void AddXP()
    {
        string input = ChestInput.text;
        int value = int.Parse(input);
        Party.Instance.EarnXP(value);
    }

    public void AddSkillPoints()
    {

    }
}
