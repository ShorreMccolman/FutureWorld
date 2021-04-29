using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSphere : MonoBehaviour
{
    public SphereLevel SphereLevel { get; protected set;}
    public Party Party { get; protected set; }

    public void Setup(Party party, SphereLevel level)
    {
        Party = party;
        SphereLevel = level;
    }
}
