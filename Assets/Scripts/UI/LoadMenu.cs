using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadMenu : FileMenu 
{
    public override void OnOpen()
    {
        _selectedSlot = -1;

        _buttons = new List<GameObject>();
        for (int i = 0; i < 10; i++)
        {
            GameObject obj = Instantiate(ButtonTemplate, Content);
            obj.transform.position = Content.position + Vector3.down * 40 * i + Vector3.right * 160;

            LoadButton button = obj.GetComponent<LoadButton>();
            button.Setup(this, i, FileManager.LoadBinary<PlayerFileInfo>("info_" + i));
            _buttons.Add(obj);
        }

        SelectSlot(0);
        _buttons[0].GetComponentInChildren<Button>().Select();
    }

    public void Load()
    {
        if (_selectedSlot < 0)
            return;

        GameController.Instance.LoadGame(_selectedSlot);
    }
}
