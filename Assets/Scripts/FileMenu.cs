using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FileMenu : Menu
{
    [SerializeField] protected RawImage Image;
    [SerializeField] protected Text TimeLabel;
    [SerializeField] protected GameObject ButtonTemplate;
    [SerializeField] protected RectTransform Content;

    protected List<GameObject> _buttons;
    protected int _selectedSlot;

    public void SelectSlot(int slot)
    {
        _selectedSlot = slot;

        Texture2D tex = FileManager.LoadPNG("screenshot_" + slot, "SaveData");
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
        if (info != null)
        {
            string date = GameConstants.monthDict[info.month] + " " + info.day + ", " + info.hour + ":" + info.minute;
            TimeLabel.text = date;
        }
        else
        {
            TimeLabel.text = "";
        }
    }

    public override void OnClose()
    {
        foreach (var button in _buttons)
            Destroy(button);
        _buttons.Clear();
    }
}
