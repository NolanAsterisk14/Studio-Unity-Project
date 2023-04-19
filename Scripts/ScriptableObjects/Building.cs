using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Building", menuName = "Building")]

public class Building : ScriptableObject
{
    [Tooltip("The t1 name that will be visible to the player")]
    public string t1name;
    [Tooltip("The t1 description that will be visible to the player")]
    public string t1description;
    [Tooltip("The t2 name that will be visible to the player")]
    public string t2name;
    [Tooltip("The t2 description that will be visible to the player")]
    public string t2description;
    [Tooltip("The t3 name that will be visible to the player")]
    public string t3name;
    [Tooltip("The t3 description that will be visible to the player")]
    public string t3description;


    [Tooltip("What t1 model should the building be instantiated with?")]
    public GameObject t1buildingModel;
    [Tooltip("What t2 model should the building be instantiated with?")]
    public GameObject t2buildingModel;
    [Tooltip("What t3 model should the building be instantiated with?")]
    public GameObject t3buildingModel;
    [Tooltip("Which t1 thumbnail should this building button have?")]
    public Sprite t1thumbnail;
    [Tooltip("Which t2 thumbnail should this building button have?")]
    public Sprite t2thumbnail;
    [Tooltip("Which t3 thumbnail should this building button have?")]
    public Sprite t3thumbnail;
    [Tooltip("What animation should play when the player selects this building?")]
    public Animation interactAnimation;

    [Tooltip("What menu should open when the player selects this building?")]
    public GameObject interactMenu;


    [Tooltip("The t1 index or indexes of the resource this building requires as a crafting material")]
    public int[] t1buildCostIndex;
    [Tooltip("The t1 amount for each resource required for crafting")]
    public int[] t1buildCostAmount;
    [Tooltip("The t2 index or indexes of the resource this building requires as a crafting material")]
    public int[] t2buildCostIndex;
    [Tooltip("The t2 amount for each resource required for crafting")]
    public int[] t2buildCostAmount;
    [Tooltip("The t3 index or indexes of the resource this building requires as a crafting material")]
    public int[] t3buildCostIndex;
    [Tooltip("The t3 amount for each resource required for crafting")]
    public int[] t3buildCostAmount;
    [Tooltip("The t1 index or indexes of the resource this building requires for upgading")]
    public int[] t1upgradeCostIndex;
    [Tooltip("The t1 amount for each resource required for upgrading")]
    public int[] t1upgradeCostAmount;
    [Tooltip("The t2 index or indexes of the resource this building requires for upgading")]
    public int[] t2upgradeCostIndex;
    [Tooltip("The t2 amount for each resource required for upgrading")]
    public int[] t2upgradeCostAmount;

}
