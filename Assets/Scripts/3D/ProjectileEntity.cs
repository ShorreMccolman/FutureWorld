using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEntity : Entity3D
{
    public Projectile Projectile { get { return State as Projectile; } }

    double lifetime;

    bool inMotion;

    public void Setup(Projectile projectile)
    {
        State = projectile;
        lifetime = 5.0f;
        inMotion = true;
        IgnoreInteraction = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if(!other.isTrigger)
        {
            if (Projectile.IsFriendly && other.tag == "Player")
                return;
            if (!Projectile.IsFriendly && other.tag == "Interactable")
                return;

            EnemyEntity enemy = other.GetComponent<EnemyEntity>();
            if(enemy != null)
            {
                bool hits = CombatHelper.ShouldHit(Projectile.Attack, enemy.Enemy.Data.ArmorClass);

                int damage = 0;
                if (hits)
                {
                    damage = Projectile.Damage;
                    float chanceOfReduction = 1 - 30 / (30 + enemy.Enemy.Data.Resistances.Physical);
                    damage = CombatHelper.ReduceDamage(damage, chanceOfReduction);

                }

                if (hits)
                {
                    string msg = Projectile.Sender + " shoots " + enemy.Enemy.Data.DisplayName + " for " + damage + " damage.";
                    if (enemy.Enemy.CurrentHP - damage <= 0)
                        msg = Projectile.Sender + " inflicts " + damage + " damage killing " + enemy.Enemy.Data.DisplayName + ".";

                    HUD.Instance.SendInfoMessage(msg, 2.0f);

                    enemy.Enemy.OnHit(damage);
                }

                lifetime = 1.0f;
            }

            inMotion = false;
            transform.parent = other.transform;
        }
    }

    void LateUpdate()
    {
        if(inMotion)
            transform.position += Projectile.Direction * Projectile.Speed * Time.fixedDeltaTime;

        lifetime -= Time.fixedDeltaTime;
        if(lifetime < 0)
        {
            Kill();
        }
    }
}
