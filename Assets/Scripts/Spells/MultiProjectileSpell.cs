using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiProjectileSpell : ProjectileSpell
{
    [SerializeField] int NoviceShots;
    [SerializeField] int ExpertShots;
    [SerializeField] int MasterShots;

    [SerializeField] int BaseDamage;

    protected override void OnCast(CombatEntity caster, int power, SkillProficiency proficiency)
    {
        Projectile projectile = ProjectileDatabase.Instance.GetProjectile(ProjectileID);
        int attack = int.MaxValue;

        int damage = BaseDamage;
        for(int i=0;i<power;i++)
        {
            damage += Roll.Roll();
        }

        projectile.SetDamage(DisplayName, attack, damage);

        PartyController.Instance.CastProjectile(projectile, null, proficiency == SkillProficiency.Novice ? NoviceShots : proficiency == SkillProficiency.Expert ? ExpertShots : MasterShots);
    }
}
