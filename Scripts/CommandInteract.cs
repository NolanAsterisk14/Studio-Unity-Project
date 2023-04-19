using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandInteract : MonoBehaviour
{
    public Building building;
    public GameObject HUDCanvas;
    public GameObject activeMenu;
    public bool menuOpen;
    public bool otherOpen;
    public bool isMouseDown;

    //public static CommandInteract Instance { get; private set; }

    /*private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(this.gameObject);
        }
    }*/

    void Start()
    {
        if (GameObject.Find("HUDCanvas") != null)
        {
            HUDCanvas = GameObject.Find("HUDCanvas");
        }
    }

    void Update()
    {
        isMouseDown = Input.GetMouseButtonDown(0);
        otherOpen = MenuManager.Instance.MenuCheck();

        if (isMouseDown == true && menuOpen == false && otherOpen == false)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            int layerMask = 1 << 0;

            if (Physics.Raycast(ray, out hit, 300f, layerMask ,QueryTriggerInteraction.Ignore))
            {
                if (hit.transform.gameObject == this.gameObject)
                {
                    InstantiateMenu();
                }
            }
        }
        if (activeMenu == null)
        {
            menuOpen = false;
        }
        if (activeMenu != null)
        {
            menuOpen = true;
        }
    }

    private void InstantiateMenu()
    {
        GameObject menu;
        menu = building.interactMenu;
        activeMenu = Instantiate(menu, menu.transform.position, menu.transform.rotation, menu.transform.parent);
        MenuManager.Instance.SetMenu(activeMenu);
        menuOpen = true;
    }

    public GameObject GetMenuObject()
    {
        GameObject menuObject = activeMenu;
        return menuObject;
    }

    public bool GetMenuOpen()
    {
        bool isMenuOpen = menuOpen;
        return isMenuOpen;
    }

    /*public void DestroyMenu()
    {
        if(activeMenu != null)
        {
            Destroy(activeMenu, 0);
        }
        menuOpen = false;
    }*/

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
