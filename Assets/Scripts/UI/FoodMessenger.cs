using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodMessenger : MonoBehaviour, IInfoMessenger
{
    public string GetInfoMessage()
    {
        return "You have " + Party.Instance.CurrentFood + " food";
    }
}
