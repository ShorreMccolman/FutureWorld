using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveButton : MonoBehaviour 
{
    [SerializeField] InputField Label;

    PlayerFileInfo _info;
    SaveMenu _menu;
    int _slot;

    public void Setup(SaveMenu menu, int slot, PlayerFileInfo info)
    {
        _menu = menu;
        _slot = slot;

        _info = info;

        if (info != null)
            Label.text = info.Title;
    }

    public void Select(string title)
    {
        _menu.SelectSlot(_slot, title);
    }
}
