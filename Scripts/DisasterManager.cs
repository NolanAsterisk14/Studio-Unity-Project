using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class DisasterManager : MonoBehaviour
{
    public int disasterCount = 0;
    public int buildingCount;
    public int buildInfluenceWeight = 4;
    public int minBuildingsDestroyed = 1;
    public int maxBuildingsDestroyed = 2;
    public int buildingsToDestroy;                  //Updates with the disaster time
    public int popTargeted = 0;                     //Updates automatically
    public float currentTime;                       //Set by TimeManager
    public float buildCheckCycle = 5f;              //Frequency at which build count updates
    public float buildCheckTime = 0f;               //Variable to be incremented
    public float disasterTimeFloor = 500f;          //You can change this in inspector *keep in mind these can both be slightly
    public float disasterTimeCeiling = 1000f;       //You can change this in inspector *offset by the building count for random influence
    public float disasterStartTime;
    public float t2warningStartTime;
    public float t3warningStartTime;
    public float musicStartVolume;                  //Will auto set
    public bool warningBuildingActiveT2;
    public bool warningBuildingActiveT3;
    public bool warningActive;
    public bool disasterActive;
    public bool disasterTimeReset;
    public bool gotParticleObjects;
    public bool firstTimeSet = false;
    public bool shouldBuildsUpdate = true;
    public bool warningT2Sent = false;
    public bool warningT3Sent = false;
    public bool popCheck = false;                   //Flag for checking pop being subbed
    //buildings for the earthquake to disable, and count
    public GameObject[] buildings;
    public List<GameObject> buildingsPicked;
    //Audio objects
    public AudioSource musicAudio;
    public AudioSource earthquakeAudio;
    public AudioSource disasterMusic;
    //Earthquake objects and variables
    public GameObject mapObject;                    //Initialize in inspector
    public GameObject[] zoneObjects;                //Initialize in inspector
    public GameObject[] particleObjects;            //Will auto set
    public float musicFadeDuration = 6f;            
    public float earthquakeFadeDuration = 6f;       
    public float shakingDuration = 10f;
    public float earthquakePeakVolume;              //Will auto set

    public static DisasterManager Instance { get; private set; }

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

    void Update()
    {
        currentTime = TimeManager.Instance.CurrentTime();
        if (gotParticleObjects == false)
        {
            particleObjects = GameObject.FindGameObjectsWithTag("DustParticle");
            gotParticleObjects = true;
        }
        if (musicAudio == null)
        {
            musicAudio = GameObject.Find("Audio").transform.Find("AmbientMusic").GetComponent<AudioSource>();
        }
        if (earthquakeAudio == null)
        {
            earthquakeAudio = GameObject.Find("Audio").transform.Find("EarthquakeSFX").GetComponent<AudioSource>();
        }
        if (disasterMusic == null)
        {
            disasterMusic = GameObject.Find("Audio").transform.Find("DisasterMusic").GetComponent<AudioSource>();
        }
        if (mapObject == null)
        {
            mapObject = GameObject.Find("Map_NOT_Connected");
        }
        if (firstTimeSet == false)
        {
            disasterTimeReset = true;
            firstTimeSet = true;
        }
        if (disasterTimeReset == true) //we will also randomize how many builidngs are destroyed here
        {
            disasterStartTime = currentTime + (Random.Range(disasterTimeFloor, disasterTimeCeiling) - ((buildingCount + 1) * buildInfluenceWeight));
            t2warningStartTime = (disasterStartTime - 60f);
            t3warningStartTime = (disasterStartTime - 120f);
            warningT2Sent = false;
            warningT3Sent = false;
            disasterTimeReset = false;
        }
        if ((disasterStartTime - currentTime) < 0.5f && firstTimeSet == true)
        {
            StartCoroutine("Earthquake");
            disasterTimeReset = true;
        }
        if ((t2warningStartTime - currentTime) < 0.5f && warningBuildingActiveT2 == true && warningBuildingActiveT3 == false && warningT2Sent == false)
        {
            InventoryManager.Instance.DisasterWarningT2();
            warningT2Sent = true;
        }
        if ((t3warningStartTime - currentTime) < 0.5f && warningBuildingActiveT3 == true && warningBuildingActiveT2 == false && warningT3Sent == false)
        {
            InventoryManager.Instance.DisasterWarningT3();
            warningT3Sent = true;
        }
        if (disasterActive == true && popCheck == false)
        {
            int popSub;
            if (warningBuildingActiveT2 == true || warningBuildingActiveT3 == true)
            {
                popSub = 0;
            }
            else
            {
                popSub = Random.Range(1, 3);
            }
            popTargeted = popSub;
            popCheck = true;
        }
        if (shouldBuildsUpdate == true)
        {
            if (buildCheckTime < buildCheckCycle)
            {
                buildCheckTime += Time.deltaTime;
            }
            if (buildCheckTime >= buildCheckCycle)
            {
                CheckBuildings();
            }
        }
    }

    void CheckBuildings()
    {
        buildings = GameObject.FindGameObjectsWithTag("Building");
        buildingCount = buildings.Length;
        buildCheckTime = 0f;
        buildingsToDestroy = Random.Range(minBuildingsDestroyed, maxBuildingsDestroyed + 1);
        buildingsPicked.Clear();
        if (buildings.Length > 0)
        {
            for (int i = 0; i < buildingsToDestroy; i++)
            {
                buildingsPicked.Add(buildings[Random.Range(0, buildings.Length)]);
            }
            buildingsPicked = buildingsPicked.Distinct().ToList();
        }
    }

    void DestroyBuilding(GameObject building)
    {
        building.GetComponent<BuildingPassive>().EarthquakeDestroy();
    }

    public void WarningBuildingActiveT2(bool isActive)
    {
        warningBuildingActiveT2 = isActive;
    }

    public void WarningBuildingActiveT3(bool isActive)
    {
        warningBuildingActiveT3 = isActive;
    }

    IEnumerator Earthquake()
    {
        popCheck = false;
        disasterActive = true;
        if (musicAudio != null)
        {
            musicStartVolume = musicAudio.volume;                                                               //get start volume
            for (float i = musicFadeDuration; i >= 0f; i -= Time.deltaTime)                                     //turn music down
            {
                musicAudio.volume = i / 10f;
                yield return null;
            }
            musicAudio.volume = 0f;                                                                             //set to 0 here to ensure it's off
        }
        yield return new WaitForSecondsRealtime(1);                                                             //pause for 1 second
        disasterMusic.Play();
        if (earthquakeAudio != null)
        {
            earthquakePeakVolume = earthquakeAudio.volume;                                                      //get peak volume
            earthquakeAudio.volume = 0f;                                                                        //set to 0 here so it can play before the loop
            earthquakeAudio.Play();                                                                             //Play, no volume yet
            foreach (GameObject particleObject in particleObjects)
            {
                particleObject.GetComponent<ParticleSystem>().Play();                                           //Turn them particles on
            }
            for (float i = 0f; i <= earthquakeFadeDuration; i += Time.deltaTime)                                //turn earthquake up
            {
                float j = Mathf.Lerp(0, 1, (i / 6));
                earthquakeAudio.volume = j;
                yield return null;
            }
            mapObject.GetComponent<Animator>().SetBool("ShakeBool", true);                                      //turn on both anims
            foreach (GameObject zoneObject in zoneObjects)
            {
                zoneObject.GetComponent<Animator>().SetBool("ShakeBool", true);
            }
            earthquakeAudio.volume = earthquakePeakVolume;                                                      //set to peak volume
        }
        yield return new WaitForSecondsRealtime(shakingDuration);                                               //pause for shake duration
        buildingsPicked.ForEach(b => DestroyBuilding(b));
        InventoryManager.Instance.Sub(1, (buildingsPicked.Count + 2));                                          //Subtract happiness here
        InventoryManager.Instance.Sub(0, popTargeted);
        popCheck = false;
        if (earthquakeAudio != null)
        {
            for (float i = earthquakeFadeDuration; i > 0f; i -= Time.deltaTime)                                 //turn earthquake down
            {
                float j = Mathf.Lerp(0, 1, (i / 10));
                earthquakeAudio.volume = j;
                yield return null;
            }
            mapObject.GetComponent<Animator>().SetBool("ShakeBool", false);
            foreach (GameObject zoneObject in zoneObjects)
            {
                zoneObject.GetComponent<Animator>().SetBool("ShakeBool", false);
            }
            foreach (GameObject particleObject in particleObjects)
            {
                particleObject.GetComponent<ParticleSystem>().Stop();                                           //Turn them particles off
            }
            earthquakeAudio.Stop();                                                                             //Stop, then reset volume to peak
            earthquakeAudio.volume = earthquakePeakVolume;
        }
        disasterMusic.Stop();
        yield return new WaitForSecondsRealtime(1);                                                             //pause for 1 second
        if (musicAudio != null)
        {
            for (float i = 0f; i <= musicFadeDuration; i += Time.deltaTime)                                     //turn music back up
            {
                musicAudio.volume = i / 10f;
                yield return null;
            }
            musicAudio.volume = musicStartVolume;                                                               //set music back to start volume
        }
        disasterCount++;
        disasterActive = false;
        yield return null;
    }
    
}
