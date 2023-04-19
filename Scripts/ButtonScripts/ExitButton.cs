using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour
{
    public Button exitButton; //initialize in inspector!
    public GameObject parentCanvas; //initialize!
    public GameObject currentOpenMenu;

    void Start()
    {
        if (parentCanvas.name != "CommandCenterMenu(Clone)" && parentCanvas.name != "CollectMenu(Clone)")
        {
            TimeManager.Instance.TimeStop();
            CameraController.Instance.ControlOff();
        }
        if (parentCanvas.name == "CommandCenterMenu(Clone)")
        {
            CameraController.Instance.ZoomOff();
        }
        if (parentCanvas == null)
        {
            parentCanvas = this.gameObject.transform.parent.parent.parent.gameObject;
        }
        if (exitButton == null)
        {
            exitButton = this.gameObject.GetComponent<Button>();
        }
        exitButton.onClick.AddListener(DestroyMenu);
    }

    void Update()
    {
       
    }

    public void DestroyMenu()
    {
        TimeManager.Instance.TimeStart();
        CameraController.Instance.ControlOn();
        CameraController.Instance.ZoomOn();
        Destroy(parentCanvas);
    }
}
