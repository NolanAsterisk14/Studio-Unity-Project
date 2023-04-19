using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public enum Resource
{
    Population,
    Happiness,
    Oxygen,
    Power,
    Fuel,
    Water,
    BioScrap,
    CommonMetals,
    RareMetals,
    Minerals,
    Electronics,
    RefinedMetals
}

public class InventoryManager : MonoBehaviour
{
    public string[] resourceName = { "Population", "Happiness", "Oxygen", "Power", "Fuel", "Water", "BioScrap", "CommonMetals", "RareMetals", "Minerals", "Electronics", "RefinedMetals" };
    public int[] resourceAmount = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    public int[] resourceMin = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    public int[] resourceMax = {1, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
    public int popWinCap = 25;
    public float winDelayTime = 2f;
    public float lossDelayTime = 0f;
    public float oxygenCycle = 60f;
    public float oxygenTime = 0f;
    public float happinessCycle = 30f;
    public float happinessTime = 0f;
    public float oxygenLossDelay = 10f;
    public float oxygenLossDelayTime = 0f;
    public float oxygenDeathCycle = 2f;
    public float oxygenDeathTime = 0f;
    public bool lostPopFirstTime = false;
    public bool popMessageDisplayed = false;
    public bool oxygenMessageDisplayed = false;
    public bool fuelMessageDisplayed = false;
    public bool mutinyMessageDisplayed = false;
    public bool winStarted = false;
    public bool lossStarted = false;
    public bool anyMenuOpen = false;
    public GameObject currentOpenMenu;          //Auto set
    public GameObject popLossMessage;           //Initialize, could change to resources.load later but time is short
    public GameObject winMessage;               //Initialize
    public GameObject lossMessage;              //Initialize
    public GameObject oxygenMessage;            //Initialize
    public GameObject disasterMessageT2;        //Initialize
    public GameObject disasterMessageT3;        //Initialize
    public GameObject fuelMessage;              //Initialize
    public GameObject mutinyMessage;            //Initialize

    public static InventoryManager Instance { get; private set; }

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

    private void Update()
    {
        if (currentOpenMenu == null)
        {
            anyMenuOpen = false;
        }
        if (currentOpenMenu != null)
        {
            anyMenuOpen = true;
        }  
        if (resourceAmount[4] == resourceMin[4] && fuelMessageDisplayed == false) //Fuel message sequence here, resets each time
        {
            currentOpenMenu = Instantiate(fuelMessage, fuelMessage.transform.position, fuelMessage.transform.rotation, fuelMessage.transform.parent);
            anyMenuOpen = true;
            fuelMessageDisplayed = true;
            StartCoroutine("FuelLoop");
        }
        if (resourceAmount[4] > resourceMin[4] && fuelMessageDisplayed == true) //reset
        {
            fuelMessageDisplayed = false;
        }
        if (lostPopFirstTime == true && popMessageDisplayed == false) //Pop message sequence here, displays once when pop lost
        {
            currentOpenMenu = Instantiate(popLossMessage, popLossMessage.transform.position, popLossMessage.transform.rotation, popLossMessage.transform.parent);
            anyMenuOpen = true;
            popMessageDisplayed = true;
        }
        if (resourceAmount[2] == resourceMin[2] && oxygenMessageDisplayed == false) //Oxygen message sequence here, resets each time
        {
            currentOpenMenu = Instantiate(oxygenMessage, oxygenMessage.transform.position, oxygenMessage.transform.rotation, oxygenMessage.transform.parent);
            anyMenuOpen = true;
            oxygenMessageDisplayed = true;
            StartCoroutine("OxygenLoop");
        }
        if (resourceAmount[2] > resourceMin[2] && oxygenMessageDisplayed == true) //reset
        {
            oxygenMessageDisplayed = false;
        }
        if (resourceMax[0] >= popWinCap && resourceAmount[0] >= popWinCap && winStarted == false) //Win sequence here
        {
            StartCoroutine("WinDelay");
            winStarted = true;
        }
        if (resourceAmount[0] == resourceMin[0] && lossStarted == false) //Loss sequence here
        {
            StartCoroutine("LossDelay");
            lossStarted = true;
        }
        if (resourceAmount[4] == resourceMin[4] && resourceAmount[1] == resourceMin[1] && mutinyMessageDisplayed == false) //Mutiny message sequence here, loss contition so no reset needed
        {
            StartCoroutine("MutinyLoss");
            mutinyMessageDisplayed = true;
        }
        if (oxygenTime < oxygenCycle)
        {
            oxygenTime += Time.deltaTime;
        }
        if (oxygenTime >= oxygenCycle)
        {
            oxygenTime = 0f;
            float oxygenUsed = Mathf.Floor(resourceAmount[0] * 1.5f);
            Sub(2, (int)oxygenUsed);
        }
    }

    public void Add(Resource nameIndex, int amount)
    {
        if ((resourceAmount[(int)nameIndex] + amount) <= resourceMax[(int)nameIndex])
        {
            resourceAmount[(int)nameIndex] += amount;
        }
        if ((resourceAmount[(int)nameIndex] + amount) > resourceMax[(int)nameIndex])
        {
            resourceAmount[(int)nameIndex] = resourceMax[(int)nameIndex];
        }
    }

    public void Add(int nameIndex, int amount)
    {
        if ((resourceAmount[(int)nameIndex] + amount) <= resourceMax[(int)nameIndex])
        {
            resourceAmount[(int)nameIndex] += amount;
        }
        if ((resourceAmount[(int)nameIndex] + amount) > resourceMax[(int)nameIndex])
        {
            resourceAmount[(int)nameIndex] = resourceMax[(int)nameIndex];
        }
    }

    public void Sub(Resource nameIndex, int amount)
    {
        if ((int)nameIndex == 0 && amount > 0 && lostPopFirstTime == false)
        {
            lostPopFirstTime = true;
        }
        if ((resourceAmount[(int)nameIndex] - amount) >= resourceMin[(int)nameIndex])
        {
            resourceAmount[(int)nameIndex] -= amount;
        }
        if ((resourceAmount[(int)nameIndex] - amount) < resourceMin[(int)nameIndex])
        {
            resourceAmount[(int)nameIndex] = resourceMin[(int)nameIndex];
        }
    }

    public void Sub(int nameIndex, int amount)
    {
        if ((int)nameIndex == 0 && amount > 0 && lostPopFirstTime == false)
        {
            lostPopFirstTime = true;
        }
        if ((resourceAmount[(int)nameIndex] - amount) >= resourceMin[(int)nameIndex])
        {
            resourceAmount[(int)nameIndex] -= amount;
        }
        if ((resourceAmount[(int)nameIndex] - amount) < resourceMin[(int)nameIndex])
        {
            resourceAmount[(int)nameIndex] = resourceMin[(int)nameIndex];
        }
    }

    public void AddStorage(int nameIndex, int amount)
    {
        if (resourceMax[(int)nameIndex] + amount < popWinCap && (int)nameIndex == 0)
        {
            resourceMax[(int)nameIndex] += amount;
        }
        if (resourceMax[(int)nameIndex] + amount >= popWinCap && (int)nameIndex == 0)
        {
            resourceMax[(int)nameIndex] = popWinCap;
        }
        if ((int)nameIndex > 0)
        {
            resourceMax[(int)nameIndex] += amount;
        }
    }

    public void SubStorage(int nameIndex, int amount)
    {
        resourceMax[(int)nameIndex] -= amount;
    }

    public int[] CostQuery(int[] nameIndexes)
    {
        int[] amounts = new int[nameIndexes.Length];
        int i = 0;
        foreach (int index in nameIndexes)
        {
            amounts[i] = resourceAmount[nameIndexes[i]];
            i++;
        }
        return amounts;
    }

    public string[] NameQuery(int[] nameIndexes)
    {
        string[] names = new string[nameIndexes.Length];
        int i = 0;
        foreach (int index in nameIndexes)
        {
            names[i] = resourceName[nameIndexes[i]];
            i++;
        }
        return names;
    }

    public bool IndexFullCheck(int nameIndex)
    {
        bool isFull = false;
        if (resourceAmount[nameIndex] < resourceMax[nameIndex])
        {
            isFull = false;
        }
        if (resourceAmount[nameIndex] == resourceMax[nameIndex])
        {
            isFull = true;
        }
        return isFull;
    }

    public void DisasterWarningT2()
    {
        currentOpenMenu = Instantiate(disasterMessageT2, disasterMessageT2.transform.position, disasterMessageT2.transform.rotation, disasterMessageT2.transform.parent);
        anyMenuOpen = true;
    }

    public void DisasterWarningT3()
    {
        currentOpenMenu = Instantiate(disasterMessageT3, disasterMessageT3.transform.position, disasterMessageT3.transform.rotation, disasterMessageT3.transform.parent);
        anyMenuOpen = true;
    }

    IEnumerator WinDelay()
    {
        yield return new WaitWhile(() => anyMenuOpen == true);
        yield return new WaitForSecondsRealtime(winDelayTime);
        yield return new WaitWhile(() => anyMenuOpen == true);
        currentOpenMenu = Instantiate(winMessage, winMessage.transform.position, winMessage.transform.rotation, winMessage.transform.parent);
        anyMenuOpen = true;
        yield return new WaitWhile(() => anyMenuOpen == true);
        Debug.Log("Win sequence ended");
        this.gameObject.transform.parent.gameObject.GetComponent<SceneLoader>().SceneTargeter("WinScreen");
    }

    IEnumerator LossDelay()
    {
        yield return new WaitWhile(() => anyMenuOpen == true);
        yield return new WaitForSecondsRealtime(lossDelayTime);
        yield return new WaitWhile(() => anyMenuOpen == true);
        currentOpenMenu = Instantiate(lossMessage, lossMessage.transform.position, lossMessage.transform.rotation, lossMessage.transform.parent);
        anyMenuOpen = true;
        yield return new WaitWhile(() => anyMenuOpen == true);
        Debug.Log("Loss sequence ended");
        this.gameObject.transform.parent.gameObject.GetComponent<SceneLoader>().SceneTargeter("LossScreen");
    }

    IEnumerator MutinyLoss()
    {
        yield return new WaitWhile(() => anyMenuOpen == true);
        yield return new WaitForSecondsRealtime(lossDelayTime);
        currentOpenMenu = Instantiate(mutinyMessage, mutinyMessage.transform.position, mutinyMessage.transform.rotation, mutinyMessage.transform.parent);
        yield return new WaitWhile(() => anyMenuOpen == true);
        this.gameObject.transform.parent.gameObject.GetComponent<SceneLoader>().SceneTargeter("LossScreen");
    }

    IEnumerator OxygenLoop()
    {
        while (resourceAmount[2] == resourceMin[2])
        {
            if (oxygenLossDelayTime < oxygenLossDelay)
            {
                oxygenLossDelayTime += Time.deltaTime;
            }
            if (oxygenLossDelayTime >= oxygenLossDelay)
            {
                if (oxygenDeathTime < oxygenDeathCycle)
                {
                    oxygenDeathTime += Time.deltaTime;
                }
                if (oxygenDeathTime >= oxygenDeathCycle)
                {
                    Sub(0, 1);
                    oxygenDeathTime = 0f;
                }
            }
            yield return null;
        }    
    }

    IEnumerator FuelLoop()
    {
        while (resourceAmount[4] == resourceMin[4])
        {
            if (happinessTime < happinessCycle)
            {
                happinessTime += Time.deltaTime;
            }    
            if (happinessTime >= happinessCycle)
            {
                Sub(1, 1);
                happinessTime = 0f;
            }
            yield return null;
        }
    }

}
