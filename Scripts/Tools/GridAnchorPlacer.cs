using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridAnchorPlacer : MonoBehaviour
{
    [SerializeField]
    private GameObject[] allObjects;
    private List<GameObject> tileList = new List<GameObject>();     
    private Vector3 bbCenter = new Vector3(0, 0, 0);
    private float yOffset = 9.916969f;
    private GameObject Anchor;                                          //I was just using this to place grid anchors
    public GameObject dustParticleSystem;                               //But now it's for particle systems too :)

    void Start()
    {
        Anchor = new GameObject();
        Anchor.name = "Anchor";
        Destroy(Anchor);
        allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject gameObject in allObjects)
        {
            if (gameObject.layer == 6)
            {
                tileList.Add(gameObject);
            }
        }
        tileList.ForEach(AnchorAdd);
    }

    void AnchorAdd(GameObject tileObject)
    {
        if (tileObject.GetComponent<MeshFilter>() != null)
        {
            bbCenter = tileObject.GetComponent<MeshFilter>().mesh.bounds.center;
            bbCenter.y += yOffset;
            GameObject AnchorInstance = Instantiate(Anchor, Anchor.transform.position, Anchor.transform.rotation, tileObject.transform); //parent set here, can now use local space
            GameObject dustSystemInstance = Instantiate(dustParticleSystem, dustParticleSystem.transform.position, dustParticleSystem.transform.rotation, tileObject.transform);
            AnchorInstance.transform.localPosition = bbCenter;
            dustSystemInstance.transform.localPosition = bbCenter;
        }
        else
        {
            Debug.LogWarning("You have unintended objects on the hex tile layer, silly.");
        }
    }
}
