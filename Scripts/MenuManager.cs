using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject currentOpenMenu;
    public bool buildingMenuOpen;

    public static MenuManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(this.gameObject);
        }
    }

    void Update()
    {
        if (currentOpenMenu == null && buildingMenuOpen == true)
        {
            buildingMenuOpen = false;
        }
    }

    public bool MenuCheck()
    {
        bool open = buildingMenuOpen;
        return open;
    }

    public GameObject GetMenu()
    {
        GameObject menu = currentOpenMenu;
        return menu;
    }

    public void SetMenu(GameObject menu)
    {
        currentOpenMenu = menu;
        buildingMenuOpen = true;
    }
}
