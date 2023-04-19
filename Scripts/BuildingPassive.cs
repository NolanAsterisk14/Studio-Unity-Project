using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPassive : MonoBehaviour
{
    //This script is basically frankenstein's monster brought to life in C#
    public int[] resourcesUsed = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };    //These arrays will control amounts of gain and loss by building passive
    public int[] resourcesGained = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    public int[] storageBonus = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };     //This array dictates the storage provided by this building
    public int[] upgradeCostIndex;              //Gotten from building object
    public int[] upgradeCostAmount;             //*
    public int[] repairCostIndex;               //Derived from current upgrade
    public int[] repairCostAmount;              //*
    public int[] currentAmounts;                //Used for cost calculating, gotten from inventorymanager
    public int currentTier;                     //Current tier from tiermanager
    public int currentUpgrade = 0;              //Current tier this building has reached
    public float currentTime;                   //Current time from timemanager
    public float passiveCycle = 30f;            //Cycle amount of time for timer
    public float currentCycle = 0f;             //Time contained by current cycle
    public float droneSearchCycle = 10f;        //Cycle amount of time for timer
    public float currentSearchCycle = 0f;       //Time of current search cycle timer
    public float nameTextOffset = 6f;               //Y-axis offset for name text instantiation
    public bool passiveActive = true;           //Should the passive be working?
    public bool passiveStorage;                 //Should the passive add storage?
    public bool passiveGain;                    //Should the passive cause gain of a resource?
    public bool canPassiveContinue;             //Can the passive continue past this cycle?
    public bool destroyed;                      //Has this building been destroyed by a disaster?
    public bool repairAvailable;                //Does this building need repairs?
    public bool canRepair;                      //Check cost of repairs
    public bool techCenter;                     //Is this building a tech center?
    public bool commandAbleT3;                  //Special flag used by tech center to tell the command center it can upgrade.
    public bool craftBuilding;                  //Does this building have a crafting menu?
    public bool droneWorkshop;                  //Is this building the drone workshop?
    public bool disasterWarningT2;              //Is this building the disaster warning system t2?
    public bool disasterWarningT3;              //Is this building the disaster warning system t3?
    public bool addLast = false;                //Flip flop flags for storage changes
    public bool subLast = true;                 //*
    public bool updateOnOpen;                   //Flag for updating text to reflect passive
    public bool upgradeAvailable;               //Flag used in checking if an upgrade is available when a menu opens
    public bool canUpgrade;                     //Flag for checking the cost of the upgrade
    public bool justUpgraded = false;           //Set when upgrading so the upgrade check in start doesn't mess up
    public bool upgradeTransferred = false;     //Flag for checking if the upgrade was transferred to building on being built
    public bool upgradeButtonListener;          //Flag for checking if the listener was added to the upgrade button
    public bool repairButtonListener;           //Flag for checking if the listener was added to the repair button
    public string nameString;                   //String that will update with building upgrade
    public string descString;                   //String that will update with building upgrade
    public Button onButton;                     //Buttons for enabling and disabling passives
    public Button offButton;                    //*
    public Button upgradeButton;                //Button on each menu to be located
    public Button repairButton;                 //Button on each menu to be located
    public Text statusText;                     //Text for displaying passive status
    public Text nameText;                       //Text for displaying name
    public Text descText;                       //Text for displaying description
    public Text[] changingTexts;                //Texts that will change color with tier
    public Image backgroundImage;               //Background image that will change color with tier
    public GameObject activeInteractMenu;       //Will store the current active menu from the buildinteract script on same obj
    public GameObject newModel;                 //Used in the upgrade method
    public GameObject currentHex;               //Used for finding anchor, and for outline *set by buildbutton*
    public GameObject smokeParticlePrefab;      //Prefab particle system for building when damaged
    public GameObject[] activeDrones;           //Auto updates if drone workshop bool is true
    public List<GameObject> commandButtons;     //Updates when command center opens, for changing sprites
    public GameObject currentParticleSystem;    //Current particle system instantiated
    public GameObject nameTextPrefab;           //Used for mouseover
    public GameObject nameTextCurrent;          //Used for mouseover
    public AudioSource buildingAudio;           //AudioSource for the building
    public AudioClip damagedSFX;                //Audio for building damaged
    public AudioClip repairedSFX;               //Audio for repairing damage
    public Color onColor = new Color(0.3726415f, 1f, 0.482348f, 1f);    //Colors for text
    public Color offColor = new Color(1f, 0.3915094f, 0.3915094f, 1f);
    public Color t1BackColor = new Color (0.05882353f, 0.3803922f, 0.5764706f, 1f);
    public Color t2BackColor = new Color (0.4169811f, 0.239053f, 0.6415094f, 1f);
    public Color t3BackColor = new Color (0.6226415f, 0.5550652f, 0.06755074f, 1f);
    public Color t1TextColor = new Color (0.1607843f, 0.6705883f, 0.8862746f, 1f);
    public Color t2TextColor = new Color(0.6518289f, 0.5226504f, 0.8207547f, 1f);
    public Color t3TextColor = new Color(0.6313726f, 0.5764706f, 0.1803922f, 1f);
    public Color textOnColor = new Color(0f, 0.8018868f, 0.645f, 1f);
    public Color textBackgroundOnColor = new Color(0f, 0.8018868f, 0.645f, 0.1215686f);
    public Color textOffColor = new Color(1f, 0f, 0f, 1f);
    public Color textBackgroundOffColor = new Color(1f, 0f, 0f, 0.1215686f);
    public Building buildingObject;             //For the upgrade cost values
    public BuildInteract buildInteractRef;
    public CommandInteract commandInteractRef;
    public CraftButton craftButtonRef;
    public Outline outlineRef;

    void Start()
    {
        if (buildInteractRef == null)
        {
            if (TryGetComponent(out BuildInteract buildScript))
            {
                buildInteractRef = buildScript;
            }
        }
        if (commandInteractRef == null)
        {
            if (TryGetComponent(out CommandInteract commandScript))
            {
                commandInteractRef = commandScript;
            }
        }
        if (buildInteractRef != null)
        {
            buildingObject = buildInteractRef.building;
        }
        if (commandInteractRef != null)
        {
            buildingObject = commandInteractRef.building;
        }
        if (outlineRef == null && currentHex != null)
        {
            if (currentHex.GetComponent<Outline>() == null)
            {
                outlineRef = currentHex.AddComponent<Outline>();
                outlineRef.enabled = false;
            }    
        }
        if (onButton != null) 
        {
            onButton.onClick.AddListener(PassiveOn);
        }
        if (offButton != null)
        {
            offButton.onClick.AddListener(PassiveOff);
        }
        if (techCenter == true)
        {
            TierHandler.Instance.Tier1Reached();
        }
        activeDrones = GameObject.FindGameObjectsWithTag("Drone");
        buildingAudio = this.GetComponent<AudioSource>();
        damagedSFX = Resources.Load<AudioClip>("Audio/DestructionSound");
        repairedSFX = Resources.Load<AudioClip>("Audio/Construction");
        smokeParticlePrefab = Resources.Load<GameObject>("ParticleSystems/SmokeParticleSystem");
        nameTextPrefab = Resources.Load<GameObject>("Prefabs/NameText");
    }

    void Update()
    {
        currentTime = TimeManager.Instance.CurrentTime(); //Didn't use this for a whole lot but if it ain't broke don't fix it
        currentTier = TierHandler.Instance.TierCheck(); //Get current tech center tier from the tier handler
        currentAmounts = InventoryManager.Instance.resourceAmount; //make sure current amounts are updated
        if (buildInteractRef == null)
        {
            if (TryGetComponent(out BuildInteract buildScript))
            {
                buildInteractRef = buildScript;
            }
        }
        if (commandInteractRef == null)
        {
            if (TryGetComponent(out CommandInteract commandScript))
            {
                commandInteractRef = commandScript;
            }
        }
        if (commandInteractRef != null)
        {
            commandAbleT3 = TierHandler.Instance.commandAbleT3;
        }
        if (nameTextCurrent != null)
        {
            nameTextCurrent.transform.LookAt(2 * nameTextCurrent.transform.position - Camera.main.transform.position, Vector3.up);
        }    
        switch (currentUpgrade) //change repair costs here
        {
            case 0:
            case 1:
                repairCostIndex = new int[] { 6 };
                repairCostAmount = new int[] { 3 };
                break;
            case 2:
                repairCostIndex = new int[] { 11 };
                repairCostAmount = new int[] { 2 };
                break;
            case 3:
                repairCostIndex = new int[] { 10, 11 };
                repairCostAmount = new int[] { 1, 3 };
                break;
        }
        if (buildInteractRef != null && justUpgraded == false && upgradeTransferred == false || commandInteractRef != null && justUpgraded == false && upgradeTransferred == false)
        {
            switch (currentTier)
            {
                case 0:
                case 1:
                    currentUpgrade = 1;
                    break;
                case 2:
                    currentUpgrade = 2;
                    break;
                case 3:
                    currentUpgrade = 3;
                    break;
                default:
                    Debug.LogWarning("BuildingPassive messed up the upgrade transferring");
                    break;
            }
            upgradeTransferred = true;
        }
        if (droneWorkshop == true)
        {
            if (currentSearchCycle < droneSearchCycle)
            {
                currentSearchCycle += Time.deltaTime;
            }
            if (currentSearchCycle >= droneSearchCycle)
            {
                currentSearchCycle = 0f;
                activeDrones = GameObject.FindGameObjectsWithTag("Drone");
            }
            if (activeDrones.Length > 0)
            {
                foreach (GameObject drone in activeDrones)
                {
                    if (passiveActive == true)
                    {
                        switch (currentUpgrade) //change repair costs based on upgrade lvl of building
                        {
                            case 0:
                            case 1:
                                drone.GetComponent<DroneController>().speedL1 = true;
                                drone.GetComponent<DroneController>().speedL2 = false;
                                drone.GetComponent<DroneController>().speedL3 = false;
                                break;
                            case 2:
                                drone.GetComponent<DroneController>().speedL1 = false;
                                drone.GetComponent<DroneController>().speedL2 = true;
                                drone.GetComponent<DroneController>().speedL3 = false;
                                break;
                            case 3:
                                drone.GetComponent<DroneController>().speedL1 = false;
                                drone.GetComponent<DroneController>().speedL2 = false;
                                drone.GetComponent<DroneController>().speedL3 = true;
                                break;
                        }
                    }
                    if (passiveActive == false)
                    {
                        drone.GetComponent<DroneController>().speedL1 = false;
                        drone.GetComponent<DroneController>().speedL2 = false;
                        drone.GetComponent<DroneController>().speedL3 = false;
                    }
                }
            }
        }
        if (outlineRef == null)
        {
            if (currentHex != null)
            {
                if (currentHex.TryGetComponent(out Outline outlineTemp))
                {
                    outlineRef = outlineTemp;
                }
            }
        }
        if (destroyed == true)
        {
            repairAvailable = true;
        }
        if (destroyed == false)
        {
            repairAvailable = false;
        }
        if (repairAvailable == true) //cost check here
        {
            for (int i = 0; i < repairCostIndex.Length; i++) //cost check loop
            {
                if (currentAmounts[repairCostIndex[i]] < repairCostAmount[i])
                {
                    canRepair = false;
                    break;
                }    
                if (currentAmounts[repairCostIndex[i]] >= repairCostAmount[i])
                {
                    canRepair = true;
                }    
            }
        }
        if (craftBuilding == true && craftButtonRef == null) //Get craft building script here if it is one
        {
            if (TryGetComponent(out CraftButton craftScript))
            {
                craftButtonRef = craftScript;
            }
        }
        if (craftButtonRef != null)
        {
            craftButtonRef.canCraft = passiveActive;
        }
        if (techCenter == true && currentUpgrade == 2) //Made TierHandler do work for me here at start of update
        {
            TierHandler.Instance.CommandCanT3();
        }
        if (disasterWarningT2 == true)
        {
            DisasterManager.Instance.WarningBuildingActiveT2(passiveActive);
        }
        if (disasterWarningT3 == true)
        {
            DisasterManager.Instance.WarningBuildingActiveT3(passiveActive);
        }
        if (buildInteractRef != null) //Updates menu to get buttons if it's open, and get building object
        {
            activeInteractMenu = buildInteractRef.activeMenu;
        }
        if (commandInteractRef != null)
        {
            activeInteractMenu = commandInteractRef.activeMenu;
        }
        if (buildingObject != null)
        {
            switch (currentUpgrade) //update the current building name based on upgrade lvl of building
            {
                case 0:
                case 1:
                    upgradeCostIndex = buildingObject.t1upgradeCostIndex;
                    upgradeCostAmount = buildingObject.t1upgradeCostAmount;
                    nameString = buildingObject.t1name;
                    descString = buildingObject.t1description;
                    break;
                case 2:
                    upgradeCostIndex = buildingObject.t2upgradeCostIndex;
                    upgradeCostAmount = buildingObject.t2upgradeCostAmount;
                    nameString = buildingObject.t2name;
                    descString = buildingObject.t2description;
                    break;
                case 3:
                    upgradeCostIndex = null;
                    upgradeCostAmount = null;
                    nameString = buildingObject.t3name;
                    descString = buildingObject.t3description;
                    break;
                default:
                    Debug.LogWarning("Upgrade cost switching messed up in BuildingPassive");
                    break;
            }
            
        }
        if (activeInteractMenu != null && upgradeButton == null)
        {
            upgradeButton = activeInteractMenu.transform.GetChild(0).GetChild(0).transform.Find("UpgradeButton").gameObject.GetComponent<Button>();
        }
        if (activeInteractMenu != null && repairButton == null && buildInteractRef != null)
        {
            repairButton = activeInteractMenu.transform.GetChild(0).GetChild(0).transform.Find("RepairButton").gameObject.GetComponent<Button>();
        }
        if (upgradeButton == null)
        {
            upgradeButtonListener = false;
        }
        if (repairButton == null)
        {
            repairButtonListener = false;
        }
        if (upgradeButton != null)
        {
            if (upgradeButton.GetComponent<UpgradeButton>() != null)
            {
                upgradeButton.GetComponent<UpgradeButton>().uCostIndex = upgradeCostIndex;
                upgradeButton.GetComponent<UpgradeButton>().uCostAmount = upgradeCostAmount;
                upgradeButton.GetComponent<UpgradeButton>().canAfford = canUpgrade;
            }
            upgradeButton.gameObject.SetActive(upgradeAvailable);    
            if (currentTier == 0 && commandInteractRef != null)
            {
                upgradeButton.gameObject.SetActive(false);
            }
        }
        if (upgradeButton != null && upgradeButtonListener == false)
        {
            upgradeButton.onClick.AddListener(Upgrade);
            upgradeButtonListener = true;
        }
        if (repairButton != null)
        {
            repairButton.GetComponent<RepairButton>().uCostIndex = repairCostIndex;
            repairButton.GetComponent<RepairButton>().uCostAmount = repairCostAmount;
            repairButton.GetComponent<RepairButton>().canAfford = canRepair;
            repairButton.gameObject.SetActive(repairAvailable);
        }
        if (repairButton != null && repairButtonListener == false)
        {
            repairButton.onClick.AddListener(Repair);
            repairButtonListener = true;
        }
        if (buildInteractRef != null)
        {
            if (currentUpgrade < currentTier)
            {
                upgradeAvailable = true;
            }
            if (currentUpgrade == currentTier)
            {
                upgradeAvailable = false;
            }
        }
        if (commandInteractRef != null)
        {
            if (currentUpgrade == currentTier && currentTier == 1)
            {
                upgradeAvailable = true;
            }
            if (currentUpgrade == currentTier && currentTier == 2 && commandAbleT3 == false)
            {
                upgradeAvailable = false;
            }
            if (currentUpgrade == currentTier && currentTier == 2 && commandAbleT3 == true)
            {
                upgradeAvailable = true;
            }
            if (currentUpgrade == currentTier && currentTier == 3)
            {
                upgradeAvailable = false;
            }
        }
        if (activeInteractMenu != null && upgradeButton != null && buildingObject != null && currentUpgrade < 3) //This is where the cost is calculated for the upgrade button
        {
            currentAmounts = InventoryManager.Instance.resourceAmount;
            for (int i = 0; i < upgradeCostIndex.Length; i++)
            {
                if (currentAmounts[upgradeCostIndex[i]] < upgradeCostAmount[i])
                {
                    canUpgrade = false;
                    break;
                }
                if (currentAmounts[upgradeCostIndex[i]] >= upgradeCostAmount[i])
                {
                    canUpgrade = true;
                }
            }
            if (upgradeButton.GetComponent<UpgradeButton>() != null)
            {
                upgradeButton.GetComponent<UpgradeButton>().canAfford = canUpgrade;
            }
        }
        if (activeInteractMenu == null && updateOnOpen != true)
        {
            updateOnOpen = true;
        }
        if (activeInteractMenu != null && updateOnOpen == true && buildInteractRef != null)
        {
            if (nameText != null)
            {
                nameText.text = nameString;
            }
            if (descText != null)
            {
                descText.text = descString;
            }
            if (statusText != null && passiveActive == true)
            {
                statusText.text = "On";
                statusText.color = onColor;
                updateOnOpen = false;
            }
            if (statusText != null && passiveActive == false)
            {
                statusText.text = "Off";
                statusText.color = offColor;
                updateOnOpen = false;
            }
        }
        if (activeInteractMenu != null && updateOnOpen == true && commandInteractRef != null)
        {
            Transform transform = activeInteractMenu.transform.GetChild(0).GetChild(0).transform.Find("ListBody").Find("ScrollRect").Find("ListItems");
            foreach (Transform child in transform)
            {
                commandButtons.Add(child.gameObject);     
            }
            foreach (GameObject button in commandButtons)
            {
                Building buildObj = button.GetComponent<BuildButton>().buildingObject;
                if (button.GetComponent<Image>() != null && buildObj != null && buildObj.t2name == "Weather Station")
                {
                    if (currentTier < 2)
                    {
                        button.GetComponent<Button>().interactable = false;
                        button.GetComponent<BuildButton>().enabled = false;
                    }
                    if (currentTier >= 2)
                    {
                        button.GetComponent<Button>().interactable = true;
                        button.GetComponent<BuildButton>().enabled = true;
                    }
                }    
                if (button.GetComponent<Image>() != null && buildObj != null)
                {
                    switch (currentUpgrade)
                    {
                        case 0:
                        case 1:
                            button.GetComponent<Image>().sprite = buildObj.t1thumbnail;
                            break;
                        case 2:
                            button.GetComponent<Image>().sprite = buildObj.t2thumbnail;
                            break;
                        case 3:
                            button.GetComponent<Image>().sprite = buildObj.t3thumbnail;
                            break;
                    }
                }
            }
            commandButtons.Clear();
            updateOnOpen = false;
        }
        if (changingTexts == null && activeInteractMenu != null || backgroundImage == null && activeInteractMenu != null || nameText == null && activeInteractMenu != null) //If the texts and image are null, and a menu opens, get colorful with it
        {
            Color textColor = new Color();
            Color backColor = new Color();
            switch (currentUpgrade)
            {
                case 0:
                case 1:
                    textColor = t1TextColor;
                    backColor = t1BackColor;
                    break;
                case 2:
                    textColor = t2TextColor;
                    backColor = t2BackColor;
                    break;
                case 3:
                    textColor = t3TextColor;
                    backColor = t3BackColor;
                    break;
                default:
                    Debug.LogWarning("Tier out of expected range in building passive");
                    break;
            }
            backgroundImage = activeInteractMenu.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>(); //Background here
            backgroundImage.color = backColor;
            if (passiveGain == true && craftBuilding == false || techCenter == true) //If building provides a passive, it needs 3 array spaces
            {
                changingTexts = new Text[3];
                changingTexts[0] = activeInteractMenu.transform.GetChild(0).GetChild(0).transform.Find("NameText").gameObject.GetComponent<Text>();
                changingTexts[1] = activeInteractMenu.transform.GetChild(0).GetChild(0).transform.Find("StatusHeader").gameObject.GetComponent<Text>();
                changingTexts[2] = activeInteractMenu.transform.GetChild(0).GetChild(0).transform.Find("TextBackground").transform.GetChild(0).gameObject.GetComponent<Text>();
                foreach(Text text in changingTexts)
                {
                    text.color = textColor;
                }
            }
            if (craftBuilding == true) //If a building has a crafting menu instead, it needs 4
            {
                changingTexts = new Text[4];
                changingTexts[0] = activeInteractMenu.transform.GetChild(0).GetChild(0).transform.Find("NameText").gameObject.GetComponent<Text>();
                changingTexts[1] = activeInteractMenu.transform.GetChild(0).GetChild(0).transform.Find("StatusHeader").gameObject.GetComponent<Text>();
                changingTexts[2] = activeInteractMenu.transform.GetChild(0).GetChild(0).transform.Find("TextBackground").transform.GetChild(0).gameObject.GetComponent<Text>();
                changingTexts[3] = activeInteractMenu.transform.GetChild(0).GetChild(0).transform.Find("CraftHeader").gameObject.GetComponent<Text>();
                foreach (Text text in changingTexts)
                {
                    text.color = textColor;
                }
            }
            if (commandInteractRef != null) //For the command center to have the color change effect
            {
                changingTexts = new Text[1];
                changingTexts[0] = activeInteractMenu.transform.GetChild(0).GetChild(0).transform.Find("NameText").gameObject.GetComponent<Text>();
                foreach (Text text in changingTexts)
                {
                    text.color = textColor;
                }
            }
        }
        if (passiveGain == true && onButton == null && activeInteractMenu != null) //on and off buttons
        {
            onButton = activeInteractMenu.transform.GetChild(0).GetChild(0).transform.Find("OnButton").GetComponent<Button>();
            onButton.onClick.AddListener(PassiveOn);
        }
        if (passiveGain == true && offButton == null && activeInteractMenu != null)
        {
            offButton = activeInteractMenu.transform.GetChild(0).GetChild(0).transform.Find("OffButton").GetComponent<Button>();
            offButton.onClick.AddListener(PassiveOff);
        }
        if (passiveGain == true && statusText == null && activeInteractMenu != null)
        {
            statusText = activeInteractMenu.transform.GetChild(0).GetChild(0).transform.Find("StatusText").GetComponent<Text>();
        }
        if (nameText == null && activeInteractMenu != null)
        {
            nameText = activeInteractMenu.transform.GetChild(0).GetChild(0).transform.Find("NameText").GetComponent<Text>();
        }
        if (descText == null && activeInteractMenu != null && commandInteractRef == null)
        {
            descText = activeInteractMenu.transform.GetChild(0).GetChild(0).transform.Find("TextBackground").transform.GetChild(0).GetComponent<Text>();
        }
        if (currentCycle < passiveCycle && passiveActive == true && passiveGain == true && canPassiveContinue == true) //If the timer hasn't reached target, keep going
        {
            currentCycle += Time.deltaTime;
        }
        if (canPassiveContinue == false && passiveGain == true)
        {
            for (int i = 0; i < 12; i++) //Check here if the passive can turn back on
            {
                if (currentAmounts[i] - resourcesUsed[i] < 0)
                {
                    canPassiveContinue = false;
                    passiveActive = false;
                    break;
                }
                if (currentAmounts[i] - resourcesUsed[i] >= 0)
                {
                    canPassiveContinue = true;
                }
            }
            if (canPassiveContinue == true)
            {
                passiveActive = true;
            }
        }
        if (currentCycle >= passiveCycle && passiveActive == true && passiveGain == true) //When timer reaches or exceeds target, reset, and call method
        {
            currentCycle = 0f;
            ResourceTick();
        }
        if (passiveStorage == true && passiveActive == true && subLast == true)
        {
            StorageAdd();
        }
        if (passiveStorage == true && passiveActive == false && addLast == true)
        {
            StorageSub();
        }
    }

    void OnMouseOver()
    {
        if (outlineRef != null)
        {
            outlineRef.enabled = true;
            if (destroyed == true)
            {
                outlineRef.OutlineColor = Color.red;
            }
            if (destroyed == false)
            {
                outlineRef.OutlineColor = textOnColor;
            }
            outlineRef.OutlineWidth = 3f;
            outlineRef.OutlineMode = Outline.Mode.OutlineVisible;
        }
        if (nameTextCurrent == null && nameTextPrefab != null)
        {
            nameTextCurrent = Instantiate(nameTextPrefab, this.transform.position, nameTextPrefab.transform.rotation, this.transform);
            nameTextCurrent.transform.position += new Vector3(0f, nameTextOffset, 0f);
            if (destroyed == true)
            {
                nameTextCurrent.transform.Find("Name").gameObject.GetComponent<Text>().color = textOffColor;
                nameTextCurrent.transform.Find("NameBackground").gameObject.GetComponent<Image>().color = textBackgroundOffColor;
            }
            if (destroyed == false)
            {
                nameTextCurrent.transform.Find("Name").gameObject.GetComponent<Text>().color = textOnColor;
                nameTextCurrent.transform.Find("NameBackground").gameObject.GetComponent<Image>().color = textBackgroundOnColor;
            }
            nameTextCurrent.transform.Find("Name").gameObject.GetComponent<Text>().text = nameString;
        }
    }

    void OnMouseExit()
    {
        if (outlineRef != null)
        {
            outlineRef.enabled = false;
        }
        if (nameTextCurrent != null)
        {
            Destroy(nameTextCurrent);
        }
    }

    void ResourceTick()
    {
        for (int i = 0; i < 12; i++)
        {
            InventoryManager.Instance.Add(i, resourcesGained[i]);
            InventoryManager.Instance.Sub(i, resourcesUsed[i]);
        }
        for (int i = 0; i < 12; i++) //Check here if the passive can continue
        {
            if (currentAmounts[i] - resourcesUsed[i] < 0)
            {
                canPassiveContinue = false;
                passiveActive = false;
                break;
            }
            if (currentAmounts[i] - resourcesUsed[i] >= 0)
            {
                canPassiveContinue = true;
            }
        }
    }

    void Upgrade()
    {
        if (canUpgrade == true)
        {
            for (int i = 0; i < upgradeCostIndex.Length; i++)
            {
                if (currentAmounts[upgradeCostIndex[i]] >= upgradeCostAmount[i])
                {
                    InventoryManager.Instance.Sub(upgradeCostIndex[i], upgradeCostAmount[i]);
                }
            }
            if (upgradeButton != null)
            {
                GameObject hoverOpen = upgradeButton.GetComponent<UpgradeButton>().currentHover;
                Destroy(hoverOpen);
            }
            if (activeInteractMenu != null)
            {
                activeInteractMenu.transform.GetChild(0).GetChild(0).transform.Find("ExitBody").gameObject.GetComponent<ExitButton>().DestroyMenu();
            }
            if (commandInteractRef != null && currentUpgrade == 1)
            {
                TierHandler.Instance.Tier2Reached();
            }
            if (commandInteractRef != null && currentUpgrade == 2)
            {
                TierHandler.Instance.Tier3Reached();
            }
            currentUpgrade++;
            justUpgraded = true;
            switch (currentUpgrade)
            {
                case 0:
                case 1:
                    newModel = buildingObject.t1buildingModel;
                    break;
                case 2:
                    newModel = buildingObject.t2buildingModel;
                    break;
                case 3:
                    newModel = buildingObject.t3buildingModel;
                    break;
                default:
                    Debug.LogWarning("Tier out of expected range in building passive");
                    break;
            }
            GameObject buildingAnchor = this.transform.GetChild(0).gameObject;
            GameObject prefabAnchor = newModel.transform.GetChild(0).gameObject;
            GameObject buildInstance = Instantiate(newModel, this.transform.position, newModel.transform.rotation, this.transform.parent);
            GameObject instanceAnchor = buildInstance.transform.GetChild(0).gameObject;
            instanceAnchor.transform.position = currentHex.transform.GetChild(0).position; //Fixed anchoring problem here
            buildInstance.transform.position = (instanceAnchor.transform.position - prefabAnchor.transform.localPosition);
            MonoBehaviour BPInstance = this;
            if (commandInteractRef != null)
            {
                MonoBehaviour commandInstance = commandInteractRef;
                System.Reflection.FieldInfo[] fieldsCommand = commandInstance.GetType().GetFields();
                foreach (System.Reflection.FieldInfo field in fieldsCommand)
                {
                    field.SetValue(buildInstance.GetComponent(commandInstance.GetType()), field.GetValue(commandInstance));
                }
            }
            if (buildInteractRef != null)
            {
                MonoBehaviour buildIntInstance = buildInteractRef;
                System.Reflection.FieldInfo[] fieldsBuild = buildIntInstance.GetType().GetFields();
                foreach (System.Reflection.FieldInfo field in fieldsBuild)
                {
                    field.SetValue(buildInstance.GetComponent(buildIntInstance.GetType()), field.GetValue(buildIntInstance));
                }
            }
            System.Reflection.FieldInfo[] fieldsBP = BPInstance.GetType().GetFields();
            foreach (System.Reflection.FieldInfo field in fieldsBP)
            {
                field.SetValue(buildInstance.GetComponent(buildInstance.GetComponent<BuildingPassive>().GetType()), field.GetValue(BPInstance));
            }
            Destroy(this.gameObject);
        }
    }

    void StorageAdd()
    {
        for (int i = 0; i < 12; i++)
        {
            InventoryManager.Instance.AddStorage(i, storageBonus[i]);
        }
        addLast = true;
        subLast = false;
    }

    void StorageSub()
    {
        for (int i = 0; i < 12; i++)
        {
            InventoryManager.Instance.SubStorage(i, storageBonus[i]);
        }
        addLast = false;
        subLast = true;
    }

    void PassiveOn()
    {
        if (destroyed == false)
        {
            passiveActive = true;
            if (statusText != null)
            {
                statusText.text = "On";
                statusText.color = onColor;
            }
        }    
    }

    void PassiveOff()
    {
        passiveActive = false;
        if (statusText != null)
        {
            statusText.text = "Off";
            statusText.color = offColor;
        }
    }

    public void EarthquakeDestroy()
    {
        destroyed = true;
        passiveActive = false;
        currentParticleSystem = Instantiate(smokeParticlePrefab, this.transform.position, smokeParticlePrefab.transform.rotation);
        buildingAudio.clip = damagedSFX;
        buildingAudio.Play();
        currentParticleSystem.GetComponent<ParticleSystem>().Play();
    }

    public void Repair()
    {
        if (repairButton != null)
        {
            GameObject hoverOpen = repairButton.GetComponent<RepairButton>().currentHover;
            Destroy(hoverOpen);
        }
        for (int i = 0; i < repairCostIndex.Length; i++)
        {
            InventoryManager.Instance.Sub(repairCostIndex[i], repairCostAmount[i]);
        }
        destroyed = false;
        buildingAudio.clip = repairedSFX;
        buildingAudio.Play();
        if (currentParticleSystem != null)
        {
            Destroy(currentParticleSystem);
        }
    }
}
