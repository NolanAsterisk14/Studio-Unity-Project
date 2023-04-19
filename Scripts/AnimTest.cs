using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimTest : MonoBehaviour
{
    public Animator anim;

    void Start()
    {
        anim.SetBool("WatermillBool", true);
    }
}
