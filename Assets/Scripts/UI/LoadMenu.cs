using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadMenu : Menu {

    [SerializeField] RawImage Image;
    [SerializeField] Text TimeLabel;
    [SerializeField] GameObject ButtonTemplate;
    [SerializeField] Transform Content;
    [SerializeField] bool isGame;

    List<LoadButton> _buttons;

    int _selectedSlot;

    protected override void Init()
    {
        _selectedSlot = -1;

        _buttons = new List<LoadButton>();
        for (int i = 0; i < 10; i++)
        {
            GameObject obj = Instantiate(ButtonTemplate, Content);
            Vector3 offset = isGame ? Vector3.right * 160 : Vector3.zero;
            obj.transform.position = Content.position + Vector3.down * 40 * i + offset;

            LoadButton button = obj.GetComponent<LoadButton>();
            _buttons.Add(button);
        }
    }

    public override void OnOpen()
    {
        List<PlayerFileInfo> infoList = new List<PlayerFileInfo>();
        for (int i = 0; i < 10; i++)
        {
            infoList.Add(FileManager.LoadBinary<PlayerFileInfo>("info_" + i));
            _buttons[i].Setup(this, i, infoList[i]);
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

    public void SelectSlot(int slot)
    {
        _selectedSlot = slot;
        
        Texture2D tex = FileManager.LoadPNG( "screenshot_" + slot, "SaveData");
        PlayerFileInfo info = FileManager.LoadBinary<PlayerFileInfo>("info_" + slot);
        if (tex != null)
        {
            Image.texture = tex;
            Image.color = Color.white;
        }
        else
        {
            Image.color = Color.black;
        }
        if(info != null)
        {
            string date = GameConstants.monthDict[info.month] + " " + info.day + ", " + info.hour + ":" + info.minute;
            TimeLabel.text = date;
        }
        else
        {
            TimeLabel.text = "";
        }
    }
}
