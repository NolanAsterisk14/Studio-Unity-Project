using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;

public class DroneController : MonoBehaviour
{
    private GameObject globalSelection; //Read only! Use 'SelectionSet' to change this
    public GameObject progressBar;  //Set in the inspector!
    public GameObject mainCamera; //Set in the inspector, or start will find it
    public GameObject currentProgressBar;
    public List<GameObject> collectablesInRange = new List<GameObject>();
    public List<Material[]> collectableMatArrays = new List<Material[]>();
    public List<Color[]> collectableColorArrays = new List<Color[]>();
    public GameObject lastSelectedCollectable;
    public AudioSource droneAudio;
    public AudioClip droneMoveStart;
    public AudioClip droneMoveLoop;
    public AudioClip droneCollectStart;
    public AudioClip droneCollectFinish;
    public Material[] lastSelectedMaterials;
    public SphereCollider dSphereTrigger;
    public Slider progressSlider;
    public bool selectState = false;
    public bool moveState = false;
    public bool disableMovement = false; //Right now this is JUST for the collect sequence
    public bool inCollectRange = false;
    public bool collectTimerActive = false;
    public bool inventoryOpenSlot;
    public bool inventorySlotsFull;
    public bool canCollect = true;
    public bool areIndexesFull;
    public bool onCollectRoute;
    public bool speedL1;
    public bool speedL2;
    public bool speedL3;
    public Animator anim;
    public Vector3 moveTarget;
    public Vector3 moveStartPos;
    public float mVal;
    [Tooltip("Height drone will hover at, relative to ground.")]
    public float hOffset = 5f;
    [Tooltip("Speed value for drone movement.")]
    public float droneSpeed = 12f;
    public float droneSpeedL1 = 12f;
    public float droneSpeedL2 = 13.5f;
    public float droneSpeedL3 = 15f;
    [Tooltip("Distance [in unity units] from click point drone must reach to stop")]
    public float precisionThreshold = 1f;
    [Tooltip("Radius of drone's sphere trigger")]
    public float collectRange = 10f;
    public float progressOffset = 3f;
    public float collectTimer;
    public float collectTime;
    public float droneFuelCycle = 15f;  //Rate that drone has fuel taken
    public float droneFuelTime = 0f;
    public int[] collectRIndex;
    public int[] collectRAmount;
    public Vector3 progressSpawnPos;
    public Color inRangeColor = new Color(0.1933962f, 0.3501418f, 1f, 1f);
    public Color ableGatherColor = new Color(0.1921569f, 1f, 0.3546591f, 1f);
    public Color unableGatherColor = new Color(1f, 0.2122642f, 0.2562469f, 1f);

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = GameObject.Find("Main Camera");
        }
        if (anim == null)
        {
            if (TryGetComponent(out Animator tempAnim))
            {
                anim = tempAnim;
            }
        }
        if (dSphereTrigger == null)
        {
            SphereCollider[] sphereColliders = GetComponents<SphereCollider>();
            foreach (SphereCollider collider in sphereColliders)
            {
                if (collider.isTrigger == true)
                {
                    dSphereTrigger = collider;
                }
            }
        }
        //if (droneInventoryRef == null)
        //{
        //    if (TryGetComponent(out DroneInventory droneInvRef))
        //    {
        //        droneInventoryRef = droneInvRef;
        //    }
        //}
        else
        {
            Debug.LogWarning("Drone controller didn't find either an animator, or a camera");
        }
        if (TryGetComponent(out AudioSource audioSource))
        {
            droneAudio = audioSource;
        }
        droneMoveStart = Resources.Load<AudioClip>("Audio/DroneMovement2");
        droneMoveLoop = Resources.Load<AudioClip>("Audio/DroneMovementLoop");
        droneCollectStart = Resources.Load<AudioClip>("Audio/ResourceCollect");
        droneCollectFinish = Resources.Load<AudioClip>("Audio/ResourceCollectShort");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Collectable")
        {
            other.tag = "CollectableInRange";
            Material[] materials = other.GetComponent<MeshRenderer>().materials;
            Color[] matColors = new Color[materials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                matColors[i] = materials[i].color;
            }
            collectablesInRange.Add(other.gameObject);
            collectableMatArrays.Add(materials);
            collectableColorArrays.Add(matColors);
            foreach (Material material in materials)
            {
                material.color = inRangeColor;
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Collectable" && selectState == true)
        {
            other.tag = "CollectableInRange";
            Material[] materials = other.GetComponent<MeshRenderer>().materials;
            foreach (Material material in materials)
            {
                material.color = inRangeColor;
            }
        }
        if (other.tag == "CollectableInRange" && selectState == false)
        {
            other.tag = "Collectable";
            int objIndex = collectablesInRange.IndexOf(other.gameObject);
            Color[] matColors = collectableColorArrays[objIndex];
            Material[] materials = other.GetComponent<MeshRenderer>().materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].color = matColors[i];
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "CollectableInRange")
        {
            other.tag = "Collectable";
            Color[] matColors = new Color[0];
            Material[] materials = other.GetComponent<MeshRenderer>().materials;
            int objIndex = collectablesInRange.IndexOf(other.gameObject);
            if (collectableColorArrays[objIndex] != null)
            {
                matColors = collectableColorArrays[objIndex];
            }
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].color = matColors[i];
            }
            if (collectableMatArrays.Contains(materials) == true)
            {
                collectableMatArrays.Remove(materials);
            }
            if (collectableColorArrays.Contains(matColors) == true)
            {
                collectableColorArrays.Remove(matColors);
            }
            if (collectablesInRange.Contains(other.gameObject) == true)
            {
                collectablesInRange.Remove(other.gameObject);
            }
        }
    }

    void Update()
    {
        globalSelection = SelectionManager.Instance.SelectionGet(); //This will update the current global selection
        if (speedL1 == true && speedL2 == false && speedL3 == false)
        {
            droneSpeed = droneSpeedL1;
        }
        if (speedL2 == true && speedL3 == false)
        {
            droneSpeed = droneSpeedL2;
        }
        if (speedL3 == true)
        {
            droneSpeed = droneSpeedL3;
        }
        if (dSphereTrigger != null)
        {
            dSphereTrigger.radius = collectRange;
        }
        //if (droneInventoryRef != null)
        //{
        //    inventoryOpenSlot = droneInventoryRef.SlotsOccupiedCheck();
        //    inventorySlotsFull = droneInventoryRef.SlotsFullCheck();
        //    if (inventoryOpenSlot == true)
        //    {
        //        canCollect = true;
        //    }
        //    if (inventoryOpenSlot == false && inventorySlotsFull == false)
        //    {
        //        canCollect = droneInventoryRef.OccupiedCanStore(collectRIndex, collectRAmount);
        //    }
        //    if (inventoryOpenSlot == false && inventorySlotsFull == true)
        //    {
        //        canCollect = false;
        //    }
        //}
        if (collectTimerActive == true)
        {
            disableMovement = true;
            collectTimer += Time.deltaTime;
            if (currentProgressBar != null )
            {
                if (progressSlider == null)
                {
                    progressSlider = currentProgressBar.transform.GetChild(0).gameObject.GetComponent<Slider>();
                    progressSlider.maxValue = collectTime;
                }
                if (progressSlider != null)
                {
                    progressSlider.value = collectTimer;
                }
            }
            if (collectTimer >= collectTime)
            {
                collectTimerActive = false;
                CollectFinish();
            }
        }
        if (globalSelection == this.gameObject) //Check if current selection matches this gameobject
        {
            selectState = true;
        }
        if (globalSelection != this.gameObject)
        {
            selectState = false;
        }
        if (currentProgressBar != null) //Progress bar look at camera
        {
            currentProgressBar.transform.LookAt((2 * currentProgressBar.transform.position - mainCamera.transform.position), Vector3.up);
        }
        if (moveState == true)
        {
            droneFuelTime += Time.deltaTime;
            if (droneFuelTime >= droneFuelCycle)
            {
                droneFuelTime = 0f;
                InventoryManager.Instance.Sub(4, 1);
            }
        }

        if (Input.GetMouseButtonDown(0)) //Checks left mouse click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "Drone")
                {
                    SelectionManager.Instance.SelectionSet(hit.transform.gameObject);   //This will set the current global selection
                }                                                                       //If a drone is hit by the raycast
                else
                {
                    SelectionManager.Instance.SelectionFlush();                         //Clicking elsewhere will clear the selection
                }
            }
        }
        
        if (Input.GetMouseButtonDown(1)) //Checks right mouse click
        {
            if (selectState == true && disableMovement == false)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                int objectLayer;
                string objectTag;
                if (Physics.Raycast(ray, out hit))
                {
                    moveTarget = hit.point;                     //Sets target where raycast hits
                    moveTarget.y += hOffset;                    //Add height offset to target
                    moveStartPos = this.transform.position;     //Gets current position of drone
                    objectLayer = hit.transform.gameObject.layer;
                    objectTag = hit.transform.gameObject.tag;
                    
                    if (objectLayer == 6)
                    {
                        moveState = true;                           //Begins the movement
                        droneAudio.clip = droneMoveStart;
                        droneAudio.volume = 0.25f;
                        droneAudio.Play();
                        anim.SetTrigger("DroneMoving");
                    }
                    if (objectTag == "CollectableInRange") //update these values regardless of the canCollect status
                    {
                        collectRIndex = hit.transform.GetComponent<CollectAmount>().resourceIndex;
                        collectRAmount = hit.transform.GetComponent<CollectAmount>().resourceAmount;
                        collectTime = hit.transform.GetComponent<CollectAmount>().collectTime;
                        for (int i = 0; i < collectRIndex.Length; i++)
                        {
                            bool indexFull = InventoryManager.Instance.IndexFullCheck(collectRIndex[i]);
                            if (indexFull == false)
                            {
                                canCollect = true;
                                break;
                            }
                            if (indexFull == true && i == collectRIndex.Length - 1) //check for last iteration of loop
                            {
                                canCollect = false;
                            }
                        }
                    }
                    if (objectTag == "CollectableInRange" && canCollect == true)
                    {
                        lastSelectedCollectable = hit.transform.gameObject;
                        lastSelectedMaterials = lastSelectedCollectable.GetComponent<MeshRenderer>().materials;
                        foreach (Material mat in lastSelectedMaterials)
                        {
                            mat.color = ableGatherColor;
                        }
                        progressSpawnPos = hit.transform.position;
                        progressSpawnPos.y += progressOffset;
                        disableMovement = true;
                        onCollectRoute = true;
                        moveState = true;
                        droneAudio.clip = droneMoveStart;
                        droneAudio.volume = 0.25f;
                        droneAudio.Play();
                        anim.SetTrigger("DroneMoving");
                    }
                    if (objectTag == "CollectableInRange" && canCollect == false)
                    {
                        lastSelectedCollectable = hit.transform.gameObject;
                        lastSelectedMaterials = lastSelectedCollectable.GetComponent<MeshRenderer>().materials;
                        foreach (Material mat in lastSelectedMaterials)
                        {
                            mat.color = unableGatherColor;
                        }
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        Vector3 rotationTarget;
        Quaternion droneRotation;
        Vector3 droneCurrent = this.transform.position;
        float droneCompare = Vector3.Distance(droneCurrent, moveTarget);

        if (moveState == true)
        {
            rotationTarget = moveTarget - this.transform.position;      
            droneRotation = Quaternion.LookRotation(rotationTarget, Vector3.up);    //Assigns current rotation to temp quaternion
            rotationTarget.y = 0;

            if (droneCompare < precisionThreshold)                                  //See if coordinates match the end coordinates
            {
                droneRotation = Quaternion.LookRotation(rotationTarget, Vector3.up);
                this.transform.rotation = droneRotation;
                moveState = false;
                anim.ResetTrigger("DroneMoving");
                droneAudio.Stop();
                if (onCollectRoute == true)
                {
                    disableMovement = false;
                    currentProgressBar = Instantiate(progressBar, progressSpawnPos, progressBar.transform.rotation);
                    collectTimer = 0f;
                    collectTimerActive = true;
                    droneAudio.clip = droneCollectStart;
                    droneAudio.volume = 0.5f;
                    droneAudio.Play();
                    onCollectRoute = false;
                }
                return;
            }

            //mVal += Time.deltaTime;                                               //If end isn't reached, increment the interp value *old method, big oopsie*
            this.transform.rotation = droneRotation;                                //Then, set the rotation to face the target
            this.transform.Translate(Vector3.forward * Time.deltaTime * droneSpeed);//New method, just pushes drone at given rate
            //this.transform.position = droneTarget;                                //And, set current position to temp vector

            if (moveState == false)
            {
                droneRotation = Quaternion.LookRotation(rotationTarget, Vector3.up);
                this.transform.rotation = droneRotation;
                mVal = 0;
            }
        }

    }

    void CollectFinish()
    {
        disableMovement = false;
        droneAudio.clip = droneCollectFinish;
        droneAudio.volume = 0.5f;
        droneAudio.Play();
        Destroy(currentProgressBar);
        for (int i = 0; i < collectRIndex.Length; i++)
        {
            InventoryManager.Instance.Add(collectRIndex[i], collectRAmount[i]);     //I changed this to simply add the resources to the inventory instead of doing
        }                                                                           //The drone inventory :(
        if (lastSelectedCollectable.name != "Watermill")
        {
            Destroy(lastSelectedCollectable);
        }
        if (lastSelectedCollectable.name == "Watermill")
        {
            Color[] matColors = new Color[0];
            Material[] materials = lastSelectedCollectable.GetComponent<MeshRenderer>().materials;
            int objIndex = collectablesInRange.IndexOf(lastSelectedCollectable.gameObject);
            if (collectableColorArrays[objIndex] != null)
            {
                matColors = collectableColorArrays[objIndex];
            }
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].color = matColors[i];
            }
            if (collectableMatArrays.Contains(materials) == true)
            {
                collectableMatArrays.Remove(materials);
            }
            if (collectableColorArrays.Contains(matColors) == true)
            {
                collectableColorArrays.Remove(matColors);
            }
            if (collectablesInRange.Contains(lastSelectedCollectable.gameObject) == true)
            {
                collectablesInRange.Remove(lastSelectedCollectable.gameObject);
            }
            lastSelectedCollectable.tag = "Untagged";
        }
    }

}
