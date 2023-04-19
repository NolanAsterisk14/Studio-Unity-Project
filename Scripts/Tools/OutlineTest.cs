using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineTest : MonoBehaviour
{
    public Outline outlineRef;

    void Start()
    {
        if (outlineRef == null)
        {
            if (TryGetComponent (out Outline outlineScript))
            {
                outlineRef = outlineScript;
            }
        }
    }

    void OnMouseOver()
    {
        if (outlineRef != null)
        {
            outlineRef.enabled = true;
        }
    }

    void OnMouseExit()
    {
        if (outlineRef != null)
        {
            outlineRef.enabled = false;
        }
    }
}
