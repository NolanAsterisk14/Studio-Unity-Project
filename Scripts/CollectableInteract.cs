using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableInteract : MonoBehaviour
{
    public GameObject collectableMenu; //set in inspector

    void Start()
    {
        if (collectableMenu == null)
        {
            collectableMenu = GameObject.Find("CollectMenu");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "Collectable")
                {
                    InstantiateMenu();
                }
            }
        }
    }

    void InstantiateMenu()
    {
        GameObject menu = collectableMenu;
        Instantiate(menu, menu.transform.position, menu.transform.rotation, menu.transform.parent);
    }
}
