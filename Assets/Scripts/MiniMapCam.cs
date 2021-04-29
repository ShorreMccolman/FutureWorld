using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCam : MonoBehaviour
{
    [SerializeField] PartyEntity Entity;

    void Start()
    {
        transform.parent = transform.parent.parent;
    }

    // Update is called once per frame
    void Update()
    {
        if(Entity != null)
            transform.position = new Vector3(Entity.transform.position.x, 75.0f, Entity.transform.position.z);
    }
}
