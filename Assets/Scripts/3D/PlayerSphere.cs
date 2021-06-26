using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSphere : MonoBehaviour
{
    [SerializeField] SphereLevel Level;

    public static event System.Action<SphereLevel,Entity3D> OnEntityEnteredSphere;
    public static event System.Action<SphereLevel, Entity3D> OnEntityExitedSphere;

    void OnTriggerEnter(Collider other)
    {
        Entity3D entity = other.GetComponent<Entity3D>();
        if (entity != null && !entity.IgnoreInteraction)
        {
            entity.EnterSphere(Level);
            OnEntityEnteredSphere?.Invoke(Level, entity);
        }
    }

    void OnTriggerExit(Collider other)
    {
        Entity3D entity = other.GetComponent<Entity3D>();
        if (entity != null && !entity.IgnoreInteraction)
        {
            entity.ExitSphere(Level);
            OnEntityExitedSphere?.Invoke(Level, entity);
        }
    }
}
