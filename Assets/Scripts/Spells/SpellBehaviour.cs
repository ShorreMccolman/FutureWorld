using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
1   Fire   Torch Light   60   60   60
2   Fire   Flame Arrow   100   90   80
3   Fire   Protection from Fire   120   120   120
4   Fire   Fire Bolt   110   100   90
5   Fire   Haste   140   120   120
6   Fire   Fireball   110   100   90
7   Fire   Ring of Fire   110   100   90
8   Fire   Fire Blast   110   90   70
9   Fire   Meteor Shower   120   110   100
10   Fire   Inferno   140   120   100
11   Fire   Incinerate   150   130   110
12   Air   Wizard Eye   60   60   60
13   Air   Static Charge   100   90   80
14   Air   Protection from Electricity   120   120   120
15   Air   Sparks   110   100   90
16   Air   Feather Fall   120   120   120
17   Air   Shield   140   120   120
18   Air   Lightning Bolt   110   100   90
19   Air   Jump   110   90   70
20   Air   Implosion   120   110   100
21   Air   Fly   250   250   250
22   Air   Starburst   150   130   110
23   Water   Awaken   60   60   60
24   Water   Cold Beam   90   80   80
25   Water   Protection from Cold   120   120   120
26   Water   Poison Spray   110   100   90
27   Water   Water Walk   150   120   120
28   Water   Ice Bolt   110   100   90
29   Water   Enchant Item   140   140   140
30   Water   Acid Burst   110   100   90
31   Water   Town Portal   200   200   200
32   Water   Ice Blast   120   110   100
33   Water   Lloyd's Beacon   250   250   250
34   Earth   Stun   80   80   80
35   Earth   Magic Arrow   100   90   80
36   Earth   Protection from Magic   120   120   120
37   Earth   Deadly Swarm   110   100   90
38   Earth   Stone Skin   140   120   120
39   Earth   Blades   110   100   90
40   Earth   Stone to Flesh   140   140   140
41   Earth   Rock Blast   110   100   90
42   Earth   Turn to Stone   130   130   130
43   Earth   Death Blossom   120   110   100
44   Earth   Mass Distortion   140   120   100
45   Spirit   Spirit Arrow   90   80   80
46   Spirit   Bless   140   100   100
47   Spirit   Healing Touch   140   140   140
48   Spirit   Lucky Day   140   140   120
49   Spirit   Remove Curse   140   140   140
50   Spirit   Guardian Angel   120   120   120
51   Spirit   Heroism   140   120   120
52   Spirit   Turn Undead   140   120   100
53   Spirit   Raise Dead   240   240   240
54   Spirit   Shared Life   150   150   150
55   Spirit   Resurrection   1000   1000   1000
56   Mind   Meditation   140   140   120
57   Mind   Remove Fear   140   140   140
58   Mind   Mind Blast   110   100   90
59   Mind   Precision   140   140   120
60   Mind   Cure Paralysis   140   140   140
61   Mind   Charm   100   100   100
62   Mind   Mass Fear   100   90   80
63   Mind   Feeblemind   110   100   90
64   Mind   Cure Insanity   140   140   140
65   Mind   Psychic Shock   130   120   110
66   Mind   Telekinesis   250   250   250
67   Body   Cure Weakness   140   140   140
68   Body   First Aid   140   140   140
69   Body   Protection from Poison   120   120   120
70   Body   Harm   110   100   90
71   Body   Cure Wounds   140   140   140
72   Body   Cure Poison   140   140   140
73   Body   Speed   140   140   120
74   Body   Cure Disease   140   140   140
75   Body   Power   140   140   120
76   Body   Flying Fist   130   120   110
77   Body   Power Cure   150   125   100
78   Light   Create Food   100   90   90
79   Light   Golden Touch   100   100   100
80   Light   Dispel Magic   100   90   90
81   Light   Slow   120   100   80
82   Light   Destroy Undead   120   110   100
83   Light   Day of the Gods   500   500   500
84   Light   Prismatic Light   150   135   120
85   Light   Hour of Power   250   250   250
86   Light   Paralyze   160   140   120
87   Light   Sun Ray   180   165   150
88   Light   Divine Intervention   200   200   200
89   Dark   Reanimate   100   100   100
90   Dark   Toxic Cloud   120   110   100
91   Dark   Mass Curse   120   120   120
92   Dark   Shrapmetal   100   90   80
93   Dark   Shrinking Ray   120   120   120
94   Dark   Day of Protection   500   500   500
95   Dark   Finger of Death   130   130   130
96   Dark   Moon Ray   150   140   130
97   Dark   Dragon Breath   160   140   130
98   Dark   Armageddon   250   250   250
99   Dark   Dark Containment   300   300   300
*/

[System.Serializable]
public class SpellBehaviour : MonoBehaviour
{
    protected CombatEntity _caster;
    public string GetCasterName => _caster.GetName();

    protected int _potency;
    protected SkillProficiency _proficiency;

    public virtual bool IsRanged => false;
    public virtual bool TargetsFriendly => false;
    public virtual float GetRecovery(SkillProficiency proficiency) => 100f;

    public virtual bool IsTargetValid(CombatEntity target) { return false; }
    public virtual int AdjustCost(int cost, InventorySkill skill) { return cost; }
    public void Cast(CombatEntity caster, InventorySkill skill) 
    {
        _caster = caster;
        _potency = skill.Level;
        _proficiency = skill.Proficiency;

        OnCast();
    }

    protected virtual void OnCast() { }
    public virtual void CastFinal(CombatEntity target) { }
}
