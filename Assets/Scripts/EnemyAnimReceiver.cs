using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimReceiver : MonoBehaviour
{
    Enemy _enemy;

    public void Setup(Enemy enemy)
    {
        _enemy = enemy;
    }

    public void OnAttackHit()
    {
        _enemy.DoAttack();
    }
}
