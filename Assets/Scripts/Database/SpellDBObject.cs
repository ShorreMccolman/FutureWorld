using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SpellDBObject : ScriptableObject
{
    [SerializeField] SpellSchool School;
    public SpellSchool GetSchool() { return School; }

    [SerializeField] SpellData[] Spells;
    public SpellData[] GetSpells() { return Spells; }
}
