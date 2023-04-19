using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectButton : MonoBehaviour
{
    public Button collectButton; //initialize!
    public GameObject parentCanvas; //initialize!
    public int resourceIndex; //Set!
    public int resourceAmount; //Set!!

    //public enum Resource   *For Reference*
    //{
    //    Population,   = 0
    //    Happiness,    = 1
    //    Oxygen,       = 2
    //    Power,        = 3
    //    Fuel,         = 4
    //    Water,        = 5
    //    BioScrap,     = 6
    //    CommonMetals, = 7
    //    RareMetals,   = 8
    //    Minerals,     = 9
    //    Electronics,  = 10
    //    RefinedMetals = 11
    //}

    void Start()
    {
        if (collectButton == null)
        {
            collectButton = this.gameObject.GetComponent<Button>();
        }
        collectButton.onClick.AddListener(AddResources);
    }


    void AddResources()
    {
        switch (resourceIndex)
        {
            case 0:
                InventoryManager.Instance.Add(Resource.Population, resourceAmount);
                break;
            case 1:
                InventoryManager.Instance.Add(Resource.Happiness, resourceAmount);
                break;
            case 2:
                InventoryManager.Instance.Add(Resource.Oxygen, resourceAmount);
                break;
            case 3:
                InventoryManager.Instance.Add(Resource.Power, resourceAmount);
                break;
            case 4:
                InventoryManager.Instance.Add(Resource.Fuel, resourceAmount);
                break;
            case 5:
                InventoryManager.Instance.Add(Resource.Water, resourceAmount);
                break;
            case 6:
                InventoryManager.Instance.Add(Resource.BioScrap, resourceAmount);
                break;
            case 7:
                InventoryManager.Instance.Add(Resource.CommonMetals, resourceAmount);
                break;
            case 8:
                InventoryManager.Instance.Add(Resource.RareMetals, resourceAmount);
                break;
            case 9:
                InventoryManager.Instance.Add(Resource.Minerals, resourceAmount);
                break;
            case 10:
                InventoryManager.Instance.Add(Resource.Electronics, resourceAmount);
                break;
            case 11:
                InventoryManager.Instance.Add(Resource.RefinedMetals, resourceAmount);
                break;
            default:
                Debug.Log("Set a proper resource index for one of your collect scripts, you doofus.");
                break;

        }
        TimeManager.Instance.TimeStart();
        CameraController.Instance.ControlOn();
        Destroy(parentCanvas, 0);
    }
}
