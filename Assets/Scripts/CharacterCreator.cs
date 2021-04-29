using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreator : Menu
{

    public CharacterCreatorUI UI1;
    public CharacterCreatorUI UI2;
    public CharacterCreatorUI UI3;
    public CharacterCreatorUI UI4;

    [SerializeField] GameObject StatSelector;
    [SerializeField] Text PointsRemainingText;
    [SerializeField] CharacterClassSelectButton[] ClassSelectButtons;
    [SerializeField] CharacterSkillSelectButton[] SkillSelectButtons;

    CharacterData char1;
    CharacterData char2;
    CharacterData char3;
    CharacterData char4;

    CharacterCreatorUI _selectedUI;

    List<Sprite> _portraitSprites;

    public int PointsRemaining
    {
        get { return 50 - char1.PointsSpent - char2.PointsSpent - char3.PointsSpent - char4.PointsSpent; }
    }
	
    protected override void Init()
    {
        LoadPortraits();

        char1 = new CharacterData(0, CharacterClass.Knight);
        UI1.Setup(this, char1);

        char2 = new CharacterData(1, CharacterClass.Archer);
        UI2.Setup(this, char2);

        char3 = new CharacterData(2, CharacterClass.Cleric);
        UI3.Setup(this, char3);

        char4 = new CharacterData(3, CharacterClass.Sorcerer);
        UI4.Setup(this, char4);

        _selectedUI = UI1;
        
        UpdateUI();
    }

    void LoadPortraits()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Portraits");
        _portraitSprites = new List<Sprite>();
        foreach(var sprite in sprites)
        {
            if (sprite.name.Contains("neutral"))
                _portraitSprites.Add(sprite);
        }

        Debug.Log("Loaded " + _portraitSprites.Count + " portraits");
    }

    public Sprite GetPortrait(int ID)
    {
        if(ID >= _portraitSprites.Count || ID < 0)
        {
            Debug.LogError("Invalid Portrait requested");
            return _portraitSprites[0];
        }
        return _portraitSprites[ID];
    }

    public int GetPortraitID(int id, int offset = 0)
    {
        return (id + offset + _portraitSprites.Count) % _portraitSprites.Count;
    }

    public void ModifySelectedStat(int amount)
    {
        if (_selectedUI == null || _selectedUI.SelectedButton == null)
            return;

        CharacterCreatorStatButton button = _selectedUI.SelectedButton as CharacterCreatorStatButton;
        if (button == null)
            return;

        if (PointsRemaining - amount < 0)
            return;

        switch(button.GetStat())
        {
            case CharacterStat.Might:
                _selectedUI.Data.Might += amount;
                break;
            case CharacterStat.Intellect:
                _selectedUI.Data.Intellect += amount;
                break;
            case CharacterStat.Personality:
                _selectedUI.Data.Personality += amount;
                break;
            case CharacterStat.Endurance:
                _selectedUI.Data.Endurance += amount;
                break;
            case CharacterStat.Accuracy:
                _selectedUI.Data.Accuracy += amount;
                break;
            case CharacterStat.Speed:
                _selectedUI.Data.Speed += amount;
                break;
            case CharacterStat.Luck:
                _selectedUI.Data.Luck += amount;
                break;
        }

        UpdateUI();

    }

    public void SelectCharacterStat(CharacterCreatorUI UI)
    {
        _selectedUI = UI;
        StatSelector.transform.position = _selectedUI.SelectedButton.transform.position;

        UpdateUI();
    }

    public void ChangeCharacterClass(CharacterClassSelectButton button)
    {
        if (_selectedUI == null)
            return;

        _selectedUI.Data.ChangeClass(button.GetClass());
        _selectedUI.UpdateUI(true);

        UpdateUI();
    }

    public void SelectCharacterSkill(CharacterSkillSelectButton button)
    {
        if (_selectedUI == null)
            return;

        _selectedUI.Data.SelectSkill(button.SkillName);

        UpdateUI();
    }

    public void RemoveCharacterSkill(CharacterCreatorUI UI, string skillName)
    {
        _selectedUI = UI;
        _selectedUI.Data.RemoveSkill(skillName);

        UpdateUI();
    }

    public void UpdateUI(bool init = false)
    {
        _selectedUI.UpdateUI();
        PointsRemainingText.text = PointsRemaining.ToString();
        for (int i = 0; i < SkillSelectButtons.Length; i++)
        {
            string skillName = _selectedUI.Data.SuggestedSkills[i];
            SkillSelectButtons[i].UpdateUI(skillName, _selectedUI.Data.HasSkill(skillName));
        }
        foreach(var button in ClassSelectButtons)
        {
            button.UpdateUI(button.GetClass() == _selectedUI.Data.Class);
        }
    }

    public void FinalizeCreation()
    {
        CharacterData[] data = new CharacterData[] { char1, char2, char3, char4 };
        GameController.Instance.StartNewGame(data);
    }
}
