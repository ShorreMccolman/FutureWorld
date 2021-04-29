using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MerchantOption
{
    Buy,
    Sell,
    Identify,
    Repair,
    Special,
    Learn
}

public enum ServiceOption
{
    Room,
    Food,
    Drink,
    Tip
}

public class DialogOptionButton : OptionButton
{
    public void Click()
    {
        OnIndexClick?.Invoke(_option);
        OnCommit?.Invoke();
    }
}
