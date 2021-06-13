using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBounce : MonoBehaviour
{
    [SerializeField] float Magnitude;
    [SerializeField] float Speed;
    [SerializeField] AnimationCurve Curve;
    [SerializeField] bool x, y, z;
    [SerializeField] bool Mirror;

    float progress;

    // Update is called once per frame
    void Update()
    {
        progress += Speed * Time.deltaTime;
        if (progress > 1)
            progress -= 1f;

        float mV = Mirror ? -Magnitude : Magnitude;
        float xV = x ? mV : 0;
        float yV = y ? mV : 0;
        float zV = z ? mV : 0;

        transform.localPosition = new Vector3(xV * Curve.Evaluate(progress), yV * Curve.Evaluate(progress), zV * Curve.Evaluate(progress));
    }
}
