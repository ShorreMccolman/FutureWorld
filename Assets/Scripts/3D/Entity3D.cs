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

public abstract class Entity3D : MonoBehaviour
{
    public GameStateEntity State { get; protected set; }
    public string MouseoverName { get; protected set; }
    public bool IsTargetable { get; protected set; }
    public bool IgnoreInteraction { get; protected set; }

    protected bool _isVisible = false;

    public static event System.Action<Entity3D> OnEntityDestroyed;

    public virtual void EnterSphere(SphereLevel level)
    {
        if (level == SphereLevel.Three)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
            _isVisible = true;
        }
    }

    public virtual void ExitSphere(SphereLevel level)
    {
        if (level == SphereLevel.Three)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            _isVisible = false;
        }
    }

    public virtual IEnumerator Interact(PartyEntity party)
    {
        yield return null;
    }

    public void Kill()
    {
        OnEntityDestroyed?.Invoke(this);
        Destroy(this.gameObject);
    }
}
