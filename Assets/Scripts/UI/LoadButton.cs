using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadButton : MonoBehaviour {

    [SerializeField] Text Label;

    PlayerFileInfo _info;
    LoadMenu _menu;
    int _slot;

    public void Setup(LoadMenu menu, int slot, PlayerFileInfo info)
    {
        _menu = menu;
        _slot = slot;
        _info = info;

        if (info != null)
            Label.text = info.Title;
    }

    public void Select()
    {
        _menu.SelectSlot(_slot);
    }
}
