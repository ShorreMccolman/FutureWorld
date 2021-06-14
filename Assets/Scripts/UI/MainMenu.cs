using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : Menu {

    public void NewGame()
    {
        SoundManager.Instance.PlayUISound("Button");
        MenuManager.Instance.OpenMenu("NewMenu");
    }

    public void QuickStart()
    {
        SoundManager.Instance.PlayUISound("Button");

        CharacterData char1 = new CharacterData(0, CharacterClass.Paladin, true);
        char1.Might = 17;
        char1.Intellect = 5;
        char1.Personality = 15;
        char1.Endurance = 15;
        char1.Accuracy = 15;
        char1.Speed = 13;
        char1.Luck = 6;
        char1.Skills.Add("Shield");
        char1.Skills.Add("Chain");

        CharacterData char2 = new CharacterData(1, CharacterClass.Archer, true);
        char2.Might = 14;
        char2.Intellect = 15;
        char2.Personality = 5;
        char2.Endurance = 15;
        char2.Accuracy = 17;
        char2.Speed = 13;
        char2.Luck = 6;
        char2.Skills.Add("Axe");
        char2.Skills.Add("Perception");

        CharacterData char3 = new CharacterData(2, CharacterClass.Cleric, true);
        char3.Might = 11;
        char3.Intellect = 7;
        char3.Personality = 17;
        char3.Endurance = 15;
        char3.Accuracy = 13;
        char3.Speed = 11;
        char3.Luck = 12;
        char3.Skills.Add("Mind");
        char3.Skills.Add("Meditation");

        CharacterData char4 = new CharacterData(3, CharacterClass.Sorcerer, true);
        char4.Might = 11;
        char4.Intellect = 17;
        char4.Personality = 7;
        char4.Endurance = 15;
        char4.Accuracy = 13;
        char4.Speed = 13;
        char4.Luck = 9;
        char4.Skills.Add("Water");
        char4.Skills.Add("Meditation");

        CharacterData[] data = new CharacterData[] { char1, char2, char3, char4 };
        GameController.Instance.StartNewGame(data);
    }

    public void LoadGame()
    {
        SoundManager.Instance.PlayUISound("Button");
        MenuManager.Instance.OpenMenu("LoadMenu");
    }

    public void Credits()
    {
        SoundManager.Instance.PlayUISound("Button");
        MenuManager.Instance.OpenMenu("Credits");
    }

    public void QuitGame()
    {
        SoundManager.Instance.PlayUISound("Button");
        Application.Quit();
    }
}
