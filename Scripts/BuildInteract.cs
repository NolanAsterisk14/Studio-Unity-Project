using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildInteract : MonoBehaviour
{
    public Building building;
    public GameObject activeMenu;
    public bool isAnotherOpen;
    public bool isThisOpen;
    public bool isMouseDown;
    


    void Update()
    {
        isAnotherOpen = MenuManager.Instance.MenuCheck();
        isMouseDown = Input.GetMouseButtonDown(0);
        if (activeMenu == null)
        {
            isThisOpen = false;
        }
        if (isMouseDown == true && isThisOpen == false && isAnotherOpen == false)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            int layerMask = 1 << 0;
            if (Physics.Raycast(ray, out hit, 300f, layerMask, QueryTriggerInteraction.Ignore))
            {
                if (hit.transform.gameObject == this.gameObject)
                {
                    InstantiateMenu();
                }    

            }
        }
    }


    private void InstantiateMenu()
    {
        GameObject menu;
        menu = building.interactMenu;
        activeMenu = Instantiate(menu, menu.transform.position, menu.transform.rotation, menu.transform.parent);
        MenuManager.Instance.SetMenu(activeMenu);
        isThisOpen = true;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6 && other.gameObject.tag == "Untagged")
        {
            other.gameObject.tag = "BuildableHex";
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 6 && other.gameObject.tag == "BuildableHex")
        {
            other.gameObject.tag = "Untagged";
        }
    }
}
