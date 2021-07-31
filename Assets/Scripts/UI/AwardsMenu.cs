using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AwardsMenu : CharacterMenu
{
    [SerializeField] Text NameLabel;

    public override void Setup(PartyMember member)
    {
        NameLabel.text = member.Profile.FullName;
    }
}
