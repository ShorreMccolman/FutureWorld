using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sparks : SpellBehaviour
{
    [SerializeField] string ProjectileID;

    public override float GetRecovery(InventorySkill skill) => 60f;

    protected override void OnCast(CombatEntity caster, int power, SkillProficiency proficiency)
    {
        Projectile projectile = ProjectileDatabase.Instance.GetProjectile(ProjectileID);
        int attack = int.MaxValue;

        int damage = 2 + power;

        projectile.SetDamage(DisplayName, attack, damage);

        PartyController.Instance.CastProjectile(projectile, null, proficiency == SkillProficiency.Novice ? 3 : proficiency == SkillProficiency.Expert ? 5 : 7 );
    }
}
