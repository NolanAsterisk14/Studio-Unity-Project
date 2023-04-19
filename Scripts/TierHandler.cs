using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TierHandler : MonoBehaviour
{
    public bool t1 = false;     //These variables will dictate which tiers have been reached
    public bool t2 = false; 
    public bool t3 = false;
    public bool commandAbleT3 = false;
    public int currentTier = 0;

    public static TierHandler Instance { get; private set; }

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
        if (t1 == false)
        {
            currentTier = 0;
        }
        if (t1 == true && t2 == false)
        {
            currentTier = 1;
        }
        if (t1 == true && t2 == true && t3 == false)
        {
            currentTier = 2;
        }
        if (t1 == true && t2 == true && t3 == true)
        {
            currentTier = 3;
        }
    }

    public int TierCheck()
    {
        int tier = currentTier;
        return tier;
    }

    //reached methods
    public void Tier1Reached()
    {
        t1 = true;
    }

    public void Tier2Reached()
    {
        t2 = true;
    }

    public void Tier3Reached()
    {
        t3 = true;
    }
    //lost methods
    public void Tier1Lost()
    {
        t1 = false;
    }

    public void Tier2Lost()
    {
        t2 = false;
    }

    public void Tier3Lost()
    {
        t3 = false;
    }

    public void CommandCanT3()
    {
        commandAbleT3 = true;
    }

    public void CommandCanNotT3()
    {
        commandAbleT3 = false;
    }
    
}
