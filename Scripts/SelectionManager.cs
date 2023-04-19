using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public GameObject currentSelection;

    public static SelectionManager Instance { get; private set; }

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

    public void SelectionSet(GameObject newSelection)
    {
        currentSelection = newSelection;
    }

    public GameObject SelectionGet()
    {
        if (currentSelection != null)
        {
            GameObject returnObj = currentSelection;
            return returnObj;
        }
        else
        {
            return null;
        }
    }

    public void SelectionFlush()
    {
        currentSelection = default(GameObject);
    }
}
