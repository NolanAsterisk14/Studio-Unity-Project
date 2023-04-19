using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroMessage : MonoBehaviour
{
    public GameObject introMenu; // Initialize!
    public string activeScene;
    public bool messageSent;

    void Start()
    {
        
    }

    void Update()
    {
        activeScene = SceneManager.GetActiveScene().name;
        if (activeScene == "Planet" && messageSent == false)
        {
            InstantiateMenu();
        }
    }

    void InstantiateMenu()
    {
        Instantiate(introMenu, introMenu.transform.position, introMenu.transform.rotation, introMenu.transform.parent);
        messageSent = true;
    }

    
}
