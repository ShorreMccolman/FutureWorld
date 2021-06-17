using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCEntity : EnemyEntity
{
    public override void Setup(Enemy enemy)
    {
        base.Setup(enemy);

        IsTargetable = false;
    }

    public override IEnumerator Interact(PartyEntity party)
    {
        if (!_isActive)
        {
            yield return base.Interact(party);
        } 
        else
        {
            yield return new WaitForEndOfFrame();

            HUD.Instance.Converse(Enemy.NPC);
        }
    }
}
