using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISlotable
{
    void Select(int slot);
    bool ClickItemButton(InventoryItemButton button);
    void Hover(InventoryItem item);
}
