using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentItemButton : ItemButton {

    public EquipSlot Slot { get; private set; }

    CharacterEquipmentDisplay _display;

    public void Setup(InventoryItem item, CharacterEquipmentDisplay display, EquipSlot slot)
    {
        _display = display;
        Slot = slot;

        if (item.IsBroken)
            Image.color = Color.red;
        else if (!item.IsIdentified)
            Image.color = Color.green;
        else
            Image.color = Color.white;

        Setup(item, true, true);
    }

    protected override void OnLeftDown()
    {
        if (!_holding)
        {
            bool success = _display.PickupItem(this, Slot);
            if (success)
            {
                Image.raycastTarget = false;
                _holding = true;
            }
        }
    }
}
