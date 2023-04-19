using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDisplay : MonoBehaviour
{
    [SerializeField]
    Text[] texts;
    [SerializeField]
    Text popMax;
  
    void Update()
    {
        int index = 0;
        foreach (Text displayText in texts)
        {
            displayText.text = InventoryManager.Instance.resourceAmount[index].ToString();
            index++;
        }

        popMax.text = InventoryManager.Instance.resourceMax[0].ToString();
        
    }
}
