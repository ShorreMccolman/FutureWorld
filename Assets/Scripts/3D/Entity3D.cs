using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SphereLevel
{
    Zero = 0,
    One = 1,
    Two = 2,
    Three = 3
}

public class Entity3D : MonoBehaviour
{
    public GameStateEntity State { get; protected set; }

    public string MouseoverName { get; protected set; }
    public bool IsTargetable { get; protected set; }
    public bool IgnoreInteraction { get; protected set; }

    void Start()
    {
        GameController.Instance.EntityUpdate += EntityUpdate;
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerSphere entity = other.GetComponent<PlayerSphere>();
        if (entity != null && !IgnoreInteraction)
        {
            OnEnterSphere(entity.Party, entity.SphereLevel);
        }
    }

    void OnTriggerExit(Collider other)
    {
        PlayerSphere entity = other.GetComponent<PlayerSphere>();
        if (entity != null && !IgnoreInteraction)
        {
            OnExitSphere(entity.Party, entity.SphereLevel);
        }
    }

    public virtual void OnEnterSphere(Party party, SphereLevel level)
    {
        if (IgnoreInteraction)
            return;

        if(level == SphereLevel.Zero)
        {
            PartyController.Instance.ShortRange(this, true);
        }
        else if (level == SphereLevel.One)
        {
            PartyController.Instance.MidRange(this, true);
        } 
        else if (level == SphereLevel.Two)
        {
            PartyController.Instance.LongRange(this, true);
        }
    }

    public virtual void OnExitSphere(Party party, SphereLevel level)
    {
        if (IgnoreInteraction)
            return;

        if (level == SphereLevel.Zero)
        {
            PartyController.Instance.ShortRange(this, false);
        }
        else if (level == SphereLevel.One)
        {
            PartyController.Instance.MidRange(this, false);
        }
        else if (level == SphereLevel.Two)
        {
            PartyController.Instance.LongRange(this, false);
        }
    }

    public virtual void EntityUpdate()
    {

    }

    public virtual IEnumerator Interact(PartyEntity party)
    {
        yield return new WaitForEndOfFrame();
    }

    protected void Kill()
    {
        PartyController.Instance.ShortRange(this, false);
        PartyController.Instance.MidRange(this, false);

        GameController.Instance.EntityUpdate -= EntityUpdate;
        Destroy(this.gameObject);
    }
}
