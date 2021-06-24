using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestMenu : InventoryMenu
{
    protected override void Init()
    {
        ChestEntity.OnInspectChest += Setup;

        base.Init();
    }

    public void Setup(Chest chest)
    {
        MenuManager.Instance.OpenMenu(MenuTag, true);

        _inventory = chest.Inventory;
        SetupInventory();
    }

    public override void OnOpen()
    {

    }

    public override void OnClose()
    {

    }
}
