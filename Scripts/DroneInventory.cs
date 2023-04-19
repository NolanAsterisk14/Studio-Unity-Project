using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneInventory : MonoBehaviour     //Sadly this script was ultimately cut. Keeping in case I want to redo these scripts
{
    public int storageSlots = 1;
    public int storageSlotsUsed = 0;
    public int indexesFull = 0;
    public string[] resourceName = { "Population", "Happiness", "Oxygen", "Power", "Fuel", "Water", "BioScrap", "CommonMetals", "RareMetals", "Minerals", "Electronics", "RefinedMetals" };
    public int[] resourceAmount = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    public int[] resourceMin = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    public int[] resourceMax = { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 };
    public List<int> storedIndexList = new List<int>();
    public bool slotsAllOccupied = false;
    public bool slotsAllFull = false;

    void Update()
    {
        if (slotsAllOccupied == false) //check to see if slots are occupied
        {
            for (int i = 0; i < resourceAmount.Length; i++)
            {
                if (resourceAmount[i] > 0)
                {
                    storedIndexList.Add(i);
                    storageSlotsUsed = storedIndexList.Count;
                }
                if (storedIndexList.Count == storageSlots)
                {
                    slotsAllOccupied = true;
                    break;
                }
                if (storedIndexList.Count > storageSlots)
                {
                    Debug.LogError("DroneInventory messed up");
                }
            }
        }

        if (slotsAllOccupied == true && slotsAllFull == false) //if slots are all occupied, see if they're full
        {
            indexesFull = 0;
            storedIndexList.ForEach(FullCheck);
        }

    }

    void FullCheck(int index)
    {
        if (resourceAmount[index] < resourceMax[index])
        {
            slotsAllFull = false;
        }
        if (resourceAmount[index] >= resourceMax[index])
        {
            indexesFull++;
        }
        if (indexesFull >= storageSlots)
        {
            slotsAllFull = true;
        }
    }

    public void InventoryAdd(int cIndex, int cAmount)
    {
        resourceAmount[cIndex] = cAmount;
    }

    public bool SlotsOccupiedCheck()
    {
        bool slotsAreOccupied = slotsAllOccupied;
        return slotsAreOccupied;
    }

    public bool SlotsFullCheck()
    {
        bool slotsAreFull = slotsAllFull;
        return slotsAreFull;
    }

    public bool OccupiedCanStore(int cIndex, int cAmount)
    {
        bool canStore;
        if ((resourceAmount[cIndex] + cAmount) <= resourceMax[cIndex])
        {
            canStore = true;
            return canStore;
        }
        if ((resourceAmount[cIndex] + cAmount) > resourceMax[cIndex])
        {
            canStore = false;
            return canStore;
        }
        else
        {
            canStore = false;
        }
        return canStore;
    }
}
