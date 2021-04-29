using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputOptionButton : OptionButton
{
    public void OnCommitInput()
    {
        int result;
        bool isInt = int.TryParse(Input.text, out result);
        
        if(isInt)
            OnIndexClick?.Invoke(result);
    }
}
