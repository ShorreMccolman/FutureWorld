using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    Camera _cam;

    // Start is called before the first frame update
    void Start()
    {
        _cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (HUD.Instance.HeldItemButton != null && Input.GetMouseButtonDown(0) && !MenuManager.Instance.IsMenuOpen())
        {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.CompareTag("Terrain"))
                {
                    HUD.Instance.DropItem();
                }
            }
        }
    }
}
