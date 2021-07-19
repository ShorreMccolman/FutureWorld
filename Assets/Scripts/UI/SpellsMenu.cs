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
    SpellData _selectedSpell;

    protected override void Init()
    {
        HUD.OnSpellsPressed += Setup;

        base.Init();
    }

    public void Setup(PartyMember member)
    {
        MenuManager.Instance.SwapMenu(MenuTag, true, false);
        Party.Instance.SetMemberLock(true);

        _member = member;
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
        Party.Instance.ActiveMember.Profile.SetQuickSpell(_selectedSpell);
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

        SchoolIcon.sprite = SpriteHandler.FetchSprite("Spells", ((SpellSchool)_selectedSchoolIndex).ToString());
        for (int i = 0; i < SpellButtons.Length; i++)
        {
            if(_member.SpellLog.KnowsSpell((SpellSchool)index, i))
            {
                SpellButtons[i].Setup(this, i, SpellDatabase.Instance.GetSpell((SpellSchool)index, i));
            }
        }
    }

    public void SelectSpellButton(SpellButton button, SpellData data)
    {
        if(_selectedSpell == data)
        {
            CloseMenu();
            Party.Instance.ActiveMember.TryCast(data);
        }
        else
        {
            _selectedSpell = data;
            foreach (var b in SpellButtons)
                b.SetHighlight(false);
            button.SetHighlight(true);
        }
    }

    public void SelectSchoolButton(int index)
    {
        _selectedSpell = null;
        SetupSchool(index);
    }

    public override void OnClose()
    {
        Party.Instance.SetMemberLock(false);

        foreach (var button in SpellButtons)
            button.gameObject.SetActive(false);

        foreach (var button in SchoolButtons)
            button.gameObject.SetActive(false);
    }

}
