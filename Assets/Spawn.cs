using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    //Auto spawn
    public float spawnInterval = 8f;
    public int limitspawn = 30;
    public int limitautospawn = 10;

    //spawn system
    public GameObject[] objectToSpawnLeft;
    public GameObject[] objectToSpawnRight;
    public float objectSpeed = 5f;
    public Camera mainCamera;
    public static Spawn instance;
    public GameObject spawnParticle;

    public GameObject[] refLaneGameobject;
    public GameObject[] positionSpawn;

    public List<ObjectToSpawnList> objectScanPrefab;
    public List<GameObject> spawnedObjects = new List<GameObject>();

    public AudioSource portalSound;
    public AudioSource jumpSound;
    public bool notUseMaterial = false;
    public bool playerSpawnedRecently = false;

    public float delayspawnAuto = 0;

    private bool autoSpawnPaused = false;
    public void Start()
    {
        instance = this;
        StartCoroutine(SpawnRoutine());

    }
    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            spawnedObjects.RemoveAll(obj => obj == null);

            if (!autoSpawnPaused && spawnedObjects.Count < limitautospawn)
            {
                SpawnObject(false,0, 0);
            }

            yield return new WaitForSeconds(spawnInterval); // ให้พักก่อนลูปใหม่
        }
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnObject(true,0,0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SpawnObject(false, 0, 1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SpawnObject(false, 0, 2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SpawnObject(false, 0, 3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SpawnObject(false, 0, 4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SpawnObject(false, 0, 5);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SpawnObject(false, 0, 6);
        }

    }
    public void SpawnObject(bool playerSpawn,int cam,int key)
    {
        if (playerSpawn)
        {
            playerSpawnedRecently = true;
            autoSpawnPaused = true;
            StartCoroutine(ResumeAutoSpawnAfterDelay(10f)); // ← หยุด auto แล้วเริ่มนับเวลา
        }

        StartCoroutine(SpawnObjectRoutine(playerSpawn,cam,key));
    }
    private IEnumerator ResumeAutoSpawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        autoSpawnPaused = false;
        playerSpawnedRecently = false;
    }

    private IEnumerator SpawnObjectRoutine(bool playerSpawn,int cam,int key)
    {
        bool spawnLeft = UnityEngine.Random.value < 0.5f;

        //int randomLaneIndex = UnityEngine.Random.Range(0, refLaneGameobject.Length);
        Vector3 lanePosition = refLaneGameobject[0].transform.position;
        int leftZSpawn = spawnLeft ? 0 : 1;
        Vector3 spawnPosition = new Vector3(spawnLeft ? -5f : 5f, 0, lanePosition.z + leftZSpawn);
        int r = UnityEngine.Random.RandomRange(0, 2);
        Debug.Log(r);
        if (spawnLeft)
        {
            if (r == 0)
            {
                spawnPosition = new Vector3(positionSpawn[0].transform.position.x, 0, lanePosition.z + leftZSpawn);
            }
            else
            {
                spawnPosition = new Vector3(positionSpawn[1].transform.position.x, 0, lanePosition.z + leftZSpawn);
            }
        }
        else
        {
            if (r == 0)
            {
                spawnPosition = new Vector3(positionSpawn[2].transform.position.x, 0, lanePosition.z + leftZSpawn);
            }
            else
            {
                spawnPosition = new Vector3(positionSpawn[3].transform.position.x, 0, lanePosition.z + leftZSpawn);
            }
        }

        if (spawnedObjects.Count >= limitspawn)
        {
            if (spawnedObjects[0] != null)
            {
                var movingObj = spawnedObjects[0].GetComponent<MovingObject>();
                if (movingObj != null)
                {
                    movingObj.bounceCount = MovingObject.maxBounce;
                }
            }
        }

        Quaternion particleRotation = Quaternion.identity;
        
        GameObject particle = Instantiate(spawnParticle, spawnPosition + new Vector3(0, 1, 0), particleRotation);
        portalSound.Play();
        particle.transform.localScale = new Vector3(2f, 2f, 2f);
        Destroy(particle, 8f);

        yield return new WaitForSeconds(2f);

        int rotateY = spawnLeft ? 0 : 180;
        int selectIndex = UnityEngine.Random.Range(0, objectToSpawnLeft.Length);

        //For scanning
        string scanned = DualWebcamController.instance.textScan;
        if(cam == 2)
        {
            scanned = DualWebcamController.instance.textScan2;
        }
        foreach (var animal in objectScanPrefab)
        {
            if (animal.nameToscan == scanned && playerSpawn)
            {
                selectIndex = Array.IndexOf(objectScanPrefab.ToArray(), animal);
                break;
            }
        }
        //Spawn Animal
        if(key > 0)
        {
            selectIndex = key - 1;
        }
        objectSpeed = objectScanPrefab[selectIndex].speed;
        GameObject selectedPrefab = objectToSpawnLeft[selectIndex];
        if(cam == 2)
        {
            selectedPrefab = objectToSpawnRight[selectIndex];
        }
        GameObject obj = Instantiate(selectedPrefab,new Vector3( spawnPosition.x,spawnPosition.y,spawnPosition.z + objectScanPrefab[selectIndex].distance), Quaternion.Euler(0, rotateY, 0));
        jumpSound.Play();
        obj.transform.localScale = new Vector3(objectScanPrefab[selectIndex].laneSetScale[0].transform.localScale.x, objectScanPrefab[selectIndex].laneSetScale[0].transform.localScale.y, 0.1f);
        float timeToStart = objectScanPrefab[selectIndex].timeToStart;
        float distance = objectScanPrefab[selectIndex].distance;
        if (playerSpawn)
        {
            obj.GetComponent<CloneMaterialTexture>().camUse = cam;
        }
        else
        {
            if (notUseMaterial == false)
            {
                int randomMat = UnityEngine.Random.Range(0, objectScanPrefab[selectIndex].materialOverride.Length);
                obj.GetComponent<CloneMaterialTexture>().originalMaterial = objectScanPrefab[selectIndex].materialOverride[randomMat];
            }
        }

        if (spawnLeft)
        {
            Vector3 scale = obj.transform.localScale;
            scale.z *= -1;
            obj.transform.localScale = scale;
        }

        obj.AddComponent<MovingObject>().Initialize(spawnLeft, objectSpeed, mainCamera, refLaneGameobject, timeToStart, objectScanPrefab[selectIndex].laneSetScale, distance);

        spawnedObjects.Add(obj);

    }
    private IEnumerator ResetPlayerSpawnedFlag()
    {
        yield return new WaitForSeconds(spawnInterval + spawnInterval);
        SpawnRoutine();
        playerSpawnedRecently = false;
    }

}

[System.Serializable]

public class ObjectToSpawnList
{
    public GameObject prefab;
    public string nameToscan;
    public float speed;
    public float timeToStart;
    public float distance;
    public GameObject[] laneSetScale;
    public Material[] materialOverride;
}