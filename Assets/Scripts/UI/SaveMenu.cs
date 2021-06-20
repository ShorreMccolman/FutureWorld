using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveMenu : FileMenu 
{
    string _selectedTitle;

    public override void OnOpen()
    {
        _selectedSlot = -1;
        _selectedTitle = "";

        _buttons = new List<GameObject>();
        for (int i = 0; i < 10; i++)
        {
            GameObject obj = Instantiate(ButtonTemplate, Content);
            obj.transform.position = Content.position + Vector3.down * 40 * i + Vector3.right * 160;

            SaveButton button = obj.GetComponent<SaveButton>();
            button.Setup(this, i, FileManager.LoadBinary<PlayerFileInfo>("info_" + i));
            _buttons.Add(obj);
        }

        SelectSlot(0);
    }

    public void Save()
    {
        if (_selectedSlot < 0 || string.IsNullOrEmpty(_selectedTitle))
            return;

        GameController.Instance.SaveGame(_selectedSlot, _selectedTitle);
        MenuManager.Instance.CloseMenu(MenuTag);
    }

    public void SelectSlot(int slot, string title)
    {
        _selectedSlot = slot;
        _selectedTitle = title;
    }

}
