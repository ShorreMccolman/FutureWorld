using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellButton : MonoBehaviour, IPopable, IInfoMessenger
{
    [SerializeField] Image Bg;
    [SerializeField] Image SpellIcon;
    [SerializeField] Text Label;

    SpellsMenu _menu;
    int _index;
    bool _isHighlighted;
    SpellData _spell;

    public void Setup(SpellsMenu menu, int index, SpellData spell)
    {
        _menu = menu;
        _index = index;
        _spell = spell;

        SpellIcon.sprite = spell.Icon;
        Label.text = spell.DisplayName;

        SetHighlight(false);
        gameObject.SetActive(true);
    }

    public void SelectSpell()
    {
        _menu.SelectSpellButton(this, _spell);
    }

    public void SetHighlight(bool isHighlighted)
    {
        _isHighlighted = isHighlighted;
        Bg.color = isHighlighted ? Color.white : Color.grey;
    }

    public void ShowPopup()
    {
        Popups.ShowSpell(_spell);
    }

    public string GetInfoMessage()
    {
        if(_isHighlighted)
            return "Cast " + _spell.DisplayName;
        else
            return "Select " + _spell.DisplayName;
    }
}
