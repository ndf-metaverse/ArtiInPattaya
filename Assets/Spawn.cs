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
    public GameObject[] objectToSpawn;
    public float objectSpeed = 5f;
    public Camera mainCamera;
    public static Spawn instance;
    public GameObject spawnParticle;

    public GameObject[] refLaneGameobject;

    public List<ObjectToSpawnList> objectScanPrefab;
    public List<GameObject> spawnedObjects = new List<GameObject>();

    public AudioSource portalSound;
    public bool notUseMaterial = false;
    private bool playerSpawnedRecently = false;

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
            if (spawnedObjects.Count < limitautospawn)
            {
                SpawnObject(false);
            }
            //if (playerSpawnedRecently)
            //{
            //    yield return new WaitForSeconds(spawnInterval * 2);
            //}
            //else
            //{
            //    yield return new WaitForSeconds(spawnInterval);
            //}
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnObject(true);
        }
    }
    public void SpawnObject(bool playerSpawn)
    {
        if (playerSpawn)
        {
            playerSpawnedRecently = true;
            StartCoroutine(ResetPlayerSpawnedFlag());
        }

        StartCoroutine(SpawnObjectRoutine(playerSpawn));
    }
    private IEnumerator SpawnObjectRoutine(bool playerSpawn)
    {
        bool spawnLeft = UnityEngine.Random.value < 0.5f;

        //int randomLaneIndex = UnityEngine.Random.Range(0, refLaneGameobject.Length);
        Vector3 lanePosition = refLaneGameobject[0].transform.position;
        int leftZSpawn = spawnLeft ? 0 : 1;
        Vector3 spawnPosition = new Vector3(spawnLeft ? -5f : 5f, 0, lanePosition.z + leftZSpawn);

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
        int selectIndex = UnityEngine.Random.Range(0, objectToSpawn.Length);

        //For scanning
        string scanned = QRDecodeTest2.instance.textScan;
        foreach (var animal in objectScanPrefab)
        {
            if (animal.nameToscan == scanned)
            {
                selectIndex = Array.IndexOf(objectScanPrefab.ToArray(), animal);
                break;
            }
        }
        //Spawn Animal
        objectSpeed = objectScanPrefab[selectIndex].speed;
        GameObject selectedPrefab = objectToSpawn[selectIndex];
        GameObject obj = Instantiate(selectedPrefab, spawnPosition, Quaternion.Euler(0, rotateY, 0));
        obj.transform.localScale = new Vector3(refLaneGameobject[0].transform.localScale.x, refLaneGameobject[0].transform.localScale.y, selectedPrefab.transform.localScale.z);
        float timeToStart = objectScanPrefab[selectIndex].timeToStart;
        if (playerSpawn)
        {

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

        obj.AddComponent<MovingObject>().Initialize(spawnLeft, objectSpeed, mainCamera, refLaneGameobject, timeToStart);

        spawnedObjects.Add(obj);

    }
    private IEnumerator ResetPlayerSpawnedFlag()
    {
        yield return new WaitForSeconds(5f);
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
    public Material[] materialOverride;
}