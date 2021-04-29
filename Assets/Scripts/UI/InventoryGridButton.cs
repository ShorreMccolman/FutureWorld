using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryGridButton : MonoBehaviour {

    int _slot;
    ISlotable _menu;

    public void Setup(int slot, ISlotable menu)
    {
        _slot = slot;
        _menu = menu;
    }

	public void Select()
    {
        _menu.Select(_slot);
    }
}
