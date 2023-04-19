using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class BuildButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Button dragButton;
    public Building buildingObject;             //Script obj reference (set in inspector)
    public GameObject prefabModel;              //Building model
    public GameObject prefabAnchor;             //Building anchor (used for referencing local position for ground adj.)
    public GameObject outlineModel;             //Model used for ghost outline of building
    public GameObject outlineAnchor;            //Anchor of building, connects to ground anchor
    public GameObject finalModel;               //Model reference for final placement
    public GameObject finalAnchor;              //Anchor reference for final placement
    public GameObject mapObject;                //Can be assigned, or start will find it
    public GameObject[] buildableTiles;         //For outline during buildstate
    public Material[] outlineMaterials;         //This will be for creating the building outline while in build mode
    public Color outlineColor;                  //Color and alpha for ghost outline of building
    public Color finalColor;                    //Color to be applied to final object
    public bool buildState = false;
    public bool allowRotation = false;          //Allows rotating the gameobject while in build mode
    public bool tilesUpdated = false;           //Update check bool for build state tiles
    public GameObject currentSelection;         //Current building selected by raycast
    public GameObject selectionAnchor;          //Anchor of hex tile
    public Vector3 impactPoint;                 //Latest impact point of raycast (reset on var change)
    public string description;                  //Set by start, for hover tooltip
    public GameObject hoverMenu;                //Will be the object instantiated for tooltips, set in inspector!!
    public GameObject currentHover;             //Current instance of hovermenu
    public Text costText;                       //Set when menu instantiates *not used now*
    public Text descText;                       //Set when menu instantiates
    public Sprite[] costSprites = new Sprite[12]; //For the TT menu, initialize these in the same order as their index
    public Vector3 hoverTargetPos = new Vector3(100, 150, 0);   //Position for hoverMenu instantiation
    public int[] cIndex;
    public int[] cAmount;
    public int currentTier;                     //Updated from tier manager
    public bool canBuild;
    

    void Start()
    {
        dragButton = this.gameObject.GetComponent<Button>();
        outlineColor = new Color(0f, 1f, 0f, 0.5f);
        finalColor = new Color(1f, 1f, 1f, 1f);
        if (mapObject == null)
        {
            mapObject = GameObject.Find("MapNotConnected");
        }
    }

    void Update()
    {
        currentTier = TierHandler.Instance.TierCheck();
        switch (currentTier)
        {
            case 0:
            case 1:
                prefabModel = buildingObject.t1buildingModel;
                description = buildingObject.t1description;
                cIndex = buildingObject.t1buildCostIndex;
                cAmount = buildingObject.t1buildCostAmount;
                break;
            case 2:
                prefabModel = buildingObject.t2buildingModel;
                description = buildingObject.t2description;
                cIndex = buildingObject.t2buildCostIndex;
                cAmount = buildingObject.t2buildCostAmount;
                break;
            case 3:
                prefabModel = buildingObject.t3buildingModel;
                description = buildingObject.t3description;
                cIndex = buildingObject.t3buildCostIndex;
                cAmount = buildingObject.t3buildCostAmount;
                break;
        }
        if (prefabModel != null && prefabAnchor == null)
        {
            if (prefabModel.transform.childCount > 0)
            {
                prefabAnchor = prefabModel.transform.GetChild(0).gameObject;
            }
        }
        if (buildState == true && allowRotation == true)
        {
            if (Input.GetKeyDown("r"))
            {
                outlineModel.transform.localRotation *= Quaternion.Euler(0, 90, 0);
            }
        }
        //if (buildState == true && tilesUpdated == false)  //This was a really cool idea. But it kills performance when build state starts, so maybe RIP
        //{
        //    buildableTiles = GameObject.FindGameObjectsWithTag("BuildableHex");
        //    if (buildableTiles.Length > 0)
        //    {
        //        foreach (GameObject buildTile in buildableTiles)
        //        {
        //            if (buildTile.GetComponent<Outline>() == null)
        //            {
        //                Outline outlineTemp = buildTile.AddComponent<Outline>();
        //                outlineTemp.OutlineColor = Color.green;
        //                outlineTemp.OutlineMode = Outline.Mode.OutlineVisible;
        //                outlineTemp.OutlineWidth = 3f;
        //            }
        //        }
        //    }
        //    tilesUpdated = true;
        //}
        //if (buildState == false && tilesUpdated == true)
        //{
        //    if (buildableTiles.Length > 0)
        //    {
        //        foreach (GameObject buildTile in buildableTiles)
        //        {
        //            if (buildTile.GetComponent<Outline>() != null)
        //            {
        //                Destroy(buildTile.GetComponent<Outline>());
        //            }
        //        }
        //    }
        //    tilesUpdated = false;
        //}
        if (buildState == true)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            int layerMask = 1 << 6;

            if (Physics.Raycast(ray, out hit, 300f, layerMask))
            {
                currentSelection = hit.transform.gameObject;
                impactPoint = hit.point;
                if (currentSelection.transform.childCount > 0)
                {
                    selectionAnchor = currentSelection.transform.GetChild(0).gameObject;
                }
            }
        }
        if (buildState == false && outlineModel != null)
        {
            Destroy(outlineModel);
        }
    }

    void LateUpdate()
    {
        if (buildState == true && outlineAnchor != null && outlineModel != null && selectionAnchor != null && currentSelection.gameObject.tag == "BuildableHex")
        {
            outlineAnchor.transform.position = selectionAnchor.transform.position;
            outlineModel.transform.position = (outlineAnchor.transform.position - prefabAnchor.transform.localPosition);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        int[] currentAmounts = InventoryManager.Instance.CostQuery(cIndex);
        if (cIndex.Length == 0)
        {
            canBuild = true;
        }
        if (cIndex.Length > 0)
        {
            for (int i = 0; i < currentAmounts.Length; i++)
            {
                if (currentAmounts[i] < cAmount[i])
                {
                    canBuild = false;
                    break;
                }
                if (currentAmounts[i] >= cAmount[i])
                {
                    canBuild = true;
                }
            }
        }
        
        if (canBuild == true)
        {
            buildState = true;
            outlineModel = Instantiate(prefabModel, impactPoint, prefabModel.transform.rotation, mapObject.transform);
            SphereCollider[] sphereColliders = outlineModel.transform.GetComponents<SphereCollider>();
            foreach (SphereCollider collider in sphereColliders)
            {
                collider.enabled = false;
            }
            var script = outlineModel.GetComponent<BuildingPassive>();
            if (script != null)
            {
                script.enabled = false;
            }
            outlineMaterials = outlineModel.transform.GetComponent<MeshRenderer>().materials;
            foreach (Material tempMaterial in outlineMaterials)
            {
                tempMaterial.color = outlineColor;
                tempMaterial.SetColor("_EmissionColor", outlineColor);
                tempMaterial.EnableKeyword("_EMISSION");
            }
            if (outlineModel.transform.childCount > 0)
            {
                outlineAnchor = outlineModel.transform.GetChild(0).gameObject;
            }
        }

        if (canBuild == false)
        {
            
        }
    }

    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (currentSelection != null)
        {
            if (currentSelection.gameObject.tag == "BuildableHex")
            {
                buildState = false;
                finalModel = Instantiate(prefabModel, outlineModel.transform.position, outlineModel.transform.rotation, currentSelection.transform.parent.parent);
                if (currentSelection != null)
                {
                    finalModel.transform.GetComponent<BuildingPassive>().currentHex = currentSelection.gameObject;
                }    
                Destroy(outlineModel, 0);
                if (finalModel.transform.childCount > 0)
                {
                    finalAnchor = finalModel.transform.GetChild(0).gameObject;
                }
                finalAnchor.transform.position = selectionAnchor.transform.position;
                finalModel.transform.position = (finalAnchor.transform.position - prefabAnchor.transform.localPosition);
                for (int i = 0; i < cIndex.Length; i++)
                {
                    InventoryManager.Instance.Sub(cIndex[i], cAmount[i]);
                }
                currentSelection.gameObject.tag = "OccupiedHex";
            }
            if (currentSelection.gameObject.tag == "OccupiedHex")
            {
                buildState = false;
                Destroy(outlineModel);
            }
            if (currentSelection.gameObject.tag != "OccupiedHex" && currentSelection.gameObject.tag != "BuildableHex")
            {
                buildState = false;
                Destroy(outlineModel);
            }
        }
    }


    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        string[] costs = Array.ConvertAll(cAmount, x => x.ToString());
        currentHover = Instantiate(hoverMenu, this.transform.position, this.transform.rotation, this.transform.parent.parent.parent.parent.parent.parent.transform);
        currentHover.transform.localPosition += hoverTargetPos;
        Transform spriteParent = currentHover.transform.GetChild(0).GetChild(0).transform.Find("Sprites");
        Transform textParent = currentHover.transform.GetChild(0).GetChild(0).transform.Find("Texts");
        Image[] sprites = new Image[5];
        Text[] costTexts = new Text[5];
        int i2 = 0;
        foreach (Transform child in spriteParent)
        {
            sprites[i2] = child.GetComponent<Image>();
            i2++;
        }
        int i3 = 0;
        foreach (Transform child in textParent)
        {
            costTexts[i3] = child.GetComponent<Text>();
            i3++;
        }
        if (sprites != null)
        {
            for (int i = 0; i < cIndex.Length; i++)
            {
                sprites[i].sprite = costSprites[cIndex[i]];
            }
        }
        if (costTexts != null)
        {
            for (int i = 0; i < cIndex.Length; i++)
            {
                costTexts[i].text = costs[i];
            }
        }
        descText = currentHover.transform.GetChild(0).GetChild(0).transform.Find("DescriptionText").GetComponent<Text>();
        descText.text = description;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        Destroy(currentHover, 0);
    }

}
