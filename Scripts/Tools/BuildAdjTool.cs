using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildAdjTool : MonoBehaviour
{
    public GameObject[] buildings;//init
    public GameObject[] anchors = new GameObject[3];
    public GameObject[] hexes;//init
    public GameObject[] hexAnchors = new GameObject[3];
    public Vector3 boundsCenter;
    public bool centerAnchors;
    public bool centerTest;
    

    void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            anchors[i] = buildings[i].transform.GetChild(0).gameObject;
            hexAnchors[i] = hexes[i].transform.GetChild(0).gameObject;
            boundsCenter = buildings[i].transform.GetComponent<MeshFilter>().mesh.bounds.center;
            if (centerAnchors == true)
            {
                anchors[i].transform.localPosition = boundsCenter;
            }
            if (centerTest == true)
            {
                Vector3 startLocPos = anchors[i].transform.localPosition;
                anchors[i].transform.position = hexAnchors[i].transform.position;
                buildings[i].transform.position = anchors[i].transform.position;
                buildings[i].transform.position = (buildings[i].transform.position - startLocPos);
            }
        }
    }
}
