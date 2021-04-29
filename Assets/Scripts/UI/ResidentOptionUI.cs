using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResidentOptionUI : MonoBehaviour
{
    [SerializeField] Image Sprite;
    [SerializeField] Text Name;

    Resident _resident;
    ResidenceMenu _menu;

    public void Setup(ResidenceMenu menu, Resident resident)
    {
        _menu = menu;
        _resident = resident;

        Sprite.sprite = resident.Data.Sprite;
        Name.text = resident.Data.ShortName;
    }

    public void Hover()
    {
        HUD.Instance.SendInfoMessage("Converse with " + _resident.Data.FirstName);
    }

    public void OnClick()
    {
        _menu.SelectResident(_resident);
    }
}
