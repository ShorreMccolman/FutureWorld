using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalanceMessenger : MonoBehaviour, IInfoMessenger
{
    public string GetInfoMessage()
    {
        return "You have " + Party.Instance.CurrentGold + " gold, " + Party.Instance.CurrentBalance + " in the bank";
    }
}
