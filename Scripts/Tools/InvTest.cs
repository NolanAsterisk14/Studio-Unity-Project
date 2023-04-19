using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvTest : MonoBehaviour
{

    void Start()
    {
        
    }


    void Update()
    {
        if (Input.GetKeyDown("i"))
        {
            InventoryManager.Instance.Add(Resource.Population, 1);
        }

        if (Input.GetKeyDown("k"))
        {
            InventoryManager.Instance.Sub(Resource.Population, 1);
        }

        if (Input.GetKeyDown("m"))
        {
            for (int i = 0; i < 12; i++)
            {
                InventoryManager.Instance.Add(i, 1);
            }
        }

        if (Input.GetKeyDown("l"))
        {
            for (int i = 0; i < 12; i++)
            {
                InventoryManager.Instance.Sub(i, 1);
            }
        }
    }
}
