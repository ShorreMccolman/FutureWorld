using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreatorStatButton : CharacterCreatorButton {

    [SerializeField] CharacterCreatorUI _creatorUI;
    [SerializeField] CharacterStat _stat;

    [SerializeField] Text StatQuantityLabel;

    int _defaultValue;

    public CharacterStat GetStat() { return _stat; }

    public void Select()
    {
        _creatorUI.SelectStat(this);
    }

    public void UpdateUI(int quantity, bool init)
    {
        if (init)
            _defaultValue = quantity;

        StatQuantityLabel.text = quantity.ToString();

        if (quantity == _defaultValue)
            StatQuantityLabel.color = Color.black;
        else if (quantity < _defaultValue)
            StatQuantityLabel.color = Color.red;
        else
            StatQuantityLabel.color = Color.green;

    }
}
