using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BBTest : MonoBehaviour
{
    public Vector3 bbCenter = new Vector3(0, 0, 0);
    public float yOffset = 0.9916969f;

    void Update()
    {
        bbCenter = this.GetComponent<MeshFilter>().mesh.bounds.center;
        bbCenter.y += yOffset;
    }
}
