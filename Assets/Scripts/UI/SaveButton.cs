using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveButton : MonoBehaviour {

    SaveMenu _menu;
    int _slot;

    public void Setup(SaveMenu menu, int slot)
    {
        _menu = menu;
        _slot = slot;
    }

    public void Select(string title)
    {
        _menu.SelectSlot(_slot, title);
    }
}
