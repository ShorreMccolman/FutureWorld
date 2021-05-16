using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortraitInfoMessenger : MonoBehaviour, IInfoMessenger
{
    [SerializeField] CharacterVitalsDisplay Display;

    public string GetInfoMessage()
    {
        return Display.Member.Profile.CharacterName + " the " + Display.Member.Profile.Class.ToString() + ": " + Display.Member.EffectiveStatusCondition();
    }
}
