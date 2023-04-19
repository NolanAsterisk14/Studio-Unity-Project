using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RepairButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int[] uCostIndex;                                            //As long as this object is active,
    public int[] uCostAmount;                                           //BuildingPassive updates both arrays!
    public bool canAfford;                                              //And this bool too.
    public string description;
    public GameObject hoverMenuPrefab;                                  //Initialize
    public GameObject currentHover;
    public Text descText;                                               //Set when menu instantiates
    public Sprite[] costSprites = new Sprite[12];                       //For the TT menu, initialize these in the same order as their index
    public Vector3 hoverTargetPos = new Vector3(10, 10, 0);             //Position for hoverMenu instantiation
    public Color currentColor;                                          //Current color to use
    public Color onColor = new Color(0.3726415f, 1f, 0.482348f, 1f);    //Colors for description
    public Color offColor = new Color(1f, 0.3915094f, 0.3915094f, 1f);

    void Update()
    {
        if (canAfford == true)
        {
            description = "You can repair this building!";
            currentColor = onColor;
        }
        if (canAfford == false)
        {
            description = "You can't repair this building.";
            currentColor = offColor;
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        string[] costs = Array.ConvertAll(uCostAmount, x => x.ToString());
        currentHover = Instantiate(hoverMenuPrefab, this.transform.position, this.transform.rotation, this.transform.parent.parent.transform);
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
            for (int i = 0; i < uCostIndex.Length; i++)
            {
                sprites[i].sprite = costSprites[uCostIndex[i]];
            }
        }
        if (costTexts != null)
        {
            for (int i = 0; i < uCostIndex.Length; i++)
            {
                costTexts[i].text = costs[i];
            }
        }
        descText = currentHover.transform.GetChild(0).GetChild(0).transform.Find("DescriptionText").GetComponent<Text>();
        descText.color = currentColor;
        descText.text = description;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        Destroy(currentHover, 0);
    }
}
