using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftButton : MonoBehaviour
{
    //This will be the only "button" script not directly placed on the button. place this on the building that instantiates the menu
    public int[] resourcesUsed = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };        //These arrays control the gain and loss from crafting
    public int[] resourcesGained = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    public int[] currentAmount = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };        //will reflect current values in inventory
    public int[] resourcesUsedDrone = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };   //This will control the cost of crafting a drone
    public Vector3 droneSpawnOffset = new Vector3(3f, 0f, 0f);                  //This offsets the drone when spawning
    public bool canCraft;                   //Toggle craft button interact
    public bool canCraftDrone;              //Constantly updated
    public bool canPlus;                    //Toggle plus interact
    public bool canMinus;                   //Toggle minus interact
    public bool timerActive;                //Toggle timer increment
    public bool isDroneWorkshop;            //Change expected menu buttons
    public int craftAmount = 1;             //Current amount to craft
    public int lastCraftAmount = 0;         //Last value if changed, to detect change
    public float craftTimer = 10f;          //Timer for individual components to craft in seconds
    public float currentTimer = 0f;         //Timer value to match the above
    public GameObject activeInteractMenu;
    public GameObject dronePrefab;          //Initialize if drone workshop
    public GameObject instantiatedDrone;
    public Button droneStartButton;
    public Button startButton;
    public Button plusButton;
    public Button minusButton;
    public Text amountText;
    public Text craftHeader;
    public Slider progressBar;
    BuildInteract buildInteractRef;

    void Start()
    {
        if (buildInteractRef == null)
        {
            if (TryGetComponent(out BuildInteract buildScript))
            {
                buildInteractRef = buildScript;
            }
        }
    }

    void Update()
    {
        if (buildInteractRef != null) //Get active menu to find refs
        {
            activeInteractMenu = buildInteractRef.activeMenu;
        }
        if (droneStartButton == null && activeInteractMenu != null && isDroneWorkshop == true)
        {
            droneStartButton = activeInteractMenu.transform.GetChild(0).GetChild(0).transform.Find("DroneStartButton").GetComponent<Button>();
            droneStartButton.onClick.AddListener(DroneCraft);
        }
        if (startButton == null && activeInteractMenu != null)
        {
            startButton = activeInteractMenu.transform.GetChild(0).GetChild(0).transform.Find("StartButton").GetComponent<Button>();
            startButton.onClick.AddListener(Craft);
        }
        if (plusButton == null && activeInteractMenu != null)
        {
            plusButton = activeInteractMenu.transform.GetChild(0).GetChild(0).transform.Find("PlusButton").GetComponent<Button>();
            plusButton.onClick.AddListener(Plus);
        }
        if (minusButton == null && activeInteractMenu != null)
        {
            minusButton = activeInteractMenu.transform.GetChild(0).GetChild(0).transform.Find("MinusButton").GetComponent<Button>();
            minusButton.onClick.AddListener(Minus);
        }
        if (amountText == null && activeInteractMenu != null)
        {
            amountText = activeInteractMenu.transform.GetChild(0).GetChild(0).transform.Find("CraftCount").GetComponent<Text>();
        }
        if (craftHeader == null && activeInteractMenu != null)
        {
            craftHeader = activeInteractMenu.transform.GetChild(0).GetChild(0).transform.Find("CraftHeader").GetComponent<Text>();
        }
        if (progressBar == null && activeInteractMenu != null)
        {
            progressBar = activeInteractMenu.transform.GetChild(0).GetChild(0).transform.Find("ProgressBar").GetComponent<Slider>();
        }

        currentAmount = InventoryManager.Instance.resourceAmount;
        int i = 0;
        foreach (int value in currentAmount) //check if resources are sufficient for crafting a drone
        {
            if (currentAmount[i] < resourcesUsedDrone[i])
            {
                canCraftDrone = false;
                break;
            }
            if (currentAmount[i] >= resourcesUsedDrone[i])
            {
                canCraftDrone = true;
            }
            i++;
        }

        //if (craftAmount != lastCraftAmount) //If the number changed, check these calculations
        //{
            int i2 = 0;
            int i3 = 0;
            foreach (int value in currentAmount) //check if resources are sufficient for crafting
            {
                if (currentAmount[i2] < (resourcesUsed[i2] * craftAmount))
                {
                    canCraft = false;
                    break;
                }
                if (currentAmount[i2] >= (resourcesUsed[i2] * craftAmount))
                {
                    canCraft = true;
                }
                i2++;
            }

            foreach (int value in currentAmount) //check if the next craft amount would be too expensive
            {
                if (currentAmount[i3] < (resourcesUsed[i3] * (craftAmount + 1)))
                {
                    canPlus = false;
                    break;
                }
                if (currentAmount[i3] >= (resourcesUsed[i3] * (craftAmount + 1)))
                {
                    canPlus = true;
                }
                i3++;
            }

            foreach (int value in currentAmount) //check if the next minus would put craft amount below 1
            {
                if ((craftAmount - 1) < 1)
                {
                    canMinus = false;
                    break;
                }
                if ((craftAmount - 1) >= 1)
                {
                    canMinus = true;
                }
            }
            //lastCraftAmount = craftAmount;
        //}

        if (plusButton != null)
        {
            plusButton.interactable = canPlus;
        }
        if (minusButton != null)
        {
            minusButton.interactable = canMinus;
        }
        if (startButton != null)
        {
            startButton.interactable = canCraft;
        }
        if (droneStartButton != null)
        {
            droneStartButton.interactable = canCraftDrone;
        }
        if (amountText != null)
        {
            amountText.text = craftAmount.ToString();
        }

        if (timerActive == true)
        {
            if (activeInteractMenu != null)
            {
                if (progressBar.gameObject.activeSelf == false || craftHeader.gameObject.activeSelf == true || amountText.gameObject.activeSelf == true)
                {
                    progressBar.gameObject.SetActive(true);
                    craftHeader.gameObject.SetActive(false);
                    amountText.gameObject.SetActive(false);
                    progressBar.maxValue = craftTimer * craftAmount;
                }
            }
            currentTimer += Time.deltaTime;
            if (progressBar != null)
            {
                progressBar.value = currentTimer;
            }
            
            if (currentTimer >= (craftTimer * craftAmount))
            {
                CraftComplete();
                timerActive = false;
            }
        }
        if (timerActive == false)
        {
            if (activeInteractMenu != null)
            {
                if (progressBar.gameObject.activeSelf == true || craftHeader.gameObject.activeSelf == false || amountText.gameObject.activeSelf == false)
                {
                    progressBar.gameObject.SetActive(false);
                    craftHeader.gameObject.SetActive(true);
                    amountText.gameObject.SetActive(true);
                }
            }
        }
    }

    void Craft()
    {
        progressBar.gameObject.SetActive(true);
        craftHeader.gameObject.SetActive(false);
        amountText.gameObject.SetActive(false);
        progressBar.maxValue = craftTimer * craftAmount;
        timerActive = true;
    }

    void DroneCraft()
    {
        for (int i = 0; i < 12; i++)
        {
            InventoryManager.Instance.Sub(i, resourcesUsedDrone[i]);
        }
        instantiatedDrone = Instantiate(dronePrefab, this.transform.position, dronePrefab.transform.rotation, this.transform.parent.parent);
        instantiatedDrone.transform.position += droneSpawnOffset;
    }

    void CraftComplete()
    {
        for (int i = 0; i < 12; i++)
        {
            InventoryManager.Instance.Add(i, (resourcesGained[i] * craftAmount));
        }
        for (int i = 0; i < 12; i++)
        {
            InventoryManager.Instance.Sub(i, (resourcesUsed[i] * craftAmount));
        }

        currentTimer = 0f;
        if (activeInteractMenu != null)
        {
            if (progressBar.gameObject.activeSelf == true || craftHeader.gameObject.activeSelf == false || amountText.gameObject.activeSelf == false)
            {
                progressBar.value = currentTimer;
                progressBar.gameObject.SetActive(false);
                craftHeader.gameObject.SetActive(true);
                amountText.gameObject.SetActive(true);
            }
        } 
    }

    void Plus()
    {
        craftAmount++;
    }

    void Minus()
    {
        craftAmount--;
    }
}
