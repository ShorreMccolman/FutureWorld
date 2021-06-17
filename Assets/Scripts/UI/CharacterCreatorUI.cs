using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreatorUI : MonoBehaviour {

    CharacterCreator _creator;

    [SerializeField] Image Portrait;
    [SerializeField] Text ClassText;
    [SerializeField] Image ClassLogo;
    [SerializeField] InputField NameField;
    [SerializeField] CharacterCreatorStatButton[] StatButtons;
    [SerializeField] Text[] SkillSlotText;
    [SerializeField] CharacterSkillSlotButton[] SkillSlotButtons;

    public CharacterData Data { get; private set; }
    public CharacterCreatorStatButton SelectedButton { get; private set; }

    public void Setup(CharacterCreator creator, CharacterData data)
    {
        Data = data;
        _creator = creator;
        SelectedButton = StatButtons[0];
        UpdateUI(true);
    }

    public void EnterNewName(string name)
    {
        Data.SetName(name);
    }

    public bool IsFocusedName()
    {
        return NameField.isFocused;
    }

    public void SelectStat(CharacterCreatorStatButton button)
    {
        SelectedButton = button;
        _creator.SelectCharacterStat(this);
    }

    public void SelectStat(bool forward)
    {
        for(int i=0;i<StatButtons.Length;i++)
        {
            if(SelectedButton == StatButtons[i])
            {
                int diff = forward ? 1 : -1;
                SelectedButton = StatButtons[(i + diff + StatButtons.Length) % StatButtons.Length];
                return;
            }
        }
    }

    public void SelectStat(CharacterStat stat)
    {
        for (int i = 0; i < StatButtons.Length; i++)
        {
            if (StatButtons[i].GetStat() == stat)
            {
                SelectedButton = StatButtons[i];
                return;
            }
        }
    }

    public void RemoveCharacterSkill(CharacterSkillSlotButton button)
    {
        _creator.RemoveCharacterSkill(this, button.SkillName);
    }

    public void NextPortrait()
    {
        Data.ChangePortrait(_creator.GetPortraitID(Data.PortraitID,1));
        UpdateUI();
    }

    public void PreviousPortrait()
    {
        Data.ChangePortrait(_creator.GetPortraitID(Data.PortraitID,-1));
        UpdateUI();
    }

    public void UpdateUI(bool init = false)
    {
        if(ClassText.text != Data.Class.ToString())
        {
            Sprite sprite = Resources.Load<Sprite>("Icon/ClassLogo" + Data.Class.ToString());
            ClassLogo.sprite = sprite;
        }

        Portrait.sprite = _creator.GetPortrait(Data.PortraitID);
        ClassText.text = Data.Class.ToString();
        NameField.text = Data.Name;

        StatButtons[0].UpdateUI(Data.Might, init);
        StatButtons[1].UpdateUI(Data.Intellect, init);
        StatButtons[2].UpdateUI(Data.Personality, init);
        StatButtons[3].UpdateUI(Data.Endurance, init);
        StatButtons[4].UpdateUI(Data.Accuracy, init);
        StatButtons[5].UpdateUI(Data.Speed, init);
        StatButtons[6].UpdateUI(Data.Luck, init);

        SkillSlotText[0].text = Data.Skills[0];
        SkillSlotText[1].text = Data.Skills[1];

        SkillSlotButtons[0].UpdateUI(Data.GetSkillNameByIndex(2));
        SkillSlotButtons[1].UpdateUI(Data.GetSkillNameByIndex(3));
    }
}
