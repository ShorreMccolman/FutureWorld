using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LODSettings : MonoBehaviour
{
    [SerializeField] float Size;

    private void Awake()
    {
        LODGroup[] lods = GetComponentsInChildren<LODGroup>();
        foreach(var lod in lods)
        {
            lod.size = Size;
        }
    }
}
