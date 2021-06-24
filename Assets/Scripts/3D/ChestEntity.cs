using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestEntity : Entity3D
{
    Chest _chest;

    public static event System.Action<Chest> OnInspectChest;

    public void Setup(Chest chest)
    {
        _chest = chest;
        MouseoverName = chest.Data.MouseoverName;
    }

    public override IEnumerator Interact(PartyEntity party)
    {
        yield return new WaitForEndOfFrame();

        bool canOpen = true;
        if (_chest.Data.LockLevel > 0)
        {
            canOpen = Party.Instance.TryDisarm(_chest.Trap);
            if (canOpen)
            {
                Party.Instance.ActiveMember.Vitals.Express(GameConstants.EXPRESSION_HAPPY, GameConstants.EXPRESSION_HAPPY_DURATION);
            }
        }

        if (canOpen)
        {
            SoundManager.Instance.PlayUISound("Chest");
            OnInspectChest?.Invoke(_chest);
        }
    }

    public static void DebugInspect(Chest chest)
    {
        OnInspectChest?.Invoke(chest);
    }
}
