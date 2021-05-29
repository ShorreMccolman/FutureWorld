using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCEntity : EnemyEntity
{
    public NPC NPC { get; protected set; }

    public void Setup(Enemy enemy, NPC npc)
    {
        Setup(enemy);
        NPC = npc;

        moveSpeed = 0.8f;

        IsTargetable = false;
    }

    public override IEnumerator Interact(PartyEntity party)
    {
        if (!IsAlive)
        {
            yield return base.Interact(party);
        } 
        else
        {
            yield return new WaitForEndOfFrame();

            HUD.Instance.Converse(NPC);
        }
    }
}
