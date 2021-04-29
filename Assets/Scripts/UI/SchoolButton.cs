using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SchoolButton : MonoBehaviour
{
    [SerializeField] Image Bg;
    public Image Icon;

    SpellsMenu _menu;
    int _index;

    public void Setup(SpellsMenu menu, int index)
    {
        _menu = menu;
        _index = index;

        SetHighlight(false);
        gameObject.SetActive(true);
    }

    public void SelectSchool()
    {
        _menu.SelectSchoolButton(_index);
    }

    public void SetHighlight(bool isHighlighted)
    {
        Bg.color = isHighlighted ? new Color(190, 181, 121) : Color.grey;
    }
}
