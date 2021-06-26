using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveable
{
    Transform Transform { get; }
    float MovementSpeed { get; set; }

    void SetTargetDirection(Quaternion rotation);
}
