using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveMenu : Menu {

    [SerializeField] GameObject ButtonTemplate;
    [SerializeField] Transform Content;

    //List<SaveButton> _buttons;
    int _selectedSlot;
    string _selectedTitle;

    protected override void Init()
    {
        _selectedSlot = -1;
        _selectedTitle = "";

        for(int i=0;i<10;i++)
        {
            GameObject obj = Instantiate(ButtonTemplate, Content);
            obj.transform.position = Content.position + Vector3.down * 40 * i + Vector3.right * 160;

            SaveButton button = obj.GetComponent<SaveButton>();
            button.Setup(this, i);
            //_buttons.Add(button);
        }
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
