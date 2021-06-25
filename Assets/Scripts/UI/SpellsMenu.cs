using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellsMenu : Menu
{
    [SerializeField] SchoolButton[] SchoolButtons;
    [SerializeField] SpellButton[] SpellButtons;

    [SerializeField] Image SchoolIcon;

    PartyMember _member;
    int _selectedSchoolIndex;
    int _selectedSpellIndex;

    protected override void Init()
    {
        HUD.OnSpellsPressed += Setup;

        base.Init();
    }

    public void Setup(PartyMember member)
    {
        MenuManager.Instance.OpenMenu(MenuTag, true);

        _member = member;
        _selectedSpellIndex = -1;
        _selectedSchoolIndex = -1;

        foreach (var button in SchoolButtons)
        {
            button.gameObject.SetActive(false);
        }

        for (int i=0;i<SchoolButtons.Length;i++)
        {
            if (_member.Skillset.KnowsSkill(((SpellSchool)i).ToString()))
            {
                SchoolButtons[i].Setup(this, i);
                if (_selectedSchoolIndex == -1)
                {
                    SetupSchool(i);
                }
            }
        }

        if (_selectedSchoolIndex == -1)
            SetupSchool(0);
    }

    public void SetQuickSpell()
    {
        if (_selectedSpellIndex == -1)
            return;

        CloseMenu();
    }

    protected void SetupSchool(int index)
    {
        if (_selectedSchoolIndex == index)
            return;

        _selectedSchoolIndex = index;

        foreach(var button in SpellButtons)
        {
            button.gameObject.SetActive(false);
        }

        foreach (var button in SchoolButtons)
        {
            button.SetHighlight(false);
        }
        SchoolButtons[index].SetHighlight(true);

        SchoolIcon.sprite = SchoolButtons[_selectedSchoolIndex].Icon.sprite;

        for (int i = 0; i < SpellButtons.Length; i++)
        {
            if(_member.SpellLog.KnowsSpell((SpellSchool)index, i))
            {
                SpellButtons[i].Setup(this, i, SpellDatabase.Instance.GetSpell((SpellSchool)index, i));
            }
        }
    }

    public void SelectSpellButton(int index)
    {
        if(_selectedSpellIndex == index)
        {
            CloseMenu();
        }
        else
        {
            if (_selectedSpellIndex >= 0)
                SpellButtons[_selectedSpellIndex].SetHighlight(false);

            _selectedSpellIndex = index;

            SpellButtons[_selectedSpellIndex].SetHighlight(true);
        }
    }

    public void SelectSchoolButton(int index)
    {
        _selectedSpellIndex = -1;
        SetupSchool(index);
    }

    public override void OnClose()
    {
        foreach (var button in SpellButtons)
            button.gameObject.SetActive(false);

        foreach (var button in SchoolButtons)
            button.gameObject.SetActive(false);
    }

}
