using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SkillDBObject : ScriptableObject
{
    public Skill[] WeaponSkills;
    public Skill[] ArmorSkills;
    public Skill[] MagicSkills;
    public Skill[] MiscSkills;
}
