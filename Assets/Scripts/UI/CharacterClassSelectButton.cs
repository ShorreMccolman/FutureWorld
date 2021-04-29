using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterClassSelectButton : CharacterCreatorButton {

    [SerializeField] CharacterCreator _creator;
    [SerializeField] CharacterClass _class;

    [SerializeField] Text Label;

    public CharacterClass GetClass() { return _class; }

    public void Select()
    {
        _creator.ChangeCharacterClass(this);
    }

    public void UpdateUI(bool isClass)
    {
        Label.color = isClass ? Color.blue : Color.black;
    }
}
