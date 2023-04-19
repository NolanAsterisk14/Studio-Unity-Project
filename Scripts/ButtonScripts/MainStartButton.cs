using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainStartButton : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Set this in the inspector for the scene to be loaded")]
    string targetScene;
    [SerializeField]
    Button thisButton;
    [SerializeField]
    SceneLoader loadRef;

    void Start()
    {
        thisButton = this.gameObject.GetComponent<Button>();
        thisButton.onClick.AddListener(PassTarget);
        loadRef = GameObject.Find("Handlers").GetComponent<SceneLoader>();
    }
    
    void PassTarget()
    {
        loadRef.SceneTargeter(targetScene);
        Destroy(this);
    }
}
