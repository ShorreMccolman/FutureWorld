using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellButton : MonoBehaviour
{
    [SerializeField] Image Bg;
    [SerializeField] Image SpellIcon;
    [SerializeField] Text Label;

    SpellsMenu _menu;
    int _index;

    public void Setup(SpellsMenu menu, int index, SpellData spell)
    {
        _menu = menu;
        _index = index;

        SpellIcon.sprite = spell.Icon;
        Label.text = spell.DisplayName;

        SetHighlight(false);
        gameObject.SetActive(true);
    }

    public void SelectSpell()
    {
        _menu.SelectSpellButton(_index);
    }

    public void SetHighlight(bool isHighlighted)
    {
        Bg.color = isHighlighted ? Color.white : Color.grey;
    }
}
