using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHitProjectileSpell : ProjectileSpell
{
    protected override Projectile MakeProjectile(string name, int power, SkillProficiency proficiency)
    {
        Projectile proj = base.MakeProjectile(name, power, proficiency);

        int potency = 0;
        switch(proficiency)
        {
            case SkillProficiency.Novice:
                potency = power;
                break;
            case SkillProficiency.Expert:
                potency = power * 2;
                break;
            case SkillProficiency.Master:
                potency = power * 3;
                break;
        }

        proj.SetOnHitPotency(potency);
        return proj;
    }
}
