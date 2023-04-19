using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    GameObject mainCamera;
    private Transform camTransform;
    [SerializeField]
    private Vector3 camPos;
    /* the following are series of options for the developer to choose how the camera starts
     * and how the camera is controlled in play mode */
    [SerializeField]
    [Tooltip("Speed camera moves using wasd keys")]
    private float speed = 10;
    [SerializeField]
    [Tooltip("Speed camera moves using drag movement")]
    private float dragSpeed = 20;
    [SerializeField]
    [Tooltip("Speed camera moves using edge movement")]
    private float edgeSpeed = 20;
    [SerializeField]
    [Tooltip("Speed camera zooms in and out")]
    private float zoomSpeed = 10;
    [SerializeField]
    [Tooltip("How far from the edge of the screen the cursor will move the camera (0.01-0.99)")]
    private float edgeBounds = 0.15f;
    [SerializeField]
    [Tooltip("Camera starting angle")]
    private float angle = 20;
    [SerializeField]
    [Tooltip("Camera starting height")]
    private float height = 15;
    [SerializeField]
    [Tooltip("Minimum zoom value for camera")]
    private float minHeight = 15;
    [SerializeField]
    [Tooltip("Maximum zoom value for camera")]
    private float maxHeight = 40;
    [SerializeField]
    [Tooltip("Should Camera be moved at all right now?")]
    private bool canControl = true;
    [SerializeField]
    [Tooltip("Should Camera be moved with wasd keys?")]
    private bool keyControl = true;
    [SerializeField]
    [Tooltip("Should Camera be moved by middle mouse drag?")]
    private bool dragControl = true;
    [SerializeField]
    [Tooltip("Should Camera be moved by cursor approaching screen edges?")]
    private bool edgeControl = true;
    [SerializeField]
    [Tooltip("Should Camera zoom in and out using scroll wheel?")]
    private bool zoomControl = true;
    // various variables needed for calculations
    [SerializeField]
    private Vector2 mousePos;
    [SerializeField]
    private Vector2 mouseCoord;
    [SerializeField]
    private float mouseDelta;

    public static CameraController Instance { get; private set; }

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

    void Start()
    {   //store values for camera
        mainCamera = this.gameObject;
        camTransform = this.gameObject.transform;
        camTransform.rotation = Quaternion.Euler(angle, 0, 0);
        camTransform.position = new Vector3(camTransform.position.x, height, camTransform.position.z);
    }

    void Update()
    { 
        camPos = camTransform.position;
        //store mouse position
        mousePos = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        //store raw mouse coordinates
        mouseCoord = Input.mousePosition;
        //store scroll wheel value
        mouseDelta = Input.mouseScrollDelta.y;
        //control camera with keys
        if (keyControl == true && canControl == true)
        {
            if (Input.GetKey("w") == true)
            {
                camTransform.Translate(Vector3.forward * Time.deltaTime * speed, Space.World);
            }
            if (Input.GetKey("a") == true)
            {
                camTransform.Translate(Vector3.left * Time.deltaTime * speed, Space.World);
            }
            if (Input.GetKey("d") == true)
            {
                camTransform.Translate(Vector3.right * Time.deltaTime * speed, Space.World);
            }
            if (Input.GetKey("s") == true)
            {
                camTransform.Translate(Vector3.back * Time.deltaTime * speed, Space.World);
            }
        }
        //control camera with middle mouse drag
        if (dragControl == true && canControl == true)
        {
            if (Input.GetKey("mouse 2") == true)
            {
                camTransform.position -= new Vector3(mousePos.x * Time.deltaTime * dragSpeed * 20, 0, mousePos.y * Time.deltaTime * dragSpeed * 20);
            }
        }
        //control camera with cursor approaching edges
        if (edgeControl == true && canControl == true)
        {
            if (mouseCoord.x < (Screen.width * 0.5 * edgeBounds))
            {
                camTransform.Translate(Vector3.left * Time.deltaTime * edgeSpeed, Space.World);
            }
            if (mouseCoord.x > (Screen.width * 0.5 * (2 - edgeBounds)))
            {
                camTransform.Translate(Vector3.right * Time.deltaTime * edgeSpeed, Space.World);
            }
            if (mouseCoord.y < (Screen.height * 0.5 * edgeBounds))
            {
                camTransform.Translate(Vector3.back * Time.deltaTime * edgeSpeed, Space.World);
            }
            if (mouseCoord.y > (Screen.height * 0.5 * (2 - edgeBounds)))
            {
                camTransform.Translate(Vector3.forward * Time.deltaTime * edgeSpeed, Space.World);
            }
        }
        //Zoom control using scroll wheel
        if (zoomControl == true && canControl == true)
        {
            if (mouseDelta > 0 && camPos.y > minHeight)
            {
                camTransform.Translate(Vector3.forward * Time.deltaTime * 5f * zoomSpeed, Space.Self);
            }
            if (mouseDelta > 0 && camPos.y <= minHeight)
            {
                camTransform.position = new Vector3(camPos.x, minHeight, camPos.z);
            }
            if (mouseDelta < 0 && camPos.y < maxHeight)
            {
                camTransform.Translate(Vector3.back * Time.deltaTime * 5f * zoomSpeed, Space.Self);
            }
            if (mouseDelta < 0 && camPos.y >= maxHeight)
            {
                camTransform.position = new Vector3(camPos.x, maxHeight, camPos.z);
            }
        }
    }

    public void ControlToggle()
    {
        canControl = !canControl;
    }

    public void ControlOn()
    {
        canControl = true;
    }

    public void ControlOff()
    {
        canControl = false;
    }

    public void ZoomOn()
    {
        zoomControl = true;
    }

    public void ZoomOff()
    {
        zoomControl = false;
    }
}

