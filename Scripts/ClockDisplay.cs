using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClockDisplay : MonoBehaviour
{
    [SerializeField]
    Text elementText;
    [SerializeField]
    string displayText;

    void Start()
    {
        elementText = this.gameObject.GetComponent<Text>();
    }

    void Update()
    {
        displayText = TimeManager.displayTime;
        elementText.text = displayText;
    }
}
